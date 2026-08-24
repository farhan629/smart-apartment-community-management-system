using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ResidentVisitorService.Application.Common.Mappings;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Extensions;
using ResidentVisitorService.Infrastructure.Extensions;
using ResidentVisitorService.Infrastructure.Persistence.Seeders;
using Serilog;
using Shared.SharedLibrary.Http;
using Shared.SharedLibrary.Middleware;
using Shared.SharedLibrary.Services;

/// <summary>
/// Entry point for configuring and running the Resident Visitor Service API.
/// </summary>
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/resident-visitor-service-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();

var identityBaseUrl =
    builder.Configuration["IdentityService:BaseUrl"]
    ?? throw new InvalidOperationException(ResidentVisitorConstants.Errors.IdentityBaseUrlMissing);

builder.Services.AddTransient<AuthenticatingDelegatingHandler>();

builder
    .Services.AddHttpClient<IPermissionService, PermissionService>(client =>
    {
        client.BaseAddress = new Uri(identityBaseUrl);
    })
    .AddHttpMessageHandler<AuthenticatingDelegatingHandler>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddApplication();

var jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(ResidentVisitorConstants.Errors.JwtKeyMissing);
var jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(ResidentVisitorConstants.Errors.JwtIssuerMissing);
var jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(ResidentVisitorConstants.Errors.JwtAudienceMissing);

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddCors(options =>
    options.AddPolicy(
        ResidentVisitorConstants.CorsPolicy.AllowAll,
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    )
);

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        ResidentVisitorConstants.Swagger.ApiVersion,
        new OpenApiInfo
        {
            Title = ResidentVisitorConstants.Swagger.ApiTitle,
            Version = ResidentVisitorConstants.Swagger.ApiVersion,
            Description = ResidentVisitorConstants.Swagger.ApiDescription,
        }
    );

    c.AddSecurityDefinition(
        ResidentVisitorConstants.Swagger.BearerScheme,
        new OpenApiSecurityScheme
        {
            Name = ResidentVisitorConstants.Swagger.AuthHeaderName,
            Type = SecuritySchemeType.Http,
            Scheme = ResidentVisitorConstants.Swagger.BearerScheme,
            BearerFormat = ResidentVisitorConstants.Swagger.BearerFormat,
            In = ParameterLocation.Header,
            Description = ResidentVisitorConstants.Swagger.BearerDescription,
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = ResidentVisitorConstants.Swagger.BearerScheme,
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint(
            ResidentVisitorConstants.Swagger.SwaggerEndpoint,
            ResidentVisitorConstants.Swagger.SwaggerDisplayName
        )
    );
}

Log.Information("Resident Visitor Service starting up...");

app.UseGlobalExceptionMiddleware();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(ResidentVisitorConstants.CorsPolicy.AllowAll);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

Log.CloseAndFlush();

/// <summary>Partial Program class for integration testing.</summary>
public partial class Program { }

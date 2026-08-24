using System.Text;
using ComplaintMaintenanceService.API.Constants;
using ComplaintMaintenanceService.API.Grpc;
using ComplaintMaintenanceService.Infrastructure.Extensions;
using ComplaintMaintenanceService.Infrastructure.Persistence.DBContext;
using ComplaintMaintenanceService.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Shared.SharedLibrary.Http;
using Shared.SharedLibrary.Middleware;
using Shared.SharedLibrary.Services;

/// <summary>
/// Entry point for configuring and running the Complaint Maintenance Service API.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configures Serilog for logging to console and file with daily rolling.
/// </summary>
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        ApiStartupConstants.Logging.LogFilePathTemplate,
        rollingInterval: RollingInterval.Day
    )
    .CreateLogger();

builder.Host.UseSerilog();

/// <summary>
/// Configures the database context with PostgreSQL.
/// </summary>
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(ApiStartupConstants.Database.DefaultConnectionKey)
            ?? throw new InvalidOperationException(
                ApiStartupConstants.Database.DefaultConnectionMissing
            ),
        x =>
            x.MigrationsHistoryTable(
                ApiStartupConstants.Database.MigrationsHistoryTable,
                ApiStartupConstants.Database.Schema
            )
    )
);

// Add this line after builder.Services.AddDbContext<AppDbContext>(...)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(ComplaintMaintenanceService.Application.Features.Complaints.Commands.CreateComplaintCommand).Assembly
    );
    cfg.AddOpenBehavior(
        typeof(ComplaintMaintenanceService.Application.Common.Behaviors.ValidationBehavior<,>)
    );
});

/// <summary>
/// Registers Permission Service for calling Identity Service.
/// </summary>
builder.Services.AddTransient<AuthenticatingDelegatingHandler>();
builder
    .Services.AddHttpClient<IPermissionService, PermissionService>(client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration[ApiStartupConstants.IdentityService.BaseUrlConfigPath]!
        );
    })
    .AddHttpMessageHandler<AuthenticatingDelegatingHandler>();
builder.Services.AddMemoryCache();

builder.Services.AddAutoMapper(
    typeof(ComplaintMaintenanceService.Application.Features.Complaints.Commands.CreateComplaintCommand).Assembly
);
builder.Services.AddValidatorsFromAssembly(
    typeof(ComplaintMaintenanceService.Application.Features.Complaints.Validators.CreateComplaintValidator).Assembly
);

/// <summary>
/// Registers repositories and services for dependency injection.
/// </summary>
// TODO: Add your repository and service registrations here

/// <summary>
/// Configures JWT authentication with token validation parameters.
/// </summary>
var jwtKey =
    builder.Configuration[ApiStartupConstants.Jwt.KeyConfigPath]
    ?? throw new InvalidOperationException(ApiStartupConstants.Jwt.KeyMissing);
var jwtIssuer =
    builder.Configuration[ApiStartupConstants.Jwt.IssuerConfigPath]
    ?? throw new InvalidOperationException(ApiStartupConstants.Jwt.IssuerMissing);
var jwtAudience =
    builder.Configuration[ApiStartupConstants.Jwt.AudienceConfigPath]
    ?? throw new InvalidOperationException(ApiStartupConstants.Jwt.AudienceMissing);

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

/// <summary>
/// Configures CORS policy to allow all origins, methods, and headers.
/// </summary>
builder.Services.AddCors(options =>
{
    var allowedOrigins =
        builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? new[] { "http://localhost:4200" };

    options.AddPolicy(
        ApiStartupConstants.Cors.AllowAllPolicy,
        policy =>
            policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
    );
});

/// <summary>
/// Adds authorization services.
/// </summary>
builder.Services.AddAuthorization();

/// <summary>
/// Adds controllers.
/// </summary>
builder.Services.AddControllers();
builder.Services.AddGrpc();

/// <summary>
/// Configures Swagger for API documentation with JWT authentication support.
/// </summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ComplaintMaintenanceService.Infrastructure.Persistence.Seeders.DatabaseSeeder>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        ApiStartupConstants.Swagger.DocName,
        new OpenApiInfo
        {
            Title = ApiStartupConstants.Swagger.ApiTitle,
            Version = ApiStartupConstants.Swagger.DocName,
            Description = ApiStartupConstants.Swagger.ApiDescription,
        }
    );

    c.AddSecurityDefinition(
        ApiStartupConstants.Security.BearerScheme,
        new OpenApiSecurityScheme
        {
            Name = ApiStartupConstants.Security.AuthorizationHeaderName,
            Type = SecuritySchemeType.Http,
            Scheme = ApiStartupConstants.Security.BearerScheme,
            BearerFormat = ApiStartupConstants.Security.JwtBearerFormat,
            In = ParameterLocation.Header,
            Description = ApiStartupConstants.Security.BearerDescription,
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
                        Id = ApiStartupConstants.Security.BearerScheme,
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

var app = builder.Build();

/// <summary>
/// Runs database seeding on startup — inserts or updates reference sets, reference
/// terms, and categories from the embedded CSV seed files.
/// </summary>
// using (var scope = app.Services.CreateScope())
// {
//     var seeder =
//         scope.ServiceProvider.GetRequiredService<ComplaintMaintenanceService.Infrastructure.Persistence.Seeders.DatabaseSeeder>();
//     await seeder.SeedAsync();
// }

/// <summary>
/// Configures middleware pipeline with Swagger UI for development environment.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            $"/swagger/{ApiStartupConstants.Swagger.DocName}/swagger.json",
            ApiStartupConstants.Swagger.SwaggerUiTitle
        );
    });
}

Log.Information("Complaint Maintenance Service starting up...");

/// <summary>
/// Configures global exception handling, HTTPS redirection, CORS, authentication,
/// authorization, and controller endpoints.
/// </summary>
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors(ApiStartupConstants.Cors.AllowAllPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<CmsGrpcService>();
app.Run();

Log.CloseAndFlush();

/// <summary>
/// Partial Program class for integration testing.
/// </summary>
public partial class Program { }

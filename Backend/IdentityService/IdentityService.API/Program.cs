using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using IdentityService.API.Grpc;
using IdentityService.Application.Common.Mappings;
using IdentityService.Application.Features.Auth.Validators;
using IdentityService.Application.Features.Permissions.Queries;
using IdentityService.Infrastructure.Extensions;
using IdentityService.Infrastructure.Persistence.DBContext;
using IdentityService.Infrastructure.Repositories;
using IdentityService.Infrastructure.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Shared;
using Shared.SharedLibrary;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Http;
using Shared.SharedLibrary.Middleware;
using Shared.SharedLibrary.Services;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
/// <summary>
/// Entry point for configuring and running the Identity Service API.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(
        5113,
        listenOptions =>
        {
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
        }
    );
    options.ListenLocalhost(
        5114,
        listenOptions =>
        {
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
        }
    );
});

/// <summary>
/// Configures Serilog for logging to console and file with daily rolling.
/// </summary>
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(LoggingConstants.LogFilePathTemplate, rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

/// <summary>
/// Configures the database context with PostgreSQL.
/// </summary>
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(ConfigKeys.DefaultConnection)
            ?? throw new InvalidOperationException(ConfigErrorMessages.DefaultConnectionMissing),
        x =>
            x.MigrationsHistoryTable(
                DbConstants.MigrationsHistoryTable,
                DbConstants.MigrationsHistorySchema
            )
    )
);

/// <summary>
/// Registers repositories and services for dependency injection.
/// </summary>
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestDtoValidator>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetUserPermissionsHandler).Assembly);
});

/// <summary>
/// Registers Permission Service for calling Identity Service.
/// </summary>
builder.Services.AddTransient<AuthenticatingDelegatingHandler>();
builder
    .Services.AddHttpClient<IPermissionService, PermissionService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["IdentityService:BaseUrl"]!);
    })
    .AddHttpMessageHandler<AuthenticatingDelegatingHandler>();
builder.Services.AddMemoryCache();

/// <summary>
/// Registers the global exception handler. ASP.NET Core's exception-handling
/// middleware (UseExceptionHandler) invokes TryHandleAsync on this when any
/// unhandled exception occurs further down the pipeline — no manual try/catch needed.
/// </summary>
builder.Services.AddProblemDetails();

/// <summary>
/// /// Configures JWT authentication with token validation parameters.
/// </summary>
var jwtKey =
    builder.Configuration[ConfigKeys.JwtKey]
    ?? throw new InvalidOperationException(ConfigErrorMessages.JwtKeyMissing);
var jwtIssuer =
    builder.Configuration[ConfigKeys.JwtIssuer]
    ?? throw new InvalidOperationException(ConfigErrorMessages.JwtIssuerMissing);
var jwtAudience =
    builder.Configuration[ConfigKeys.JwtAudience]
    ?? throw new InvalidOperationException(ConfigErrorMessages.JwtAudienceMissing);

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
/// Configures CORS policy. Cookies require specific origins with AllowCredentials.
/// In production behind an API Gateway, this should be configured on the gateway instead,
/// and each microservice can drop back to AllowAnyOrigin (or remove CORS entirely).
/// </summary>
builder.Services.AddCors(options =>
{
    var allowedOrigins =
        builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? new[] { "http://localhost:4200" };

    options.AddPolicy(
        CorsPolicies.AllowAll,
        policy => policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
    );
});

/// <summary>
/// Adds authorization services.
/// </summary>
builder.Services.AddAuthorization();

/// <summary>
/// Registers gRPC services for inter-service communication.
/// </summary>
builder.Services.AddGrpc();

/// <summary>
/// Adds controllers.
/// </summary>
builder.Services.AddControllers();
builder.Services.AddGrpc();

/// <summary>
/// Configures Swagger for API documentation with JWT authentication support.
/// </summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        SwaggerConstants.DocName,
        new OpenApiInfo
        {
            Title = SwaggerConstants.Title,
            Version = SwaggerConstants.Version,
            Description = SwaggerConstants.Description,
        }
    );

    c.AddSecurityDefinition(
        SwaggerConstants.SecuritySchemeName,
        new OpenApiSecurityScheme
        {
            Name = SwaggerConstants.AuthHeaderName,
            Type = SecuritySchemeType.Http,
            Scheme = SwaggerConstants.SecurityScheme,
            BearerFormat = SwaggerConstants.BearerFormat,
            In = ParameterLocation.Header,
            Description = SwaggerConstants.AuthDescription,
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
                        Id = SwaggerConstants.SecuritySchemeName,
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

var app = builder.Build();

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     var context = services.GetRequiredService<AppDbContext>();
//     await context.Database.MigrateAsync();

//     // var seeder = services.GetRequiredService<DatabaseSeeder>();
//     // await seeder.SeedAsync();
// }

/// <summary>
/// Activates the registered IExceptionHandler(s) for the request pipeline.
/// Must be one of the first middleware registered so it wraps everything after it.
/// </summary>
app.UseExceptionHandler();
app.UseStaticFiles();

/// <summary>
/// Configures middleware pipeline with Swagger UI for development environment.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(SwaggerConstants.SwaggerJsonEndpoint, SwaggerConstants.SwaggerUiTitle);
    });
}

Log.Information("Identity Service starting up...");

/// <summary>
/// Configures HTTPS redirection, CORS, authentication, authorization, and controller endpoints.
/// </summary>
app.UseGlobalExceptionMiddleware();
app.UseCors(CorsPolicies.AllowAll);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGrpcService<IdentityService.API.Grpc.IdentityGrpcService>();
app.MapGrpcService<FlatLookupGrpcServiceImpl>();

app.Run();

Log.CloseAndFlush();

/// <summary>
/// Partial Program class for integration testing.
/// </summary>
public partial class Program { }

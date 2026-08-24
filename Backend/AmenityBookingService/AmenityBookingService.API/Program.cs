using System.Text;
using AmenityBookingService.API.Converters;
using AmenityBookingService.API.Services;
using AmenityBookingService.Application.Features.Amenities.Queries;
using AmenityBookingService.Application.Interfaces.Repositories;
using AmenityBookingService.Application.Mappings;
using AmenityBookingService.Application.Settings;
using AmenityBookingService.Application.Validators;
using AmenityBookingService.Infrastructure.Extensions;
using AmenityBookingService.Infrastructure.Persistence.DBContext;
using AmenityBookingService.Infrastructure.Persistence.Seeders;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Serilog;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Http;
using Shared.SharedLibrary.Middleware;
using Shared.SharedLibrary.Services;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

/// <summary>
/// Entry point for configuring and running the Amenity Booking Service API.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Configures Serilog for logging to console and file with daily rolling.
/// </summary>
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(LoggingConstants.AmenityLogFilePathTemplate, rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

/// <summary>
/// Configures the database context with PostgreSQL.
/// </summary>
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(ConfigErrorMessages.DefaultConnectionMissing),
        x => x.MigrationsHistoryTable(DbConstants.MigrationsHistoryTable, DbConstants.AmenitySchema)
    )
);

/// <summary>
/// Configures AutoMapper and FluentValidation.
/// </summary>
builder.Services.AddAutoMapper(typeof(AmenityMappingProfile));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAmenityRequestValidator>();

/// <summary>
/// Configures HttpContextAccessor and CurrentUserService.
/// </summary>
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

/// <summary>
/// Registers infrastructure services (repositories, gRPC clients, seeders).
/// </summary>
builder.Services.AddInfrastructure(builder.Configuration);

/// <summary>
/// Registers MediatR.
/// </summary>
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetAmenitiesHandler).Assembly);
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

builder.Services.AddHostedService<BookingBackgroundService>();

builder.Services.Configure<FileStorageSettings>(
    builder.Configuration.GetSection(FileStorageSettings.SectionName)
);

/// <summary>
/// Configures JWT authentication with token validation parameters.
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
/// Configures CORS policy to allow specific origins with credentials.
/// </summary>
builder.Services.AddCors(options =>
{
    var allowedOrigins =
        builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? new[] { "http://localhost:4200" };

    options.AddPolicy(
        CorsPolicies.AllowAll,
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
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new TimeSpanToStringConverter());
    });

/// <summary>
/// Configures Swagger for API documentation with JWT authentication support.
/// </summary>
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        SwaggerConstants.AmenityVersion,
        new OpenApiInfo
        {
            Title = SwaggerConstants.AmenityTitle,
            Version = SwaggerConstants.AmenityVersion,
            Description = SwaggerConstants.AmenityDescription,
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

    c.MapType<TimeSpan>(() =>
        new OpenApiSchema
        {
            Type = OpenApiSchemaConstants.TimeSpanType,
            Format = OpenApiSchemaConstants.TimeSpanFormat,
            Example = new OpenApiString(OpenApiSchemaConstants.TimeSpanExample),
        }
    );
});

var app = builder.Build();

/// <summary>
/// Configures middleware pipeline with Swagger UI for development environment.
/// </summary>
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            SwaggerConstants.SwaggerJsonEndpoint,
            SwaggerConstants.AmenitySwaggerUiTitle
        );
    });
}

Log.Information(StartupLogMessages.AmenityServiceStarting);

/// <summary>
/// Seed data on startup
/// </summary>
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
    await seeder.SeedAllAsync();
}

/// <summary>
/// Global exception middleware to catch unhandled exceptions.
/// Must be early in the pipeline to wrap all downstream middleware.
/// </summary>
app.UseGlobalExceptionMiddleware();

/// <summary>
/// Configures HTTPS redirection, CORS, authentication, authorization, and controller endpoints.
/// </summary>
app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(CorsPolicies.AllowAll);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

Log.CloseAndFlush();

/// <summary>
/// Partial Program class for integration testing.
/// </summary>
public partial class Program { }

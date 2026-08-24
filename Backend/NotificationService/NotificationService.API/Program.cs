using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NotificationService.API.Grpc;
using NotificationService.API.Services;
using NotificationService.Application;
using NotificationService.Application.Common.Interfaces;
using NotificationService.Application.Constants;
using NotificationService.Infrastructure;
using NotificationService.Infrastructure.Hubs;
using NotificationService.Infrastructure.Persistence.DBContext;
using NotificationService.Infrastructure.Persistence.Seeder;
using NotificationService.Infrastructure.Services;
using Serilog;
using Shared.SharedLibrary.Middleware;
using Shared.SharedLibrary.Services;

/// <summary>
/// Entry point for configuring and running the Notification Service API.
/// </summary>
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.ListenLocalhost(
        5266,
        listenOptions =>
        {
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
        }
    );
    options.ListenLocalhost(
        5267,
        listenOptions =>
        {
            listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
        }
    );
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(NotificationConstants.Logging.LOG_FILE_PATH, rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<DbSeeder>(); 

var jwtKey =
    builder.Configuration[NotificationConstants.ConfigKeys.JWT_KEY]
    ?? throw new InvalidOperationException(NotificationConstants.Errors.JWT_KEY_MISSING);

var jwtIssuer =
    builder.Configuration[NotificationConstants.ConfigKeys.JWT_ISSUER]
    ?? throw new InvalidOperationException(NotificationConstants.Errors.JWT_ISSUER_MISSING);

var jwtAudience =
    builder.Configuration[NotificationConstants.ConfigKeys.JWT_AUDIENCE]
    ?? throw new InvalidOperationException(NotificationConstants.Errors.JWT_AUDIENCE_MISSING);

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query[
                    NotificationConstants.SignalR.ACCESS_TOKEN_QUERY_PARAM
                ];
                var path = context.HttpContext.Request.Path;

                if (
                    !string.IsNullOrEmpty(accessToken)
                    && path.StartsWithSegments(NotificationConstants.SignalR.HUB_PATH)
                )
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },
        };

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

var allowedOrigin =
    builder.Configuration[NotificationConstants.ConfigKeys.CORS_ORIGIN]
    ?? NotificationConstants.CorsPolicy.DEFAULT_ORIGIN;

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        NotificationConstants.CorsPolicy.ALLOW_ALL,
        policy =>
            policy.WithOrigins(allowedOrigin).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
    );
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddGrpc();
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationHubService, NotificationHubService>();
builder.Services.AddHostedService<ScheduledNotificationService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        NotificationConstants.Swagger.API_VERSION,
        new OpenApiInfo
        {
            Title = NotificationConstants.Swagger.API_TITLE,
            Version = NotificationConstants.Swagger.API_VERSION,
            Description = NotificationConstants.Swagger.API_DESCRIPTION,
        }
    );

    c.AddSecurityDefinition(
        NotificationConstants.Swagger.BEARER_SCHEME,
        new OpenApiSecurityScheme
        {
            Name = NotificationConstants.Swagger.AUTH_HEADER_NAME,
            Type = SecuritySchemeType.Http,
            Scheme = NotificationConstants.Swagger.BEARER_SCHEME,
            BearerFormat = NotificationConstants.Swagger.BEARER_FORMAT,
            In = ParameterLocation.Header,
            Description = NotificationConstants.Swagger.AUTH_DESCRIPTION,
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
                        Id = NotificationConstants.Swagger.BEARER_SCHEME,
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

app.UseGlobalExceptionMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            $"/swagger/{NotificationConstants.Swagger.API_VERSION}/swagger.json",
            NotificationConstants.Swagger.SWAGGER_UI_TITLE
        );
    });
}

app.UseCors(NotificationConstants.CorsPolicy.ALLOW_ALL);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<NotificationGrpcService>();
app.MapHub<NotificationHub>(NotificationConstants.SignalR.HUB_PATH);

Log.Information(NotificationConstants.Logging.STARTUP_MESSAGE);

app.Run();
Log.CloseAndFlush();

/// <summary>Partial Program class for integration testing.</summary>
public partial class Program { }

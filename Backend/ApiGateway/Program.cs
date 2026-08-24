using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Shared.SharedLibrary.Constants;
using Shared.SharedLibrary.Middleware;

/// <summary>
/// Configures Serilog for logging to console and file with daily rolling.
/// </summary>
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/api-gateway-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("API Gateway starting up...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    /// <summary>
    /// Loads Ocelot route configuration from ocelot.json with hot-reload support.
    /// </summary>
    builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

    /// <summary>
    /// Registers Ocelot services for API Gateway routing.
    /// </summary>
    builder.Services.AddOcelot(builder.Configuration);

    /// <summary>
    /// Configures CORS policy to allow all origins, methods, and headers.
    /// </summary>
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(
            CorsPolicies.AllowAll,
            policy =>
                policy
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
        );
    });

    /// <summary>
    /// Registers health check services.
    /// </summary>
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    /// <summary>
    /// Global exception middleware to catch unhandled exceptions.
    /// Must be early in the pipeline to wrap all downstream middleware.
    /// </summary>
    app.UseGlobalExceptionMiddleware();

    app.UseCors(CorsPolicies.AllowAll);

    /// <summary>
    /// Maps the health check endpoint at /health.
    /// </summary>
    app.MapHealthChecks("/health");

    /// <summary>
    /// Configures the Ocelot middleware to handle request routing.
    /// </summary>
    await app.UseOcelot();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API Gateway terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

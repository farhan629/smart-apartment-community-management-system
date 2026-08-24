using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.API.Grpc;
using ResidentVisitorService.Application.Constants;
using ResidentVisitorService.Application.Features.Visitors.Services;
using ResidentVisitorService.Application.Interfaces.Repositories;
using ResidentVisitorService.Application.Interfaces.Services;
using ResidentVisitorService.Infrastructure.Persistence.DBContext;
using ResidentVisitorService.Infrastructure.Persistence.Repositories;
using ResidentVisitorService.Infrastructure.Persistence.Seeders;
using ResidentVisitorService.Infrastructure.Services;
using Shared.Grpc;

namespace ResidentVisitorService.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering all Infrastructure layer services in the DI container.
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Registers the database context, repositories, seeder, and infrastructure services
    /// for the ResidentVisitor service.
    /// </summary>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString(
                    ResidentVisitorConstants.Database.ConnectionStringName
                ),
                npgsql =>
                    npgsql.MigrationsHistoryTable(
                        ResidentVisitorConstants.Database.MigrationsHistoryTable,
                        ResidentVisitorConstants.Database.SchemaName
                    )
            )
        );

        services.AddScoped<IVisitorRepository, VisitorRepository>();
        services.AddScoped<IVisitRepository, VisitRepository>();
        services.AddScoped<IRefTermRepository, RefTermRepository>();
        services.AddScoped<IVisitQrTokenRepository, VisitQrTokenRepository>();

        services.AddScoped<IFileStorageService, FileStorageService>();
        services.AddScoped<IQrCodeService, QrCodeService>();

        var notificationUrl =
            configuration[ResidentVisitorConstants.NotificationService.UrlConfigKey]
            ?? throw new InvalidOperationException(
                ResidentVisitorConstants.Errors.NotificationUrlMissing
            );

        services
            .AddGrpcClient<NotificationGrpc.NotificationGrpcClient>(o =>
            {
                o.Address = new Uri(notificationUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
                new SocketsHttpHandler { EnableMultipleHttp2Connections = true }
            );

        services.AddScoped<INotificationClient, NotificationClient>();

        var identityUrl =
            configuration[ResidentVisitorConstants.IdentityService.GrpcUrlConfigKey]
            ?? throw new InvalidOperationException(
                ResidentVisitorConstants.Errors.IdentityUrlMissing
            );

        services
            .AddGrpcClient<FlatLookupGrpcService.FlatLookupGrpcServiceClient>(o =>
            {
                o.Address = new Uri(identityUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
                new SocketsHttpHandler { EnableMultipleHttp2Connections = true }
            );

        services.AddScoped<IFlatLookupClient, FlatLookupClient>();

        services.AddTransient<DbSeeder>();

        return services;
    }
}

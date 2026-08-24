using AmenityBookingService.Application.Interfaces.Repositories;
using AmenityBookingService.Application.Interfaces.Services;
using AmenityBookingService.Infrastructure.Persistence.Repositories;
using AmenityBookingService.Infrastructure.Persistence.Seeders;
using AmenityBookingService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.API.Grpc;
using IdentityService.Infrastructure.Protos;

namespace AmenityBookingService.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IAmenityRepository, AmenityRepository>();
        services.AddScoped<IRefTermRepository, RefTermRepository>();
        services.AddScoped<ISlotRepository, SlotRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();

        services.AddScoped<ISeeder, ReferenceDataSeeder>();
        services.AddScoped<Seeder>();

        services.AddGrpcClient<NotificationGrpc.NotificationGrpcClient>(options =>
        {
            options.Address = new Uri(configuration["NotificationService:GrpcUrl"]!);
        });

        services.AddGrpcClient<IdentityGrpc.IdentityGrpcClient>(options =>
        {
            options.Address = new Uri(configuration["IdentityService:GrpcUrl"]!);
        });

        services.AddScoped<INotificationClient, NotificationGrpcClient>();
        services.AddScoped<IIdentityClient, IdentityGrpcClient>();

        return services;
    }
}

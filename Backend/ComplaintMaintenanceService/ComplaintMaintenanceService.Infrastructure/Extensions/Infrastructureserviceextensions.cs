using ComplaintMaintenanceService.Application.Interfaces.Repositories;
using ComplaintMaintenanceService.Application.Interfaces.Services;
using ComplaintMaintenanceService.Infrastructure.Persistence.Repositories;
using ComplaintMaintenanceService.Infrastructure.Persistence.Seeders;
using ComplaintMaintenanceService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ComplaintMaintenanceService.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for registering infrastructure services in the DI container.
    /// </summary>
    public static class InfrastructureServiceExtensions
    {
        /// <summary>
        /// Registers repositories, services, and seeding utilities to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add dependencies to.</param>
        /// <param name="configuration">The application configuration instance.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddScoped<IRefSetRepository, RefSetRepository>();
            services.AddScoped<IRefTermRepository, RefTermRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IComplaintRepository, ComplaintRepository>();
            services.AddScoped<IComplaintProgressLogRepository, ComplaintProgressLogRepository>();
            services.AddScoped<IStaffAvailabilityRepository, StaffAvailabilityRepository>();
            services.AddScoped<IComplaintAssignmentRepository, ComplaintAssignmentRepository>();
            services.AddScoped<IComplaintCommentRepository, ComplaintCommentRepository>();
            services.AddScoped<IComplaintEscalationRepository, ComplaintEscalationRepository>();
            // Staff repository — used by gRPC server and REST handlers
            services.AddScoped<IStaffRepository, StaffRepository>();
            // Add more repositories here as you build them out:
            // services.AddScoped<IStaffRepository, StaffRepository>();
            // services.AddScoped<IComplaintAssignmentRepository, ComplaintAssignmentRepository>();
            // services.AddScoped<IComplaintCommentRepository, ComplaintCommentRepository>();
            // services.AddScoped<IComplaintEscalationRepository, ComplaintEscalationRepository>();

            services.AddScoped<DatabaseSeeder>();
            services.AddScoped<IIdentityGrpcClient, IdentityGrpcClient>();
            services.AddScoped<INotificationGrpcClient, NotificationGrpcClient>();
            services.AddScoped<IFlatLookupGrpcClient, FlatLookupGrpcClient>();

            return services;
        }
    }
}

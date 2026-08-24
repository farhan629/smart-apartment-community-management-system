using IdentityService.Application.Interfaces.Repositories;
using IdentityService.Application.Interfaces.Services;
using IdentityService.Infrastructure.Persistence.Repositories;
using IdentityService.Infrastructure.Repositories;
using IdentityService.Infrastructure.Seeders;
using IdentityService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.SharedLibrary.Services;

namespace IdentityService.Infrastructure.Extensions
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
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefSetRepository, RefSetRepository>();
            services.AddScoped<IRefTermRepository, RefTermRepository>();
            services.AddScoped<IFlatRepository, FlatRepository>();
            services.AddScoped<IFlatOccupancyRepository, FlatOccupancyRepository>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IGrpcStaffClient, GrpcStaffClient>();
            services.AddScoped<DatabaseSeeder>();

            services.AddSingleton<IOtpService, OtpService>();
            services.AddSingleton<IOtpCacheService, OtpCacheService>();
            services.AddTransient<ISmsService, SmsService>();

            services.Configure<TwilioSettings>(configuration.GetSection("TwilioSettings"));

            return services;
        }
    }
}

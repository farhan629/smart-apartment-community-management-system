using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ResidentVisitorService.Application.Common.Behaviours;
using ResidentVisitorService.Application.Common.Mappings;

namespace ResidentVisitorService.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(MappingProfile).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        });

        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        services.AddValidatorsFromAssembly(typeof(MappingProfile).Assembly);

        return services;
    }
}

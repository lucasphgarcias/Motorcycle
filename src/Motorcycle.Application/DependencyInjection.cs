using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Motorcycle.Application.Behaviors;
using Motorcycle.Application.Interfaces;
using Motorcycle.Application.Mappings;
using Motorcycle.Application.Services;

namespace Motorcycle.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registrar AutoMapper
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        
        // Registrar FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Registrar MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        
        // Registrar serviços de aplicação
        services.AddScoped<IMotorcycleService, MotorcycleService>();
        services.AddScoped<IDeliveryPersonService, DeliveryPersonService>();
        services.AddScoped<IRentalService, RentalService>();
        
        return services;
    }
}
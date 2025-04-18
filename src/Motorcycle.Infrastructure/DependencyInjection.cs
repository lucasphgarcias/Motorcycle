using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Motorcycle.Application.Interfaces;
using Motorcycle.Application.Services;
using Motorcycle.Infrastructure.Auth.Configuration;
using Motorcycle.Infrastructure.Auth.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Motorcycle.Domain.Interfaces.Repositories;
using Motorcycle.Domain.Interfaces.Services;
using Motorcycle.Infrastructure.Data.Context;
using Motorcycle.Infrastructure.Data.Repositories;
using Motorcycle.Infrastructure.Messaging.Configuration;
using Motorcycle.Infrastructure.Messaging.Consumers;
using Motorcycle.Infrastructure.Messaging.Publishers;
using Motorcycle.Infrastructure.Storage.Configuration;
using Motorcycle.Infrastructure.Storage.Services;

namespace Motorcycle.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar banco de dados
        services.AddDbContext<MotorcycleDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(MotorcycleDbContext).Assembly.FullName)
            )
        );

        // Registrar repositórios
        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IDeliveryPersonRepository, DeliveryPersonRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();

        // Configurar MinIO
        services.Configure<MinioSettings>(configuration.GetSection("MinIO"));
        services.AddSingleton<IFileStorageService, MinioStorageService>();

        // Configurar RabbitMQ
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMQ"));
        services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
        services.AddHostedService<Motorcycle2024Consumer>();

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        

        return services;
    }
}
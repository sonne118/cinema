using Infrastructure.Persistence.Interceptors;
using Application.Common.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Persistence.Repositories;
using Application.Common.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace BuberDinner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddPersistance();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    public static IServiceCollection AddPersistance(
        this IServiceCollection services)
    {
        services.AddDbContext<DbContext>(options =>
            options.UseSqlServer("Server=localhost;Database=BuberDinner;User Id=SA;Password=amiko123!;TrustServerCertificate=true"));

        services.AddScoped<PublishDomainEventsInterceptor>();
        services.AddScoped<IShowTimeRepository, ShowTimeRepository>();      
        
        return services;
    }
}
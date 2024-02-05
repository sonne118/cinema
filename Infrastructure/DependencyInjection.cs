using ApiApplication.Common.Interfaces.Persistence;
using ApiApplication.Infrastructure.Persistence;
using ApiApplication.Infrastructure.Persistence.Interceptors;
using ApiApplication.Infrastructure.Persistence.Repositories;
using Application.Common.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiApplication.Infrastructure;

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
        services.AddDbContext<DbContextApi>(options =>
            options.UseSqlServer("Server=localhost,1433;Database=Movies;User Id=SA;Password=Pass12345;TrustServerCertificate=true"));

        services.AddScoped<PublishDomainEventsInterceptor>();
        services.AddScoped<IShowTimeRepository, ShowTimeRepository>();      
        
        return services;
    }
}
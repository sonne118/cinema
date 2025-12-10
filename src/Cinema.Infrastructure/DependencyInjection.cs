using Cinema.Application.Common.Interfaces.Messaging;
using Cinema.Application.Common.Interfaces.Persistence;
using Cinema.Application.Common.Interfaces.Queries;
using Cinema.Application.Common.Interfaces.Services;
using Cinema.Infrastructure.BackgroundJobs;
using Cinema.Infrastructure.Messaging;
using Cinema.Infrastructure.Persistence.Read;
using Cinema.Infrastructure.Persistence.Read.Repositories;
using Cinema.Infrastructure.Persistence.Write;
using Cinema.Infrastructure.Persistence.Write.Interceptors;
using Cinema.Infrastructure.Services;
using Cinema.Infrastructure.Persistence.Write.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Cinema.Infrastructure;




public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isWriteSide = true)
    {
        services
            .AddPersistence(configuration)
            .AddMessaging(configuration);

        if (isWriteSide)
        {
            services.AddBackgroundJobs();
        }

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        
        services.AddSingleton<PublishDomainEventsInterceptor>();

        services.AddDbContext<CinemaDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<PublishDomainEventsInterceptor>();
            
            options.UseSqlServer(
                configuration.GetConnectionString("SqlServer"),
                b => b.MigrationsAssembly(typeof(CinemaDbContext).Assembly.FullName))
                .AddInterceptors(interceptor);
        });

        
        services.AddScoped<IShowtimeRepository, ShowtimeRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        
        services.Configure<MongoDbSettings>(
            configuration.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<MongoDbContext>();

        
        services.AddScoped<IShowtimeReadRepository, ShowtimeReadRepository>();
        services.AddScoped<IReservationReadRepository, ReservationReadRepository>();

        
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        
        services.AddScoped<IMovieApiService, MovieApiService>();

        return services;
    }

    private static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        
        services.Configure<KafkaSettings>(
            configuration.GetSection(nameof(KafkaSettings)));

        services.AddSingleton<IEventBus, KafkaEventBus>();
        

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(
        this IServiceCollection services)
    {
        
        services.AddQuartz(configure =>
        {
            var processOutboxJobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

            configure
                .AddJob<ProcessOutboxMessagesJob>(processOutboxJobKey)
                .AddTrigger(trigger =>
                    trigger.ForJob(processOutboxJobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInSeconds(10)
                                .RepeatForever()));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }
}




public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

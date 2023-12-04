using ApiApplication.Domain.Common.Models;
using ApiApplication.Domain.ShowTimeAggregate;
using ApiApplication.Domain.TicketAggregate;
using Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace ApiApplication.Infrastructure.Persistence;

public class DbContextApi : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly PublishDomainEventsInterceptor _publishDomainEventsInterceptor;

    public DbContextApi(DbContextOptions<DbContextApi> options, PublishDomainEventsInterceptor publishDomainEventsInterceptor)
        : base(options)
    {
        _publishDomainEventsInterceptor = publishDomainEventsInterceptor;
    }

    public DbSet<ShowTime> ShowTimes { get; set; } = null!;
    public DbSet<Reservation> Tickets { get; set; } = null!;
   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Ignore<List<IDomainEvent>>()
            .ApplyConfigurationsFromAssembly(typeof(DbContextApi).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_publishDomainEventsInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}
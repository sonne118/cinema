using Cinema.Domain.ReservationAggregate;
using Cinema.Domain.ShowtimeAggregate;
using Cinema.Infrastructure.Persistence.Write.Interceptors;
using Cinema.Infrastructure.Persistence.Write.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Infrastructure.Persistence.Write;





public class CinemaDbContext : DbContext
{
    

    public DbSet<Showtime> Showtimes => Set<Showtime>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    public CinemaDbContext(
        DbContextOptions<CinemaDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CinemaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        base.OnConfiguring(optionsBuilder);
    }
}

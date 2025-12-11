using Cinema.MasterNode.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema.MasterNode.Infrastructure.Data;

public class MasterDbContext : DbContext
{
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationSeat> ReservationSeats { get; set; }
    public DbSet<Showtime> Showtimes { get; set; }

    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservations");
            entity.HasKey(e => new { e.Id, e.CreatedAt }); 
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10,2)");
            
            entity.HasIndex(e => e.ShowtimeId);
            entity.HasIndex(e => e.CustomerId);
        });

        modelBuilder.Entity<ReservationSeat>(entity =>
        {
            entity.ToTable("ReservationSeats");
            entity.HasKey(e => new { e.Id, e.CreatedAt });
            
            entity.HasOne<Reservation>()
                .WithMany(r => r.Seats)
                .HasForeignKey(e => new { e.ReservationId, e.CreatedAt });
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.ToTable("Showtimes");
            entity.HasKey(e => new { e.Id, e.CreatedAt });
            entity.Property(e => e.MovieImdbId).HasMaxLength(50);
            
            entity.HasIndex(e => e.AuditoriumId);
            entity.HasIndex(e => e.ScreeningTime);
        });
    }
}

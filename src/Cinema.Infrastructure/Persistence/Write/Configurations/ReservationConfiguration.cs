using Cinema.Domain.ReservationAggregate;
using Cinema.Domain.ReservationAggregate.ValueObjects;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Write.Configurations;




public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        ConfigureReservationsTable(builder);
        ConfigureSeats(builder);
        ConfigureStatus(builder);
    }

    private void ConfigureReservationsTable(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasConversion(
                id => id.Value,
                value => ReservationId.Create(value))
            .ValueGeneratedNever();

        builder.Property(r => r.ShowtimeId)
            .HasConversion(
                id => id.Value,
                value => ShowtimeId.Create(value))
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.ExpiresAt)
            .IsRequired();

        builder.Property(r => r.ConfirmedAt);

        
        builder.Ignore(r => r.DomainEvents);
    }

    private void ConfigureSeats(EntityTypeBuilder<Reservation> builder)
    {
        builder.OwnsMany(r => r.Seats, sb =>
        {
            sb.ToTable("ReservationSeats");

            sb.WithOwner().HasForeignKey("ReservationId");

            sb.Property<Guid>("Id").ValueGeneratedOnAdd();
            sb.HasKey("Id");

            sb.Property(s => s.Row)
                .HasColumnName("Row")
                .IsRequired();

            sb.Property(s => s.Number)
                .HasColumnName("SeatNumber")
                .IsRequired();
        });
    }

    private void ConfigureStatus(EntityTypeBuilder<Reservation> builder)
    {
        builder.OwnsOne(r => r.Status, sb =>
        {
            sb.Property(s => s.Value)
                .HasColumnName("Status")
                .HasConversion<string>()
                .IsRequired();
        });
    }
}

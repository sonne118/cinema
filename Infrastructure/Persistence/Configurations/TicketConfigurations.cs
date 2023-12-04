using ApiApplication.Domain.TicketAggregate;
using BuberDinner.Domain.DinnerAggregate.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuberDinner.Infrastructure.Persistence.Configurations;

public class TicketConfigurations : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        //ConfigureTicketEntitiesTable(builder);
        //ConfigureReservationsEntityTable(builder);
        ConfigureTicketsAggregateTable(builder);
    }

    //private static void ConfigureTicketEntitiesTable(EntityTypeBuilder<Ticket> builder)
    //{
    //    builder.OwnsMany(d => d.TicketEntities, rb =>
    //    {
    //        rb.ToTable("Ticket_TicketEntities");

    //        rb.WithOwner().HasForeignKey("TicketId");

    //        rb.HasKey("TicketId", "Id");                        

    //        rb.Property(r => r.Id)
    //        .HasColumnName("TicketId")
    //        .ValueGeneratedNever()
    //        .HasConversion(
    //            id => id.Value,
    //            value => TicketId.Create(value));

    //        rb.Property(r => r.TicketStatus)
    //            .HasConversion(
    //                status => status.Value,
    //                value => TicketStatus.FromValue(value));

    //        rb.Property(r => r.CreatedTime)
    //           .HasMaxLength(100);
               

    //        rb.Property(d => d.Paid)
    //            .HasMaxLength(100);

    //        rb.Property(d => d.Row)
    //           .HasMaxLength(100);

    //        rb.Property(d => d.SeatNumber)
    //            .HasMaxLength(100);

    //        rb.Property(d => d.AuditoriumId)
    //           .HasMaxLength(100);

    //    });

    //}

    //private static void ConfigureReservationsEntityTable(EntityTypeBuilder<Ticket> builder)
    //{
    //    builder.OwnsMany(d => d.Reservations, rb =>
    //    {
    //        rb.ToTable("Ticket_ReservationEntities");

    //        rb.WithOwner().HasForeignKey("Ticket");

    //        rb.HasKey("Ticket", "Id");

    //        rb.Property(r => r.Id)
    //            .HasColumnName("ReservationId")
    //            .ValueGeneratedNever()
    //            .HasConversion(
    //                id => id.Value,
    //                value => ReservationId.Create(value));

    //        //rb.Property(r => r.GuestId)
    //        //    .HasConversion(
    //        //        id => id.Value,
    //        //        value => GuestId.Create(value));



    //        rb.Property(r => r.Status)
    //            .HasConversion(
    //                status => status.Value,
    //                value => ReservationStatus.FromValue(value));
    //    });

    //    builder.Metadata.FindNavigation(nameof(Ticket.Reservations))!
    //        .SetPropertyAccessMode(PropertyAccessMode.Field);
    //}

    private static void ConfigureTicketsAggregateTable(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("TicketsAggregate");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => ReservationId.Create(value));


        //builder.Property(d => d.CreatedTime)
        //    .HasMaxLength(100);

        //builder.Property(d => d.ShowtimeId)
        //    .HasMaxLength(100);


        //builder.Property(d => d.TicketStatus)
        //    .HasConversion(
        //        status => status.Value,
        //        value => TicketStatus.FromValue(value));       


        //builder.Property(d => d.MenuId)
        //    .HasConversion(
        //        id => id.Value,
        //        value => MenuId.Create(value));

        //builder.OwnsOne(d => d.Location);
    }
}
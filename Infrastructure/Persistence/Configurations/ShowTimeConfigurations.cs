using ApiApplication.Domain.ShowTimeAggregate;
using ApiApplication.Domain.TicketAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiApplication.Persistence.Configuration
{

    public class ShowTimeConfigurations : IEntityTypeConfiguration<ShowTime>
    {
        public void Configure(EntityTypeBuilder<ShowTime> builder)
        {
            ConfigureTicketTable(builder);
            ConfigureShowTimeTable(builder);
        }

        private static void ConfigureTicketTable(EntityTypeBuilder<ShowTime> builder)
        {
            builder.OwnsMany(m => m.TicketEntities, dib =>
            {
                dib.ToTable("ShowTimeTicketIds");

                dib.WithOwner().HasForeignKey("ShowTimeId");

                dib.HasKey("Id");

                dib.Property(s => s.Id)
                .HasColumnName("MenuTicketId")
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => TicketId.Create(value));

                dib.Property(t => t.CreatedTime)
                 .HasMaxLength(100);

                dib.Property(p => p.Paid)
                 .HasMaxLength(100);

                builder.Property(s => s.ShowTimeId)
                 .HasMaxLength(100);

                dib.OwnsMany(s => s.SeatEntities, ib =>
                {
                    ib.ToTable("ShowSeat");

                    ib.WithOwner().HasForeignKey("ShowTimeSeatEntityId", "");

                    ib.HasKey(nameof(SeatEntity.Id), "ShowTimeSeatEntityId", "ShowTimeId");

                    ib.Property(i => i.Id)
                        .HasColumnName("ShowTimeSeatEntityId")
                        .ValueGeneratedNever()
                        .HasConversion(
                            id => id.Value,
                            value => SeatId.Create(value));

                    ib.Property(s => s.Row)
                      .HasMaxLength(100);

                    ib.Property(s => s.SeatNumber)
                      .HasMaxLength(100);


                    ib.Property(s => s.SeatNumber)
                     .HasMaxLength(100);

                    ib.Property(s => s.AuditoriumId)
                     .HasMaxLength(100);

                });
                dib.Navigation(s => s.SeatEntities).Metadata.SetField("_seats");
                dib.Navigation(s => s.SeatEntities).UsePropertyAccessMode(PropertyAccessMode.Field);

            });

            builder.Metadata.FindNavigation(nameof(ShowTime.TicketEntities))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }

        private static void ConfigureShowTimeTable(EntityTypeBuilder<ShowTime> builder)
        {
            builder.ToTable("ShowTime");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => ShowTimeId.Create(value));

            builder.Property(m => m.CreatedDateTime)
               .HasMaxLength(100);

            builder.Property(m => m.UpdatedDateTime)
               .HasMaxLength(100);


            builder.Property(m => m.SessionDate)
               .HasMaxLength(100);

            builder.Property(h => h.AuditoriumId)
                .HasConversion(
                    id => id.Value,
                    value => AuditoriumId.Create(value));

            builder.Property(h => h.MovieId)
                .HasConversion(
                    id => id.Value,
                    value => MovieId.Create(value));
        }
    }

}
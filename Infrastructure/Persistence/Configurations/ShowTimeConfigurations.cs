using ApiApplication.Domain.Domain.ShowTimeAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiApplication.Infrastructure.Persistence.Configurations
{
   // public class SeatConfigurations : IEntityTypeConfiguration<SeatEntity>
    //{
       // public void Configure(EntityTypeBuilder<SeatEntity> builder)
      //  {

            // builder.HasOne(s => s.Auditorium);
            // builder.has

            //builder.OwnsOne(a => a.Auditorium, s=>
        //    {

              //  s.ToTable("SeatEntityAuditorium");
                //s.WithOwner().HasForeignKey("AuditoriumId");
               // s.HasKey("Id", "AuditoriumId");

                //s.Property(h => h.a)
               //  .HasConversion(
               // id => id.Value,
               //value => AuditoriumId.Create(value));


         //   }//;);
                                           
                                           
                                           
                
                
                //(s => s.SeatEntities, ib =>
              //{
              //    ib.ToTable("ShowSeat");

              //    ib.WithOwner().HasForeignKey("ShowTimeSeatEntityId", "AuditoriumId");

              //    ib.HasKey(nameof(SeatEntity.Id), "ShowTimeSeatEntityId", "AuditoriumId");

              //    ib.Property(i => i.Id)
              //        .HasColumnName("ShowTimeSeatEntityId")
              //        .ValueGeneratedNever()
              //        .HasConversion(
              //            id => id.Value,
              //            value => SeatId.Create(value));

              //    ib.Property(s => s.Row)
              //                      .HasMaxLength(100);

              //    ib.Property(s => s.SeatNumber)
              //                      .HasMaxLength(100);


              //    ib.Property(s => s.SeatNumber)
              //                     .HasMaxLength(100);

              //    //ib.Property(s => s.AuditoriumId)
              //    //.HasMaxLength(100);
              //    ib.Property(h => h.AuditoriumId)
              //                      .HasConversion(
              //                     id => id.Value,
              //                    value => AuditoriumId.Create(value));
              //});     
                
         //}


    public class AuditoriumConfigurations : IEntityTypeConfiguration<Auditorium>
    {
        public void Configure(EntityTypeBuilder<Auditorium> builder)
        {

            //builder.HasKey("Id");

            builder.Property(h => h.Id)
                  .HasConversion(
                 id => id.Value,
                value => AuditoriumId.Create(value));


            builder.OwnsMany(s => s.Seats, ib =>
            {
                ib.ToTable("AuditoriumSeats");

                ib.WithOwner().HasForeignKey("AuditoriumId");

                //ib.Property<Guid>("Id"); 
                //builder.HasKey(user => user.Id);

                 ib.HasKey("Id", "AuditoriumId");
                //ib.HasKey(nameof(SeatEntity.Id), "Auditorium", "AuditoriumId");

                ib.Property(i => i.Id)
                    .HasColumnName("AuditoriumSeatId")
                   .ValueGeneratedNever()
                   .HasConversion(
                       id => id.Value,
                       value => SeatId.Create(value));

                ib.Property(p => p.Row)
                 .HasMaxLength(100);

                ib.Property(s => s.SeatNumber)
                  .HasMaxLength(100);

                //ib.Property(s => s.AuditoriumId)
                //    .HasMaxLength(100);
               
                ib.Property(h => h.AuditoriumId)
                  .HasConversion(
                 id => id.Value,
                value => AuditoriumId.Create(value));

            });

            //builder.OwnsMany(m => m.Items, sb =>
            //{
            //    sb.ToTable("AuditoriumSeats");
            //    sb.HasKey(s => s.Id);
            //    sb.WithOwner().HasForeignKey("AuditoriumId");

            //    sb.Property(s => s.Id)
            //    .HasColumnName("AuditoriumSeatId")
            //    .ValueGeneratedNever()
            //    .HasConversion(
            //        id => id.Value,
            //        value => SeatId.Create(value));

            //    //sb.HasKey("Id", "ShowTimeId");

            //});
        }
    }
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

                dib.HasKey("Id", "ShowTimeId");

                dib.Property(m => m.ShowTimeId)
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => ShowTimeId.Create(value));

                dib.Property(s => s.Id)
                .HasColumnName("ShowTimeTicketId")
                .ValueGeneratedNever()
                .HasConversion(
                    id => id.Value,
                    value => TicketId.Create(value));

                dib.Property(t => t.CreatedTime)
                 .HasMaxLength(100);

                dib.Property(p => p.Paid)
                 .HasMaxLength(100);

                dib.Property(s => s.ShowTimeId)
                  .HasMaxLength(100);

                dib.OwnsMany(s => s.SeatEntities, ib =>
                {
                    ib.ToTable("ShowSeat");

                    ib.WithOwner().HasForeignKey("ShowTimeSeatEntityId", "ShowTimeId");

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

                     //ib.Property<Guid>("Id");
                    //ib.Property(s => s.AuditoriumId)
                    //.HasMaxLength(100);
                    ib.Property(h => h.AuditoriumId)
                      .HasConversion(
                     id => id.Value,
                    value => AuditoriumId.Create(value));

                    //});
                   // dib.Navigation(s => s.SeatEntities).Metadata.SetField("_seats");
                    //dib.Navigation(s => s.SeatEntities).UsePropertyAccessMode(PropertyAccessMode.Field);

                });

                builder.Metadata.FindNavigation(nameof(ShowTime.TicketEntities))!
                    .SetPropertyAccessMode(PropertyAccessMode.Field);
            });
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

            //builder.Property(m => m.CreatedDateTime)
            //   .HasMaxLength(100);

            //builder.Property(m => m.UpdatedDateTime)
            //   .HasMaxLength(100);

            //builder.Property(m => m.TicketEntities);

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
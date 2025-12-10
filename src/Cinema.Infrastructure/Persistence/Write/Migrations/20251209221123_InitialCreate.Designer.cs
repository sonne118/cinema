
using System;
using Cinema.Infrastructure.Persistence.Write;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Cinema.Infrastructure.Persistence.Write.Migrations
{
    [DbContext(typeof(CinemaDbContext))]
    [Migration("20251209221123_InitialCreate")]
    partial class InitialCreate
    {
        
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Cinema.Domain.ReservationAggregate.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("ConfirmedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("ShowtimeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Reservations", (string)null);
                });

            modelBuilder.Entity("Cinema.Domain.ShowtimeAggregate.Showtime", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuditoriumId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CancelledAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Showtimes", (string)null);
                });

            modelBuilder.Entity("Cinema.Infrastructure.Persistence.Write.Outbox.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Error")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OccurredOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ProcessedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("ProcessedOnUtc");

                    b.ToTable("OutboxMessages", (string)null);
                });

            modelBuilder.Entity("Cinema.Domain.ReservationAggregate.Reservation", b =>
                {
                    b.OwnsOne("Cinema.Domain.ReservationAggregate.ValueObjects.ReservationStatus", "Status", b1 =>
                        {
                            b1.Property<Guid>("ReservationId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("Status");

                            b1.HasKey("ReservationId");

                            b1.ToTable("Reservations");

                            b1.WithOwner()
                                .HasForeignKey("ReservationId");
                        });

                    b.OwnsMany("Cinema.Domain.ReservationAggregate.ValueObjects.SeatNumber", "Seats", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Number")
                                .HasColumnType("int")
                                .HasColumnName("SeatNumber");

                            b1.Property<Guid>("ReservationId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("Row")
                                .HasColumnType("int")
                                .HasColumnName("Row");

                            b1.HasKey("Id");

                            b1.HasIndex("ReservationId");

                            b1.ToTable("ReservationSeats", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("ReservationId");
                        });

                    b.Navigation("Seats");

                    b.Navigation("Status")
                        .IsRequired();
                });

            modelBuilder.Entity("Cinema.Domain.ShowtimeAggregate.Showtime", b =>
                {
                    b.OwnsOne("Cinema.Domain.ShowtimeAggregate.ValueObjects.MovieDetails", "MovieDetails", b1 =>
                        {
                            b1.Property<Guid>("ShowtimeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("ImdbId")
                                .IsRequired()
                                .HasMaxLength(20)
                                .HasColumnType("nvarchar(20)")
                                .HasColumnName("MovieImdbId");

                            b1.Property<string>("PosterUrl")
                                .HasMaxLength(500)
                                .HasColumnType("nvarchar(500)")
                                .HasColumnName("MoviePosterUrl");

                            b1.Property<int?>("ReleaseYear")
                                .HasColumnType("int")
                                .HasColumnName("MovieReleaseYear");

                            b1.Property<string>("Title")
                                .IsRequired()
                                .HasMaxLength(200)
                                .HasColumnType("nvarchar(200)")
                                .HasColumnName("MovieTitle");

                            b1.HasKey("ShowtimeId");

                            b1.ToTable("Showtimes");

                            b1.WithOwner()
                                .HasForeignKey("ShowtimeId");
                        });

                    b.OwnsOne("Cinema.Domain.ShowtimeAggregate.ValueObjects.ScreeningTime", "ScreeningTime", b1 =>
                        {
                            b1.Property<Guid>("ShowtimeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<DateTime>("Value")
                                .HasColumnType("datetime2")
                                .HasColumnName("ScreeningTime");

                            b1.HasKey("ShowtimeId");

                            b1.ToTable("Showtimes");

                            b1.WithOwner()
                                .HasForeignKey("ShowtimeId");
                        });

                    b.Navigation("MovieDetails")
                        .IsRequired();

                    b.Navigation("ScreeningTime")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

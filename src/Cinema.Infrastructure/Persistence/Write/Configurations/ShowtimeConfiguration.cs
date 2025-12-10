using Cinema.Domain.ShowtimeAggregate;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema.Infrastructure.Persistence.Write.Configurations;




public class ShowtimeConfiguration : IEntityTypeConfiguration<Showtime>
{
    public void Configure(EntityTypeBuilder<Showtime> builder)
    {
        ConfigureShowtimesTable(builder);
        ConfigureMovieDetails(builder);
        ConfigureScreeningTime(builder);
    }

    private void ConfigureShowtimesTable(EntityTypeBuilder<Showtime> builder)
    {
        builder.ToTable("Showtimes");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => ShowtimeId.Create(value))
            .ValueGeneratedNever();

        builder.Property(s => s.AuditoriumId)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.CancelledAt);

        
        builder.Ignore(s => s.DomainEvents);
    }

    private void ConfigureMovieDetails(EntityTypeBuilder<Showtime> builder)
    {
        builder.OwnsOne(s => s.MovieDetails, md =>
        {
            md.Property(m => m.ImdbId)
                .HasColumnName("MovieImdbId")
                .HasMaxLength(20)
                .IsRequired();

            md.Property(m => m.Title)
                .HasColumnName("MovieTitle")
                .HasMaxLength(200)
                .IsRequired();

            md.Property(m => m.PosterUrl)
                .HasColumnName("MoviePosterUrl")
                .HasMaxLength(500);

            md.Property(m => m.ReleaseYear)
                .HasColumnName("MovieReleaseYear");
        });
    }

    private void ConfigureScreeningTime(EntityTypeBuilder<Showtime> builder)
    {
        builder.OwnsOne(s => s.ScreeningTime, st =>
        {
            st.Property(t => t.Value)
                .HasColumnName("ScreeningTime")
                .IsRequired();
        });
    }
}

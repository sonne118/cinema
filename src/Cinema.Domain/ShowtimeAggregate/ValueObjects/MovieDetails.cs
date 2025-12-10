using Cinema.Domain.Common.Errors;
using Cinema.Domain.Common.Models;

namespace Cinema.Domain.ShowtimeAggregate.ValueObjects;




public sealed class MovieDetails : ValueObject
{
    public string ImdbId { get; private set; }
    public string Title { get; private set; }
    public string? PosterUrl { get; private set; }
    public int? ReleaseYear { get; private set; }

    private MovieDetails(string imdbId, string title, string? posterUrl, int? releaseYear)
    {
        ImdbId = imdbId;
        Title = title;
        PosterUrl = posterUrl;
        ReleaseYear = releaseYear;
    }

    public static MovieDetails Create(
        string imdbId,
        string title,
        string? posterUrl = null,
        int? releaseYear = null)
    {
        if (string.IsNullOrWhiteSpace(imdbId))
            throw new ArgumentException("IMDb ID cannot be empty", nameof(imdbId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (releaseYear.HasValue && (releaseYear < 1800 || releaseYear > DateTime.UtcNow.Year + 5))
            throw new ArgumentException("Release year is invalid", nameof(releaseYear));

        return new MovieDetails(imdbId, title, posterUrl, releaseYear);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return ImdbId;
        yield return Title;
        yield return PosterUrl ?? string.Empty;
        yield return ReleaseYear ?? 0;
    }

#pragma warning disable CS8618
    private MovieDetails() { }
#pragma warning restore CS8618
}

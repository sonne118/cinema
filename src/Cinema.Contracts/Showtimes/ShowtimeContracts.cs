namespace Cinema.Contracts.Showtimes;

public record CreateShowtimeRequest(
    string MovieImdbId,
    DateTime ScreeningTime,
    Guid AuditoriumId);

public record ShowtimeResponse(
    Guid Id,
    MovieDetailsResponse Movie,
    DateTime ScreeningTime,
    Guid AuditoriumId,
    string Status,
    DateTime CreatedAt);

public record MovieDetailsResponse(
    string ImdbId,
    string Title,
    string? PosterUrl,
    int? ReleaseYear);

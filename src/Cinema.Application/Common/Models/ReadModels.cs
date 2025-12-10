namespace Cinema.Application.Common.Models;




public class ShowtimeReadModel
{
    public Guid Id { get; set; }
    public string MovieImdbId { get; set; } = string.Empty;
    public string MovieTitle { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
    public int? ReleaseYear { get; set; }
    public DateTime ScreeningTime { get; set; }
    public Guid AuditoriumId { get; set; }
    public string AuditoriumName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<Guid> ReservedSeats { get; set; } = new();
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
}




public class ReservationReadModel
{
    public Guid Id { get; set; }
    public Guid ShowtimeId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime ScreeningTime { get; set; }
    public List<SeatReadModel> Seats { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
}




public class SeatReadModel
{
    public int Row { get; set; }
    public int Number { get; set; }
}

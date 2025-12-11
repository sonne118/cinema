namespace Cinema.MasterNode.Domain.Models;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid ShowtimeId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public List<ReservationSeat> Seats { get; set; } = new();
}

public class ReservationSeat
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public int SeatRow { get; set; }
    public int SeatNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Showtime
{
    public Guid Id { get; set; }
    public string MovieImdbId { get; set; } = string.Empty;
    public DateTime ScreeningTime { get; set; }
    public Guid AuditoriumId { get; set; }
    public DateTime CreatedAt { get; set; }
}

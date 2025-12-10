using Cinema.Domain.Common.Models;
using Cinema.Domain.ReservationAggregate.Events;
using Cinema.Domain.ReservationAggregate.ValueObjects;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;

namespace Cinema.Domain.ReservationAggregate;





public sealed class Reservation : AggregateRoot<ReservationId>
{
    private const int ReservationExpirationMinutes = 10;

    private readonly List<SeatNumber> _seats = new();

    public ShowtimeId ShowtimeId { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }

    public IReadOnlyList<SeatNumber> Seats => _seats.AsReadOnly();

    private Reservation(
        ReservationId id,
        ShowtimeId showtimeId,
        List<SeatNumber> seats,
        DateTime createdAt) : base(id)
    {
        ShowtimeId = showtimeId;
        _seats = seats;
        Status = ReservationStatus.Pending;
        CreatedAt = createdAt;
        ExpiresAt = createdAt.AddMinutes(ReservationExpirationMinutes);
    }

    
    
    
    public static Reservation Create(
        ShowtimeId showtimeId,
        List<SeatNumber> seats,
        DateTime createdAt)
    {
        ValidateSeats(seats);

        var reservationId = ReservationId.CreateUnique();
        var reservation = new Reservation(
            reservationId,
            showtimeId,
            seats,
            createdAt);

        reservation.RaiseDomainEvent(new ReservationCreatedEvent(
            reservationId,
            showtimeId,
            seats,
            reservation.ExpiresAt));

        return reservation;
    }

    
    
    
    public void Confirm()
    {
        if (!Status.IsPending)
            throw new InvalidOperationException($"Cannot confirm reservation with status {Status.Value}");

        if (HasExpired())
            throw new InvalidOperationException("Cannot confirm expired reservation");

        Status = ReservationStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;

        RaiseDomainEvent(new ReservationConfirmedEvent(
            Id,
            ShowtimeId,
            DateTime.UtcNow));
    }

    
    
    
    public void Expire()
    {
        if (!Status.IsPending)
            throw new InvalidOperationException($"Cannot expire reservation with status {Status.Value}");

        Status = ReservationStatus.Expired;

        RaiseDomainEvent(new ReservationExpiredEvent(
            Id,
            ShowtimeId,
            DateTime.UtcNow));
    }

    
    
    
    public void Cancel(string reason)
    {
        if (Status.IsConfirmed)
            throw new InvalidOperationException("Cannot cancel confirmed reservation");

        if (Status.IsCancelled)
            throw new InvalidOperationException("Reservation is already cancelled");

        Status = ReservationStatus.Cancelled;

        RaiseDomainEvent(new ReservationCancelledEvent(
            Id,
            ShowtimeId,
            reason,
            DateTime.UtcNow));
    }

    
    
    
    public bool HasExpired()
    {
        return DateTime.UtcNow > ExpiresAt && Status.IsPending;
    }

    
    
    
    private static void ValidateSeats(List<SeatNumber> seats)
    {
        if (seats == null || seats.Count == 0)
            throw new ArgumentException("Reservation must have at least one seat", nameof(seats));

        
        var firstRow = seats[0].Row;
        if (seats.Any(s => s.Row != firstRow))
            throw new ArgumentException("All seats must be in the same row", nameof(seats));

        
        var sortedSeats = seats.OrderBy(s => s.Number).ToList();
        for (int i = 0; i < sortedSeats.Count - 1; i++)
        {
            if (sortedSeats[i + 1].Number != sortedSeats[i].Number + 1)
                throw new ArgumentException("Seats must be contiguous (consecutive numbers)", nameof(seats));
        }
    }

#pragma warning disable CS8618
    private Reservation() { }
#pragma warning restore CS8618
}

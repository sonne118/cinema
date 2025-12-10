using Cinema.Domain.Common.Models;
using Cinema.Domain.ShowtimeAggregate.Events;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;

namespace Cinema.Domain.ShowtimeAggregate;





public sealed class Showtime : AggregateRoot<ShowtimeId>
{
    private readonly List<Guid> _reservedSeats = new();

    public MovieDetails MovieDetails { get; private set; }
    public ScreeningTime ScreeningTime { get; private set; }
    public Guid AuditoriumId { get; private set; }
    public ShowtimeStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public IReadOnlyList<Guid> ReservedSeats => _reservedSeats.AsReadOnly();

    private Showtime(
        ShowtimeId id,
        MovieDetails movieDetails,
        ScreeningTime screeningTime,
        Guid auditoriumId) : base(id)
    {
        MovieDetails = movieDetails;
        ScreeningTime = screeningTime;
        AuditoriumId = auditoriumId;
        Status = ShowtimeStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    
    
    
    public static Showtime Create(
        MovieDetails movieDetails,
        ScreeningTime screeningTime,
        Guid auditoriumId)
    {
        var showtimeId = ShowtimeId.CreateUnique();
        var showtime = new Showtime(
            showtimeId,
            movieDetails,
            screeningTime,
            auditoriumId);

        showtime.RaiseDomainEvent(new ShowtimeCreatedEvent(
            showtimeId,
            movieDetails,
            screeningTime,
            auditoriumId));

        return showtime;
    }

    
    
    
    public void Cancel(string reason)
    {
        if (Status == ShowtimeStatus.Cancelled)
            throw new InvalidOperationException("Showtime is already cancelled");

        if (ScreeningTime.IsWithinHours(2))
            throw new InvalidOperationException("Cannot cancel showtime within 2 hours of start time");

        Status = ShowtimeStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;

        RaiseDomainEvent(new ShowtimeCancelledEvent(
            Id,
            reason,
            DateTime.UtcNow));
    }

    
    
    
    public void ReserveSeats(IEnumerable<Guid> seatIds)
    {
        if (Status != ShowtimeStatus.Scheduled)
            throw new InvalidOperationException("Cannot reserve seats for cancelled showtime");

        if (ScreeningTime.HasPassed())
            throw new InvalidOperationException("Cannot reserve seats for past showtime");

        foreach (var seatId in seatIds)
        {
            if (!_reservedSeats.Contains(seatId))
            {
                _reservedSeats.Add(seatId);
            }
        }
    }

    
    
    
    public bool AreSeatsAvailable(IEnumerable<Guid> seatIds)
    {
        return !seatIds.Any(seatId => _reservedSeats.Contains(seatId));
    }

#pragma warning disable CS8618
    private Showtime() { }
#pragma warning restore CS8618
}

public enum ShowtimeStatus
{
    Scheduled,
    Cancelled,
    Completed
}

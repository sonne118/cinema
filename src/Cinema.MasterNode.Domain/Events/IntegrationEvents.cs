namespace Cinema.MasterNode.Domain.Events;

public abstract class IntegrationEvent
{
    public Guid Id { get; set; }
    public DateTime OccurredOnUtc { get; set; }
}

public class ReservationCreatedIntegrationEvent : IntegrationEvent
{
    public Guid ReservationId { get; set; }
    public Guid ShowtimeId { get; set; }
    public Guid CustomerId { get; set; }
    public List<SeatDto> Seats { get; set; } = new();
    public decimal TotalPrice { get; set; }
}

public class ReservationConfirmedIntegrationEvent : IntegrationEvent
{
    public Guid ReservationId { get; set; }
}

public class ShowtimeCreatedIntegrationEvent : IntegrationEvent
{
    public Guid ShowtimeId { get; set; }
    public string MovieImdbId { get; set; } = string.Empty;
    public DateTime ScreeningTime { get; set; }
    public Guid AuditoriumId { get; set; }
}

public class SeatDto
{
    public int Row { get; set; }
    public int Number { get; set; }
}

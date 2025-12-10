using Cinema.Domain.Common.Models;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;

namespace Cinema.Domain.ShowtimeAggregate.Events;




public sealed record ShowtimeCreatedEvent(
    ShowtimeId ShowtimeId,
    MovieDetails MovieDetails,
    ScreeningTime ScreeningTime,
    Guid AuditoriumId) : DomainEvent;

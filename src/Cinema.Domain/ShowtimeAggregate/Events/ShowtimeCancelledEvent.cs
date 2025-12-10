using Cinema.Domain.Common.Models;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;

namespace Cinema.Domain.ShowtimeAggregate.Events;




public sealed record ShowtimeCancelledEvent(
    ShowtimeId ShowtimeId,
    string Reason,
    DateTime CancelledAt) : DomainEvent;

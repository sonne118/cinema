using Cinema.Domain.Common.Models;
using Cinema.Domain.ReservationAggregate.ValueObjects;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;

namespace Cinema.Domain.ReservationAggregate.Events;




public sealed record ReservationCreatedEvent(
    ReservationId ReservationId,
    ShowtimeId ShowtimeId,
    IReadOnlyList<SeatNumber> Seats,
    DateTime ExpiresAt) : DomainEvent;




public sealed record ReservationConfirmedEvent(
    ReservationId ReservationId,
    ShowtimeId ShowtimeId,
    DateTime ConfirmedAt) : DomainEvent;




public sealed record ReservationExpiredEvent(
    ReservationId ReservationId,
    ShowtimeId ShowtimeId,
    DateTime ExpiredAt) : DomainEvent;




public sealed record ReservationCancelledEvent(
    ReservationId ReservationId,
    ShowtimeId ShowtimeId,
    string Reason,
    DateTime CancelledAt) : DomainEvent;

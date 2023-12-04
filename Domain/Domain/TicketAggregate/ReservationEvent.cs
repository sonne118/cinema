using ApiApplication.Domain.Common.Models;

namespace ApiApplication.Domain.TicketAggregate
{
    public record ReservationCreated(Reservation reserve) : IDomainEvent;
}
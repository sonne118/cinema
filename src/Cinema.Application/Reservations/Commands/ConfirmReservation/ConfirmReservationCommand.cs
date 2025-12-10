using Cinema.Domain.ReservationAggregate;
using MediatR;

namespace Cinema.Application.Reservations.Commands.ConfirmReservation;




public record ConfirmReservationCommand(Guid ReservationId) : IRequest<Reservation>;

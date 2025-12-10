using Cinema.Domain.ReservationAggregate;
using Cinema.Domain.ReservationAggregate.ValueObjects;
using MediatR;

namespace Cinema.Application.Reservations.Commands.CreateReservation;




public record CreateReservationCommand(
    Guid ShowtimeId,
    List<SeatRequest> Seats) : IRequest<Reservation>;

public record SeatRequest(int Row, int Number);

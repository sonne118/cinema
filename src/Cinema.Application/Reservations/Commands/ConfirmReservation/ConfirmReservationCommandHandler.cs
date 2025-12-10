using Cinema.Application.Common.Interfaces.Persistence;
using Cinema.Domain.ReservationAggregate;
using Cinema.Domain.ReservationAggregate.ValueObjects;
using MediatR;

namespace Cinema.Application.Reservations.Commands.ConfirmReservation;




public class ConfirmReservationCommandHandler : IRequestHandler<ConfirmReservationCommand, Reservation>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmReservationCommandHandler(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Reservation> Handle(ConfirmReservationCommand command, CancellationToken cancellationToken)
    {
        
        var reservationId = ReservationId.Create(command.ReservationId);
        var reservation = await _reservationRepository.GetByIdAsync(reservationId, cancellationToken);

        if (reservation is null)
            throw new InvalidOperationException($"Reservation with ID {command.ReservationId} not found");

        
        reservation.Confirm();

        
        _reservationRepository.Update(reservation);

        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation;
    }
}

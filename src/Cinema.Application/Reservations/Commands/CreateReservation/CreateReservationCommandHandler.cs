using Cinema.Application.Common.Interfaces.Persistence;
using Cinema.Application.Common.Interfaces.Services;
using Cinema.Domain.ReservationAggregate;
using Cinema.Domain.ReservationAggregate.ValueObjects;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;
using MediatR;

namespace Cinema.Application.Reservations.Commands.CreateReservation;




public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, Reservation>
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IShowtimeRepository _showtimeRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReservationCommandHandler(
        IReservationRepository reservationRepository,
        IShowtimeRepository showtimeRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _reservationRepository = reservationRepository;
        _showtimeRepository = showtimeRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Reservation> Handle(CreateReservationCommand command, CancellationToken cancellationToken)
    {
        
        var showtimeId = ShowtimeId.Create(command.ShowtimeId);
        var showtime = await _showtimeRepository.GetByIdAsync(showtimeId, cancellationToken);
        
        if (showtime is null)
            throw new InvalidOperationException($"Showtime with ID {command.ShowtimeId} not found");

        
        var seatNumbers = command.Seats
            .Select(s => SeatNumber.Create(s.Row, s.Number))
            .ToList();

        
        var existingReservations = await _reservationRepository.GetByShowtimeIdAsync(showtimeId, cancellationToken);
        
        
        var soldSeats = existingReservations
            .Where(r => r.Status.IsConfirmed)
            .SelectMany(r => r.Seats)
            .ToList();

        if (seatNumbers.Any(seat => soldSeats.Any(s => s.Equals(seat))))
            throw new InvalidOperationException("One or more seats are already sold");

        
        var recentReservations = existingReservations
            .Where(r => r.Status.IsPending && !r.HasExpired())
            .SelectMany(r => r.Seats)
            .ToList();

        if (seatNumbers.Any(seat => recentReservations.Any(s => s.Equals(seat))))
            throw new InvalidOperationException("One or more seats are currently reserved");

        
        var reservation = Reservation.Create(
            showtimeId,
            seatNumbers,
            _dateTimeProvider.UtcNow);

        
        await _reservationRepository.AddAsync(reservation, cancellationToken);

        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation;
    }
}

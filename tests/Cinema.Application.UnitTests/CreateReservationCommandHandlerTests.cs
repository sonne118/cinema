using Cinema.Application.Common.Interfaces.Persistence;
using Cinema.Application.Common.Interfaces.Services;
using Cinema.Application.Reservations.Commands.CreateReservation;
using Cinema.Domain.ReservationAggregate;
using Cinema.Domain.ReservationAggregate.ValueObjects;
using Cinema.Domain.ShowtimeAggregate;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace Cinema.Application.UnitTests.Reservations.Commands;

public class CreateReservationCommandHandlerTests
{
    private readonly Mock<IReservationRepository> _reservationRepositoryMock;
    private readonly Mock<IShowtimeRepository> _showtimeRepositoryMock;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateReservationCommandHandler _handler;

    public CreateReservationCommandHandlerTests()
    {
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _showtimeRepositoryMock = new Mock<IShowtimeRepository>();
        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        _handler = new CreateReservationCommandHandler(
            _reservationRepositoryMock.Object,
            _showtimeRepositoryMock.Object,
            _dateTimeProviderMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateReservation()
    {
        
        var showtimeId = Guid.NewGuid();
        var showtime = CreateShowtime(showtimeId);

        _showtimeRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<ShowtimeId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(showtime);

        _reservationRepositoryMock
            .Setup(x => x.GetByShowtimeIdAsync(It.IsAny<ShowtimeId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation>());

        var command = new CreateReservationCommand(
            showtimeId,
            new List<SeatRequest>
            {
                new(5, 10),
                new(5, 11)
            });

        
        var result = await _handler.Handle(command, CancellationToken.None);

        
        result.Should().NotBeNull();
        result.Seats.Should().HaveCount(2);
        result.Status.Should().Be(ReservationStatus.Pending);

        _reservationRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentShowtime_ShouldThrowException()
    {
        
        _showtimeRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<ShowtimeId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Showtime?)null);

        var command = new CreateReservationCommand(
            Guid.NewGuid(),
            new List<SeatRequest> { new(5, 10) });

        
        var act = () => _handler.Handle(command, CancellationToken.None);

        
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_WithAlreadySoldSeats_ShouldThrowException()
    {
        
        var showtimeId = Guid.NewGuid();
        var showtime = CreateShowtime(showtimeId);

        var existingReservation = CreateConfirmedReservation(ShowtimeId.Create(showtimeId));

        _showtimeRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<ShowtimeId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(showtime);

        _reservationRepositoryMock
            .Setup(x => x.GetByShowtimeIdAsync(It.IsAny<ShowtimeId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Reservation> { existingReservation });

        var command = new CreateReservationCommand(
            showtimeId,
            new List<SeatRequest>
            {
                new(5, 10), 
                new(5, 11)
            });

        
        var act = () => _handler.Handle(command, CancellationToken.None);

        
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already sold*");
    }

    
    private static Showtime CreateShowtime(Guid showtimeId)
    {
        var movieDetails = MovieDetails.Create("tt0111161", "The Shawshank Redemption");
        var screeningTime = ScreeningTime.CreateForTesting(DateTime.UtcNow.AddDays(1));
        var auditoriumId = Guid.NewGuid();

        var showtime = Showtime.Create(movieDetails, screeningTime, auditoriumId);
        
        
        var idProperty = typeof(Showtime).BaseType!.GetProperty("Id")!;
        idProperty.SetValue(showtime, ShowtimeId.Create(showtimeId));

        return showtime;
    }

    private static Reservation CreateConfirmedReservation(ShowtimeId showtimeId)
    {
        var seats = new List<SeatNumber>
        {
            SeatNumber.Create(5, 10),
            SeatNumber.Create(5, 11)
        };

        var reservation = Reservation.Create(showtimeId, seats, DateTime.UtcNow);
        reservation.Confirm();

        return reservation;
    }
}

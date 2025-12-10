using Cinema.Domain.ShowtimeAggregate;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Cinema.Domain.UnitTests.ShowtimeAggregate;

public class ShowtimeTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateShowtime()
    {
        
        var movieDetails = MovieDetails.Create("tt0111161", "The Shawshank Redemption", "http://poster.jpg", 1994);
        var screeningTime = ScreeningTime.CreateForTesting(DateTime.UtcNow.AddDays(1));
        var auditoriumId = Guid.NewGuid();

        
        var showtime = Showtime.Create(movieDetails, screeningTime, auditoriumId);

        
        showtime.Should().NotBeNull();
        showtime.MovieDetails.Should().Be(movieDetails);
        showtime.ScreeningTime.Should().Be(screeningTime);
        showtime.AuditoriumId.Should().Be(auditoriumId);
        showtime.Status.Should().Be(ShowtimeStatus.Scheduled);
        showtime.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void Cancel_WithScheduledShowtime_ShouldCancelSuccessfully()
    {
        
        var showtime = CreateValidShowtime();

        
        showtime.Cancel("Maintenance required");

        
        showtime.Status.Should().Be(ShowtimeStatus.Cancelled);
        showtime.CancelledAt.Should().NotBeNull();
        showtime.DomainEvents.Should().HaveCount(2); 
    }

    [Fact]
    public void Cancel_WithCancelledShowtime_ShouldThrowException()
    {
        
        var showtime = CreateValidShowtime();
        showtime.Cancel("First cancellation");

        
        var act = () => showtime.Cancel("Second cancellation");

        
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*already cancelled*");
    }

    [Fact]
    public void ReserveSeats_WithScheduledShowtime_ShouldReserveSuccessfully()
    {
        
        var showtime = CreateValidShowtime();
        var seatIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        
        showtime.ReserveSeats(seatIds);

        
        showtime.ReservedSeats.Should().HaveCount(2);
        showtime.ReservedSeats.Should().Contain(seatIds);
    }

    [Fact]
    public void ReserveSeats_WithCancelledShowtime_ShouldThrowException()
    {
        
        var showtime = CreateValidShowtime();
        showtime.Cancel("Test");
        var seatIds = new List<Guid> { Guid.NewGuid() };

        
        var act = () => showtime.ReserveSeats(seatIds);

        
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*cancelled*");
    }

    [Fact]
    public void AreSeatsAvailable_WithUnreservedSeats_ShouldReturnTrue()
    {
        
        var showtime = CreateValidShowtime();
        var seatIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        
        var available = showtime.AreSeatsAvailable(seatIds);

        
        available.Should().BeTrue();
    }

    [Fact]
    public void AreSeatsAvailable_WithReservedSeats_ShouldReturnFalse()
    {
        
        var showtime = CreateValidShowtime();
        var seatIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        showtime.ReserveSeats(seatIds);

        
        var available = showtime.AreSeatsAvailable(seatIds);

        
        available.Should().BeFalse();
    }

    
    private static Showtime CreateValidShowtime()
    {
        var movieDetails = MovieDetails.Create("tt0111161", "The Shawshank Redemption");
        var screeningTime = ScreeningTime.CreateForTesting(DateTime.UtcNow.AddDays(3));
        var auditoriumId = Guid.NewGuid();

        return Showtime.Create(movieDetails, screeningTime, auditoriumId);
    }
}

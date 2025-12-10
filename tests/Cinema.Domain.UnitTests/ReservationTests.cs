using Cinema.Domain.ReservationAggregate;
using Cinema.Domain.ReservationAggregate.ValueObjects;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Cinema.Domain.UnitTests.ReservationAggregate;

public class ReservationTests
{
    [Fact]
    public void Create_WithValidSeats_ShouldCreateReservation()
    {
        
        var showtimeId = ShowtimeId.CreateUnique();
        var seats = new List<SeatNumber>
        {
            SeatNumber.Create(5, 10),
            SeatNumber.Create(5, 11),
            SeatNumber.Create(5, 12)
        };
        var createdAt = DateTime.UtcNow;

        
        var reservation = Reservation.Create(showtimeId, seats, createdAt);

        
        reservation.Should().NotBeNull();
        reservation.ShowtimeId.Should().Be(showtimeId);
        reservation.Seats.Should().HaveCount(3);
        reservation.Status.Should().Be(ReservationStatus.Pending);
        reservation.CreatedAt.Should().Be(createdAt);
        reservation.ExpiresAt.Should().BeCloseTo(createdAt.AddMinutes(10), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithNonContiguousSeats_ShouldThrowException()
    {
        
        var showtimeId = ShowtimeId.CreateUnique();
        var seats = new List<SeatNumber>
        {
            SeatNumber.Create(5, 10),
            SeatNumber.Create(5, 12) 
        };

        
        var act = () => Reservation.Create(showtimeId, seats, DateTime.UtcNow);

        
        act.Should().Throw<ArgumentException>()
            .WithMessage("*contiguous*");
    }

    [Fact]
    public void Create_WithDifferentRows_ShouldThrowException()
    {
        
        var showtimeId = ShowtimeId.CreateUnique();
        var seats = new List<SeatNumber>
        {
            SeatNumber.Create(5, 10),
            SeatNumber.Create(6, 11) 
        };

        
        var act = () => Reservation.Create(showtimeId, seats, DateTime.UtcNow);

        
        act.Should().Throw<ArgumentException>()
            .WithMessage("*same row*");
    }

    [Fact]
    public void Confirm_WithPendingReservation_ShouldConfirmSuccessfully()
    {
        
        var reservation = CreateValidReservation();

        
        reservation.Confirm();

        
        reservation.Status.Should().Be(ReservationStatus.Confirmed);
        reservation.ConfirmedAt.Should().NotBeNull();
        reservation.ConfirmedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Confirm_WithExpiredReservation_ShouldThrowException()
    {
        
        var createdAt = DateTime.UtcNow.AddMinutes(-15); 
        var reservation = CreateValidReservation(createdAt);

        
        var act = () => reservation.Confirm();

        
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*expired*");
    }

    [Fact]
    public void Expire_WithPendingReservation_ShouldExpireSuccessfully()
    {
        
        var reservation = CreateValidReservation();

        
        reservation.Expire();

        
        reservation.Status.Should().Be(ReservationStatus.Expired);
    }

    [Fact]
    public void Cancel_WithPendingReservation_ShouldCancelSuccessfully()
    {
        
        var reservation = CreateValidReservation();

        
        reservation.Cancel("Customer requested cancellation");

        
        reservation.Status.Should().Be(ReservationStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WithConfirmedReservation_ShouldThrowException()
    {
        
        var reservation = CreateValidReservation();
        reservation.Confirm();

        
        var act = () => reservation.Cancel("Test");

        
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*confirmed*");
    }

    [Fact]
    public void HasExpired_WithRecentReservation_ShouldReturnFalse()
    {
        
        var reservation = CreateValidReservation();

        
        var hasExpired = reservation.HasExpired();

        
        hasExpired.Should().BeFalse();
    }

    [Fact]
    public void HasExpired_WithOldReservation_ShouldReturnTrue()
    {
        
        var createdAt = DateTime.UtcNow.AddMinutes(-15);
        var reservation = CreateValidReservation(createdAt);

        
        var hasExpired = reservation.HasExpired();

        
        hasExpired.Should().BeTrue();
    }

    
    private static Reservation CreateValidReservation(DateTime? createdAt = null)
    {
        var showtimeId = ShowtimeId.CreateUnique();
        var seats = new List<SeatNumber>
        {
            SeatNumber.Create(5, 10),
            SeatNumber.Create(5, 11)
        };
        return Reservation.Create(showtimeId, seats, createdAt ?? DateTime.UtcNow);
    }
}

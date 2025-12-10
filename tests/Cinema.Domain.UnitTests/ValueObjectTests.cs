using Cinema.Domain.ReservationAggregate.ValueObjects;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Cinema.Domain.UnitTests.ValueObjects;

public class SeatNumberTests
{
    [Fact]
    public void Create_WithValidRowAndNumber_ShouldCreateSeat()
    {
        
        var seat = SeatNumber.Create(5, 10);

        
        seat.Row.Should().Be(5);
        seat.Number.Should().Be(10);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-1, 10)]
    [InlineData(5, 0)]
    [InlineData(5, -1)]
    public void Create_WithInvalidValues_ShouldThrowException(int row, int number)
    {
        
        var act = () => SeatNumber.Create(row, number);

        
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void IsContiguousWith_WithAdjacentSeats_ShouldReturnTrue()
    {
        
        var seat1 = SeatNumber.Create(5, 10);
        var seat2 = SeatNumber.Create(5, 11);

        
        var isContiguous = seat1.IsContiguousWith(seat2);

        
        isContiguous.Should().BeTrue();
    }

    [Fact]
    public void IsContiguousWith_WithNonAdjacentSeats_ShouldReturnFalse()
    {
        
        var seat1 = SeatNumber.Create(5, 10);
        var seat2 = SeatNumber.Create(5, 12);

        
        var isContiguous = seat1.IsContiguousWith(seat2);

        
        isContiguous.Should().BeFalse();
    }

    [Fact]
    public void IsContiguousWith_WithDifferentRows_ShouldReturnFalse()
    {
        
        var seat1 = SeatNumber.Create(5, 10);
        var seat2 = SeatNumber.Create(6, 11);

        
        var isContiguous = seat1.IsContiguousWith(seat2);

        
        isContiguous.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        
        var seat1 = SeatNumber.Create(5, 10);
        var seat2 = SeatNumber.Create(5, 10);

        
        seat1.Should().Be(seat2);
    }
}

public class MovieDetailsTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateMovieDetails()
    {
        
        var movie = MovieDetails.Create("tt0111161", "The Shawshank Redemption", "http://poster.jpg", 1994);

        
        movie.ImdbId.Should().Be("tt0111161");
        movie.Title.Should().Be("The Shawshank Redemption");
        movie.PosterUrl.Should().Be("http://poster.jpg");
        movie.ReleaseYear.Should().Be(1994);
    }

    [Theory]
    [InlineData("", "Title")]
    [InlineData("tt123", "")]
    [InlineData(null, "Title")]
    [InlineData("tt123", null)]
    public void Create_WithInvalidData_ShouldThrowException(string imdbId, string title)
    {
        
        var act = () => MovieDetails.Create(imdbId, title);

        
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(1700)]
    [InlineData(2040)]
    public void Create_WithInvalidYear_ShouldThrowException(int year)
    {
        
        var act = () => MovieDetails.Create("tt123", "Title", null, year);

        
        act.Should().Throw<ArgumentException>();
    }
}

public class ScreeningTimeTests
{
    [Fact]
    public void Create_WithFutureTime_ShouldCreateScreeningTime()
    {
        
        var futureTime = DateTime.UtcNow.AddDays(1);

        
        var screeningTime = ScreeningTime.Create(futureTime);

        
        screeningTime.Value.Should().Be(futureTime);
    }

    [Fact]
    public void Create_WithPastTime_ShouldThrowException()
    {
        
        var pastTime = DateTime.UtcNow.AddDays(-1);

        
        var act = () => ScreeningTime.Create(pastTime);

        
        act.Should().Throw<ArgumentException>()
            .WithMessage("*future*");
    }

    [Fact]
    public void HasPassed_WithPastTime_ShouldReturnTrue()
    {
        
        var pastTime = DateTime.UtcNow.AddHours(-1);
        var screeningTime = ScreeningTime.CreateForTesting(pastTime);

        
        var hasPassed = screeningTime.HasPassed();

        
        hasPassed.Should().BeTrue();
    }

    [Fact]
    public void HasPassed_WithFutureTime_ShouldReturnFalse()
    {
        
        var futureTime = DateTime.UtcNow.AddHours(1);
        var screeningTime = ScreeningTime.CreateForTesting(futureTime);

        
        var hasPassed = screeningTime.HasPassed();

        
        hasPassed.Should().BeFalse();
    }
}

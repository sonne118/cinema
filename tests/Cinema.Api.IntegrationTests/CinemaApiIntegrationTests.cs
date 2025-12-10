using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Cinema.Api.IntegrationTests;






public class CinemaApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CinemaApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact(Skip = "Requires infrastructure")]
    public async Task GetShowtimes_ShouldReturnSuccess()
    {
        
        var response = await _client.GetAsync("/api/showtimes");

        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires infrastructure")]
    public async Task CreateShowtime_WithValidData_ShouldReturnCreated()
    {
        
        var request = new
        {
            movieImdbId = "tt0111161",
            screeningTime = DateTime.UtcNow.AddDays(1),
            auditoriumId = Guid.NewGuid()
        };

        
        var response = await _client.PostAsJsonAsync("/api/showtimes", request);

        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("id");
    }

    [Fact(Skip = "Requires infrastructure")]
    public async Task CreateReservation_WithValidData_ShouldReturnCreated()
    {
        
        var showtimeId = await CreateTestShowtime();

        var request = new
        {
            showtimeId = showtimeId,
            seats = new[]
            {
                new { row = 5, number = 10 },
                new { row = 5, number = 11 }
            }
        };

        
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact(Skip = "Requires infrastructure")]
    public async Task ConfirmReservation_WithValidReservation_ShouldReturnSuccess()
    {
        
        var reservationId = await CreateTestReservation();

        
        var response = await _client.PostAsync($"/api/reservations/{reservationId}/confirm", null);

        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires infrastructure")]
    public async Task CreateReservation_WithNonContiguousSeats_ShouldReturnBadRequest()
    {
        
        var showtimeId = await CreateTestShowtime();

        var request = new
        {
            showtimeId = showtimeId,
            seats = new[]
            {
                new { row = 5, number = 10 },
                new { row = 5, number = 12 } 
            }
        };

        
        var response = await _client.PostAsJsonAsync("/api/reservations", request);

        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    
    private async Task<Guid> CreateTestShowtime()
    {
        var request = new
        {
            movieImdbId = "tt0111161",
            screeningTime = DateTime.UtcNow.AddDays(1),
            auditoriumId = Guid.NewGuid()
        };

        var response = await _client.PostAsJsonAsync("/api/showtimes", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ShowtimeResponse>();
        return Guid.Parse(result!.Id);
    }

    private async Task<Guid> CreateTestReservation()
    {
        var showtimeId = await CreateTestShowtime();

        var request = new
        {
            showtimeId = showtimeId,
            seats = new[]
            {
                new { row = 5, number = 10 },
                new { row = 5, number = 11 }
            }
        };

        var response = await _client.PostAsJsonAsync("/api/reservations", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ReservationResponse>();
        return Guid.Parse(result!.Id);
    }

    
    private record ShowtimeResponse(string Id);
    private record ReservationResponse(string Id);
}

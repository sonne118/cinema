using System.Text.Json.Serialization;

namespace Cinema.ApiGateway.Models
{
    
    public class CreateShowtimeRequestDto
    {
        [JsonPropertyName("movieImdbId")]
        public string MovieImdbId { get; set; } = string.Empty;

        [JsonPropertyName("screeningTime")]
        public string ScreeningTime { get; set; } = string.Empty;

        [JsonPropertyName("auditoriumId")]
        public string AuditoriumId { get; set; } = string.Empty;
    }

    public class CreateShowtimeResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class CreateReservationRequestDto
    {
        [JsonPropertyName("showtimeId")]
        public string ShowtimeId { get; set; } = string.Empty;

        [JsonPropertyName("customerId")]
        public string CustomerId { get; set; } = string.Empty;

        [JsonPropertyName("seatNumber")]
        public int SeatNumber { get; set; }

        [JsonPropertyName("rowNumber")]
        public int RowNumber { get; set; }
    }

    public class CreateReservationResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    
    public class MovieDto
    {
        public string ImdbId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
    }

    public class ShowtimeDto
    {
        public string Id { get; set; } = string.Empty;
        public MovieDto? Movie { get; set; }
        public string ScreeningTime { get; set; } = string.Empty;
        public string AuditoriumId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
    }

    public class ListShowtimesResponseDto
    {
        public List<ShowtimeDto> Showtimes { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class SeatDto
    {
        public int Row { get; set; }
        public int Number { get; set; }
    }

    public class ReservationDto
    {
        public string Id { get; set; } = string.Empty;
        public string ShowtimeId { get; set; } = string.Empty;
        public List<SeatDto> Seats { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string ExpiresAt { get; set; } = string.Empty;
        public string ConfirmedAt { get; set; } = string.Empty;
    }
}

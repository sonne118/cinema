using Microsoft.AspNetCore.Mvc;
using Cinema.Api.Services; // Write Service Namespace
using Cinema.GrpcService; // Read Service Namespace
using Grpc.Core;
using Cinema.ApiGateway.Models;

using WriteRequest = Cinema.Api.Services;
using ReadRequest = Cinema.GrpcService;

namespace Cinema.ApiGateway.Controllers
{
    [ApiController]
    [Route("api")]
    public class GatewayController : ControllerBase
    {
        private readonly CinemaWriteService.CinemaWriteServiceClient _writeClient;
        private readonly CinemaService.CinemaServiceClient _readClient;
        private readonly ILogger<GatewayController> _logger;

        public GatewayController(
            CinemaWriteService.CinemaWriteServiceClient writeClient,
            CinemaService.CinemaServiceClient readClient,
            ILogger<GatewayController> logger)
        {
            _writeClient = writeClient;
            _readClient = readClient;
            _logger = logger;
        }

        [HttpPost("showtimes")]
        [ProducesResponseType(typeof(CreateShowtimeResponseDto), 200)]
        public async Task<IActionResult> CreateShowtime([FromBody] CreateShowtimeRequestDto request)
        {
            try
            {
                var grpcRequest = new WriteRequest.CreateShowtimeRequest
                {
                    MovieImdbId = request.MovieImdbId,
                    ScreeningTime = request.ScreeningTime,
                    AuditoriumId = request.AuditoriumId
                };

                var response = await _writeClient.CreateShowtimeAsync(grpcRequest);
                
                return Ok(new CreateShowtimeResponseDto
                {
                    Id = response.Id,
                    Status = response.Status
                });
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Error creating showtime");
                return StatusCode((int)ex.StatusCode, ex.Status.Detail);
            }
        }

        [HttpPost("reservations")]
        [ProducesResponseType(typeof(CreateReservationResponseDto), 200)]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequestDto request)
        {
            try
            {
                var grpcRequest = new WriteRequest.CreateReservationRequest
                {
                    ShowtimeId = request.ShowtimeId,
                    CustomerId = request.CustomerId,
                    SeatNumber = request.SeatNumber,
                    RowNumber = request.RowNumber
                };

                var response = await _writeClient.CreateReservationAsync(grpcRequest);

                return Ok(new CreateReservationResponseDto
                {
                    Id = response.Id,
                    Status = response.Status
                });
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Error creating reservation");
                return StatusCode((int)ex.StatusCode, ex.Status.Detail);
            }
        }

        [HttpGet("showtimes/{id}")]
        [ProducesResponseType(typeof(ShowtimeDto), 200)]
        public async Task<IActionResult> GetShowtime(string id)
        {
            try
            {
                var request = new GetShowtimeRequest { Id = id };
                var response = await _readClient.GetShowtimeAsync(request);
                
                return Ok(MapShowtime(response));
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Error getting showtime");
                return StatusCode((int)ex.StatusCode, ex.Status.Detail);
            }
        }

        [HttpGet("showtimes")]
        [ProducesResponseType(typeof(ListShowtimesResponseDto), 200)]
        public async Task<IActionResult> ListShowtimes([FromQuery] string? date, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var request = new ListShowtimesRequest 
                { 
                    DateFilter = date ?? "",
                    PageNumber = page,
                    PageSize = pageSize
                };
                var response = await _readClient.ListShowtimesAsync(request);

                return Ok(new ListShowtimesResponseDto
                {
                    TotalCount = response.TotalCount,
                    Showtimes = response.Showtimes.Select(MapShowtime).ToList()
                });
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Error listing showtimes");
                return StatusCode((int)ex.StatusCode, ex.Status.Detail);
            }
        }

        [HttpGet("reservations/{id}")]
        [ProducesResponseType(typeof(ReservationDto), 200)]
        public async Task<IActionResult> GetReservation(string id)
        {
            try
            {
                var request = new GetReservationRequest { Id = id };
                var response = await _readClient.GetReservationAsync(request);
                
                return Ok(new ReservationDto
                {
                    Id = response.Id,
                    ShowtimeId = response.ShowtimeId,
                    Status = response.Status,
                    CreatedAt = response.CreatedAt,
                    ExpiresAt = response.ExpiresAt,
                    ConfirmedAt = response.ConfirmedAt,
                    Seats = response.Seats.Select(s => new SeatDto { Row = s.Row, Number = s.Number }).ToList()
                });
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Error getting reservation");
                return StatusCode((int)ex.StatusCode, ex.Status.Detail);
            }
        }

        private static ShowtimeDto MapShowtime(ShowtimeResponse response)
        {
            return new ShowtimeDto
            {
                Id = response.Id,
                ScreeningTime = response.ScreeningTime,
                AuditoriumId = response.AuditoriumId,
                Status = response.Status,
                CreatedAt = response.CreatedAt,
                Movie = response.Movie == null ? null : new MovieDto
                {
                    ImdbId = response.Movie.ImdbId,
                    Title = response.Movie.Title,
                    PosterUrl = response.Movie.PosterUrl,
                    ReleaseYear = response.Movie.ReleaseYear
                }
            };
        }
    }
}

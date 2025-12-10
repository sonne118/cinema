using Cinema.Application.Common.Interfaces.Queries;
using Grpc.Core;
using MediatR;

namespace Cinema.GrpcService.Services;




public class CinemaGrpcService : CinemaService.CinemaServiceBase
{
    private readonly IMediator _mediator;
    private readonly IShowtimeReadRepository _showtimeReadRepository;
    private readonly IReservationReadRepository _reservationReadRepository;
    private readonly ILogger<CinemaGrpcService> _logger;

    public CinemaGrpcService(
        IMediator mediator,
        IShowtimeReadRepository showtimeReadRepository,
        IReservationReadRepository reservationReadRepository,
        ILogger<CinemaGrpcService> logger)
    {
        _mediator = mediator;
        _showtimeReadRepository = showtimeReadRepository;
        _reservationReadRepository = reservationReadRepository;
        _logger = logger;
    }

    public override async Task<ShowtimeResponse> GetShowtime(
        GetShowtimeRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("gRPC GetShowtime called for ID: {ShowtimeId}", request.Id);

        if (!Guid.TryParse(request.Id, out var showtimeGuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid showtime ID format"));
        }

        var showtime = await _showtimeReadRepository.GetByIdAsync(showtimeGuid, context.CancellationToken);

        if (showtime == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Showtime not found"));
        }

        return new ShowtimeResponse
        {
            Id = showtime.Id.ToString(),
            Movie = new MovieDetails
            {
                ImdbId = showtime.MovieImdbId,
                Title = showtime.MovieTitle,
                PosterUrl = showtime.PosterUrl ?? "",
                ReleaseYear = showtime.ReleaseYear ?? 0
            },
            ScreeningTime = showtime.ScreeningTime.ToString("O"),
            AuditoriumId = showtime.AuditoriumId.ToString(),
            Status = showtime.Status,
            CreatedAt = showtime.CreatedAt.ToString("O")
        };
    }

    public override async Task<ListShowtimesResponse> ListShowtimes(
        ListShowtimesRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("gRPC ListShowtimes called");

        var showtimes = await _showtimeReadRepository.GetAllAsync(context.CancellationToken);

        var response = new ListShowtimesResponse
        {
            TotalCount = showtimes.Count
        };

        response.Showtimes.AddRange(showtimes.Select(s => new ShowtimeResponse
        {
            Id = s.Id.ToString(),
            Movie = new MovieDetails
            {
                ImdbId = s.MovieImdbId,
                Title = s.MovieTitle,
                PosterUrl = s.PosterUrl ?? "",
                ReleaseYear = s.ReleaseYear ?? 0
            },
            ScreeningTime = s.ScreeningTime.ToString("O"),
            AuditoriumId = s.AuditoriumId.ToString(),
            Status = s.Status,
            CreatedAt = s.CreatedAt.ToString("O")
        }));

        return response;
    }

    public override async Task<ReservationResponse> GetReservation(
        GetReservationRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("gRPC GetReservation called for ID: {ReservationId}", request.Id);

        if (!Guid.TryParse(request.Id, out var reservationGuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid reservation ID format"));
        }

        var reservation = await _reservationReadRepository.GetByIdAsync(reservationGuid, context.CancellationToken);

        if (reservation == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Reservation not found"));
        }

        var response = new ReservationResponse
        {
            Id = reservation.Id.ToString(),
            ShowtimeId = reservation.ShowtimeId.ToString(),
            Status = reservation.Status,
            CreatedAt = reservation.CreatedAt.ToString("O"),
            ExpiresAt = reservation.ExpiresAt.ToString("O"),
            ConfirmedAt = reservation.ConfirmedAt?.ToString("O") ?? ""
        };

        response.Seats.AddRange(reservation.Seats.Select(s => new Seat
        {
            Row = s.Row,
            Number = s.Number
        }));

        return response;
    }
}

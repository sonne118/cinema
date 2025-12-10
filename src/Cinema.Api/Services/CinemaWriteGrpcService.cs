using Cinema.Application.Reservations.Commands.CreateReservation;
using Cinema.Application.Showtimes.Commands.CreateShowtime;
using Grpc.Core;
using MediatR;

namespace Cinema.Api.Services;

public class CinemaWriteGrpcService : CinemaWriteService.CinemaWriteServiceBase
{
    private readonly ISender _sender;
    private readonly ILogger<CinemaWriteGrpcService> _logger;

    public CinemaWriteGrpcService(ISender sender, ILogger<CinemaWriteGrpcService> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public override async Task<CreateShowtimeResponse> CreateShowtime(CreateShowtimeRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC CreateShowtime called for movie: {MovieId}", request.MovieImdbId);

        if (!DateTime.TryParse(request.ScreeningTime, out var screeningTime))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid screening time format"));
        }

        if (!Guid.TryParse(request.AuditoriumId, out var auditoriumGuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid auditorium ID format"));
        }

        var command = new CreateShowtimeCommand(
            request.MovieImdbId,
            screeningTime,
            auditoriumGuid);

        var result = await _sender.Send(command, context.CancellationToken);

        return new CreateShowtimeResponse
        {
            Id = result.Id.Value.ToString(),
            Status = result.Status.ToString()
        };
    }

    public override async Task<CreateReservationResponse> CreateReservation(CreateReservationRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC CreateReservation called for showtime: {ShowtimeId}", request.ShowtimeId);

        if (!Guid.TryParse(request.ShowtimeId, out var showtimeGuid))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid showtime ID format"));
        }

        
        

        var seats = new List<SeatRequest>
        {
            new SeatRequest(request.RowNumber, request.SeatNumber)
        };

        var command = new CreateReservationCommand(
            showtimeGuid,
            seats);

        var result = await _sender.Send(command, context.CancellationToken);

        return new CreateReservationResponse
        {
            Id = result.Id.Value.ToString(),
            Status = result.Status.ToString()
        };
    }
}

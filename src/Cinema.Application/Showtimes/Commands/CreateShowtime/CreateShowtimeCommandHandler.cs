using Cinema.Application.Common.Interfaces.Persistence;
using Cinema.Application.Common.Interfaces.Services;
using Cinema.Domain.ShowtimeAggregate;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;
using MediatR;

namespace Cinema.Application.Showtimes.Commands.CreateShowtime;




public class CreateShowtimeCommandHandler : IRequestHandler<CreateShowtimeCommand, Showtime>
{
    private readonly IShowtimeRepository _showtimeRepository;
    private readonly IMovieApiService _movieApiService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateShowtimeCommandHandler(
        IShowtimeRepository showtimeRepository,
        IMovieApiService movieApiService,
        IUnitOfWork unitOfWork)
    {
        _showtimeRepository = showtimeRepository;
        _movieApiService = movieApiService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Showtime> Handle(CreateShowtimeCommand command, CancellationToken cancellationToken)
    {
        
        var movieDetails = await _movieApiService.GetMovieByImdbIdAsync(
            command.MovieImdbId,
            cancellationToken);

        
        var screeningTime = ScreeningTime.Create(command.ScreeningTime);

        
        var showtime = Showtime.Create(
            movieDetails,
            screeningTime,
            command.AuditoriumId);

        
        await _showtimeRepository.AddAsync(showtime, cancellationToken);

        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return showtime;
    }
}

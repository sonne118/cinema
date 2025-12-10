using Cinema.Application.Common.Interfaces.Persistence;
using Cinema.Application.Showtimes.Commands.CreateShowtime;
using Cinema.Contracts.Showtimes;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowtimesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IShowtimeRepository _showtimeRepository;

    public ShowtimesController(ISender sender, IShowtimeRepository showtimeRepository)
    {
        _sender = sender;
        _showtimeRepository = showtimeRepository;
    }

    
    
    
    [HttpPost]
    [ProducesResponseType(typeof(ShowtimeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateShowtime(
        [FromBody] CreateShowtimeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateShowtimeCommand(
            request.MovieImdbId,
            request.ScreeningTime,
            request.AuditoriumId);

        var showtime = await _sender.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetShowtime),
            new { id = showtime.Id.Value },
            new ShowtimeResponse(
                showtime.Id.Value,
                new MovieDetailsResponse(
                    showtime.MovieDetails.ImdbId,
                    showtime.MovieDetails.Title,
                    showtime.MovieDetails.PosterUrl,
                    showtime.MovieDetails.ReleaseYear),
                showtime.ScreeningTime.Value,
                showtime.AuditoriumId,
                showtime.Status.ToString(),
                showtime.CreatedAt));
    }

    
    
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ShowtimeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShowtime(
        Guid id,
        CancellationToken cancellationToken)
    {
        
        var showtimeId = ShowtimeId.Create(id);
        var showtime = await _showtimeRepository.GetByIdAsync(showtimeId, cancellationToken);

        if (showtime == null)
            return NotFound();

        return Ok(new ShowtimeResponse(
            showtime.Id.Value,
            new MovieDetailsResponse(
                showtime.MovieDetails.ImdbId,
                showtime.MovieDetails.Title,
                showtime.MovieDetails.PosterUrl,
                showtime.MovieDetails.ReleaseYear),
            showtime.ScreeningTime.Value,
            showtime.AuditoriumId,
            showtime.Status.ToString(),
            showtime.CreatedAt));
    }

    
    
    
    [HttpGet]
    [ProducesResponseType(typeof(List<ShowtimeResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListShowtimes(CancellationToken cancellationToken)
    {
        
        
        
        
        return Ok(new List<ShowtimeResponse>());
    }
}

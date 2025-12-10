using Cinema.Domain.ShowtimeAggregate;
using MediatR;

namespace Cinema.Application.Showtimes.Commands.CreateShowtime;




public record CreateShowtimeCommand(
    string MovieImdbId,
    DateTime ScreeningTime,
    Guid AuditoriumId) : IRequest<Showtime>;

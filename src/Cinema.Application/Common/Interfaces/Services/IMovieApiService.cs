using Cinema.Domain.ShowtimeAggregate.ValueObjects;

namespace Cinema.Application.Common.Interfaces.Services;




public interface IMovieApiService
{
    Task<MovieDetails> GetMovieByImdbIdAsync(string imdbId, CancellationToken cancellationToken = default);
}

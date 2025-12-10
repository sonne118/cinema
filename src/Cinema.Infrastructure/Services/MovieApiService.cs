using Cinema.Application.Common.Interfaces.Services;
using Cinema.Domain.ShowtimeAggregate.ValueObjects;

namespace Cinema.Infrastructure.Services;

public class MovieApiService : IMovieApiService
{
    public Task<MovieDetails> GetMovieByImdbIdAsync(string imdbId, CancellationToken cancellationToken = default)
    {
        
        
        var movieDetails = MovieDetails.Create(
            imdbId,
            $"Movie {imdbId}",
            "https://example.com/poster.jpg",
            2023);
            
        return Task.FromResult(movieDetails);
    }
}

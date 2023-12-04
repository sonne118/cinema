using ApiApplication.Domain.Common.Models;
using ApiApplication.Domain.TicketAggregate;

namespace ApiApplication.Domain.ShowTimeAggregate
{

    public sealed class Movie : Entity<MovieId>   
    {        
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ImdbId { get; set; }
        public string Stars { get; set; }
        public DateTime ReleaseDate { get; set; }        
        
        private readonly List<ShowTime> _showTimes = new();
        public IReadOnlyList<ShowTime> TicketEntities => _showTimes.AsReadOnly();

        private Movie(Guid Id, string title, string imdbId, string stars, DateTime releaseDate, MovieId? id = null)
            : base(MovieId.CreateUnique())
           //:base(Id)
        {
            Title = title;
            ImdbId = imdbId;
            Stars = stars;
            ReleaseDate = releaseDate;
        }

        public static Movie Create(Guid Id,string title, string imdbId, string stars, DateTime releaseDate)
        {
            // TODO: enforce invariants
            return new Movie(Id,title, imdbId, stars, releaseDate);
        }
        private Movie()
        {
        }
    }

}

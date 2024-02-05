using ApiApplication.Domain.Common.Models;

namespace ApiApplication.Domain.Domain.ShowTimeAggregate
{

    public sealed class Movie : Entity<MovieId>   
    {        
      //  public Guid Id { get; set; }
        public string Title { get; private set; }
        public string ImdbId { get; private set; }
        public string Stars { get; private set; }
        public DateTime ReleaseDate { get; private set; }        
        
        private readonly List<ShowTime> _showTimes = new();
        public IReadOnlyList<ShowTime> TicketEntities => _showTimes.AsReadOnly();

        private Movie(string title, string imdbId, string stars, DateTime releaseDate, MovieId? id = null)
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
            return new Movie(title, imdbId, stars, releaseDate);
        }
        private Movie()
        {
        }
    }

}

using ApiApplication.Domain.Common.Models;
using static ApiApplication.Domain.Common.DomainErrors.Errors;

namespace ApiApplication.Domain.Domain.ShowTimeAggregate
{
    public sealed class Auditorium : Entity<AuditoriumId>
    {       
       // public AuditoriumId auditoriumId { get; set; }

      //  public AuditoriumId Id { get; set; }

        private readonly List<ShowTime> _showTime = new();

        private readonly List<SeatEntity> _seats = new();     

        public IReadOnlyList<ShowTime> ShowTime => _showTime.AsReadOnly();

        public IReadOnlyList<SeatEntity> Seats => _seats.AsReadOnly();

        private Auditorium(List<SeatEntity> seats, List<ShowTime> showTime, AuditoriumId? Id = null)
            : base(Id ?? AuditoriumId.CreateUnique())
        {
            //auditoriumId = AuditoriumId.CreateUnique();
            _showTime = showTime;
            _seats = seats;
        }

        public static Auditorium Create(          
            List<SeatEntity> seats ,
            List<ShowTime> items)
        {
            // TODO: enforce invariants
            return new Auditorium( seats, items );
        }

        private Auditorium()
        {
        }
    }
}
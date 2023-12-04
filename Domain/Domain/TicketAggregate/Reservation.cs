using ApiApplication.Domain.Common.Models;
using ApiApplication.Domain.ShowTimeAggregate;

namespace ApiApplication.Domain.TicketAggregate
{
    public sealed class Reservation : AggregateRoot<ReservationId, Guid>
    {
      //  private readonly List<Reservation> _reservations = new();

        private readonly List<TicketEntity> _ticketEntities = new();
        public int ShowtimeId { get; set; }        
        public ShowTime Showtime { get; set; }      
        //public IReadOnlyList<Reservation> Reservations => _reservations.AsReadOnly();
        public IReadOnlyList<TicketEntity> TicketEntities => _ticketEntities.AsReadOnly();
        public Reservation(
            ReservationId ticketId,
            List<Reservation> reservations,
            List<TicketEntity> ticketEntities,
           // int showtimeId,
           // ICollection<SeatEntity> seats,
           // DateTime createdTime,
           // bool paid,
            ShowTime showtime) : base(ticketId)
        {
            //_reservations = reservations;
            _ticketEntities = ticketEntities;
            //ShowtimeId = showtimeId;
           // Seats = seats;
           // CreatedTime = createdTime;
           // Paid = paid;
            Showtime = showtime;
        }

        public static Reservation Create(
            TicketId ticketId,
            List<Reservation> reservations,
            List<TicketEntity> ticketEntities,
          //  int showtimeId,
           // ICollection<SeatEntity> seats,
           // DateTime createdTime,
           // bool paid,
            ShowTime showtime
          )
        {
            // enforce invariants
            var reserve = new Reservation(
                ReservationId.CreateUnique(),
                reservations,
                ticketEntities,
              //  showtimeId,
              //  seats,
              //  createdTime,
             //   paid,
                showtime
              );

            reserve.AddDomainEvent(new ReservationCreated(reserve));

            return reserve;
        }
        private Reservation()
        {
        }
    }
}
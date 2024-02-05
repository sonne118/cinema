using ApiApplication.Domain.Common.Models;
using System.Collections.Generic;
using static ApiApplication.Domain.Common.DomainErrors.Errors;

namespace ApiApplication.Domain.Domain.ShowTimeAggregate
{
    public sealed class TicketEntity : Entity<TicketId>
    {
        // public TicketStatus TicketStatus { get; private set; }

       // public Guid Id { get; set; }
       // public int ShowtimeId { get; set; }
        public ShowTimeId ShowTimeId { get; set; }
        public ShowTime Showtime { get; set; }
        public ICollection<SeatEntity> SeatEntities { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Paid { get; set; }
        
        public ShowTime ShowTime { get; set; }

        private TicketEntity(ShowTimeId showtimeId, ICollection<SeatEntity> seatEntities, DateTime createdTime, bool paid, ShowTime showtime)
            : base(TicketId.CreateUnique())
        {
            ShowTimeId = showtimeId;
            SeatEntities = seatEntities;
            CreatedTime = createdTime;
            Paid = paid;
            ShowTime = showtime;
        }

        //private TicketEntity(int guestCount)//, TicketStatus ticketStatus
        //    : base(TicketId.CreateUnique())
        //{
        //    //TicketStatus = ticketStatus;
        //}

        public static TicketEntity Create(ShowTimeId showtimeId, ICollection<SeatEntity> seatEntities, DateTime createdTime, bool paid, ShowTime showtime)     //, TicketStatus ticketStatus)
        {
            // TODO: Enforce invariants
            return new TicketEntity(showtimeId, seatEntities,createdTime,paid,showtime);   //, ticketStatus);
        }

        private TicketEntity()
        {
            //CreatedTime = DateTime.Now;
            //Paid = false;
        }
    }

}
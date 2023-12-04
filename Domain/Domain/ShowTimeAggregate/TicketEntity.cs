using ApiApplication.Domain.Common.Models;
using BuberDinner.Domain.DinnerAggregate.Enums;

namespace ApiApplication.Domain.ShowTimeAggregate
{
    public sealed class TicketEntity : Entity<TicketId>
    {
        // public TicketStatus TicketStatus { get; private set; }

       // public Guid Id { get; set; }
        public int ShowtimeId { get; set; }
        public ICollection<SeatEntity> SeatEntities { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Paid { get; set; }
        //public ShowtimeEntity Showtime { get; set; }

        private TicketEntity(int guestCount)//, TicketStatus ticketStatus
            : base(TicketId.CreateUnique())
        {
            //TicketStatus = ticketStatus;
        }

        public static TicketEntity Create(int guestCount)     //, TicketStatus ticketStatus)
        {
            // TODO: Enforce invariants
            return new TicketEntity(guestCount);   //, ticketStatus);
        }

        private TicketEntity()
        {
        }
    }

}
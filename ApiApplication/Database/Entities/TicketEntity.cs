using System;
using System.Collections.Generic;

namespace ApiApplication.Database.Entities
{
    public class TicketEntity1
    {
        public TicketEntity1()
        {
            CreatedTime = DateTime.Now;
            Paid = false;
        }

        public Guid Id { get; set; }
        public int ShowtimeId { get; set; }
        public ICollection<SeatEntity1> Seats { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool Paid { get; set; }
        public ShowtimeEntity Showtime { get; set; }
    }
}

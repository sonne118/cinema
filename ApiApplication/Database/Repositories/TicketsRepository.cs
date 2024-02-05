//using ApiApplication.Database.Entities;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Threading;
//using System;
//using System.Linq;
//using ApiApplication.Database.Repositories.Abstractions;

//namespace ApiApplication.Database.Repositories
//{
//    public class TicketsRepository : ITicketsRepository
//    {
//        //private readonly CinemaContext _context;

//        //public TicketsRepository(CinemaContext context)
//        //{
//        //    _context = context;
//        //}

//        public Task<TicketEntity1> GetAsync(Guid id, CancellationToken cancel)
//        {
//            return _context.Tickets.FirstOrDefaultAsync(x => x.Id == id, cancel);
//        }

//        public async Task<IEnumerable<TicketEntity1>> GetEnrichedAsync(int showtimeId, CancellationToken cancel)
//        {
//            return await _context.Tickets
//                .Include(x => x.Showtime)
//                .Include(x => x.Seats)
//                .Where(x => x.ShowtimeId == showtimeId)
//                .ToListAsync(cancel);
//        }

//        public async Task<TicketEntity1> CreateAsync(ShowtimeEntity showtime, IEnumerable<SeatEntity1> selectedSeats, CancellationToken cancel)
//        {
//            var ticket = _context.Tickets.Add(new TicketEntity1
//            {
//                Showtime = showtime,
//                Seats = new List<SeatEntity1>(selectedSeats)
//            });

//            await _context.SaveChangesAsync(cancel);

//            return ticket.Entity;
//        }

//        public async Task<TicketEntity1> ConfirmPaymentAsync(TicketEntity1 ticket, CancellationToken cancel)
//        {
//            ticket.Paid = true;
//            _context.Update(ticket);
//            await _context.SaveChangesAsync(cancel);
//            return ticket;
//        }
//    }
//}

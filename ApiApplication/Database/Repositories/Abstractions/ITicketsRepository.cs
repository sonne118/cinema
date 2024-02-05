//using ApiApplication.Database.Entities;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ApiApplication.Database.Repositories.Abstractions
//{
//    public interface ITicketsRepository
//    {
//        Task<TicketEntity1> ConfirmPaymentAsync(TicketEntity1 ticket, CancellationToken cancel);
//        Task<TicketEntity1> CreateAsync(ShowtimeEntity showtime, IEnumerable<SeatEntity1> selectedSeats, CancellationToken cancel);
//        Task<TicketEntity1> GetAsync(Guid id, CancellationToken cancel);
//        Task<IEnumerable<TicketEntity1>> GetEnrichedAsync(int showtimeId, CancellationToken cancel);
//    }
//}
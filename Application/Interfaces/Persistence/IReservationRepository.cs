using ApiApplication.Domain.TicketAggregate;

namespace Application.Common.Interfaces.Persistence;

public interface IReservationRepository
{
    Task AddAsync(Reservation dinner);
   //  Task<List<Reservation>> ListAsync(HostId hostId);
}
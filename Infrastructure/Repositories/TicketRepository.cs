//using Microsoft.EntityFrameworkCore;

//namespace ApiApplication.Infrastructure.Persistence.Repositories;

//public class DinnerRepository : IReservationRepository
//{
//    private readonly DbContext _dbContext;

//    public DinnerRepository(DbContext dbContext)
//    {
//        _dbContext = dbContext;
//    }

//    public async Task AddAsync(Reservation dinner)
//    {
//        await _dbContext.AddAsync(dinner);

//        await _dbContext.SaveChangesAsync();
//    }

//    //public async Task<List<Ticket>> ListAsync(HostId hostId)
//    //{
//    //    return await _dbContext.Tickets.Where(dinner => dinner.HostId == hostId).ToListAsync();
//    //}
//}
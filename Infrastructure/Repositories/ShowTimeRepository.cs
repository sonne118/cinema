using ApiApplication.Domain.ShowTimeAggregate;
using ApiApplication.Infrastructure.Persistence;
using Application.Common.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ShowTimeRepository : IShowTimeRepository
{
    private readonly DbContextApi _dbContext;

    public ShowTimeRepository(DbContextApi dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ShowTime menu)
    {
        _dbContext.Add(menu);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(ShowTimeId menuId)
    {
        return await _dbContext.ShowTimes.AnyAsync(menu => menu.Id == menuId);
    }

    public async Task<ShowTime?> GetByIdAsync(ShowTimeId menuId)
    {
        return await _dbContext.ShowTimes.FirstOrDefaultAsync(menu => menu.Id == menuId);
    }

    //public async Task<List<ShowTime>> ListAsync(HostId hostId)
    //{
    //    return await _dbContext.Menus.Where(menu => menu.HostId == hostId).ToListAsync();
    //}

    public async Task UpdateAsync(ShowTime menu)
    {
        _dbContext.ShowTimes.Update(menu);
        await _dbContext.SaveChangesAsync();
    }
}
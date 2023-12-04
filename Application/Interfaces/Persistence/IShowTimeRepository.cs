using ApiApplication.Domain.ShowTimeAggregate;
namespace Application.Common.Interfaces.Persistence;

public interface IShowTimeRepository
{
    Task UpdateAsync(ShowTime menu);
    Task AddAsync(ShowTime menu);
    Task<ShowTime?> GetByIdAsync(ShowTimeId menuId);
    Task<bool> ExistsAsync(ShowTimeId menuId); 
}
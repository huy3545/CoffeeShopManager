using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManager.DAL.Repositories;

public interface IMonRepository : IRepository<Mon>
{
    Task<IEnumerable<Mon>> GetMonsByLoaiAsync(string loai);
}

public class MonRepository : Repository<Mon>, IMonRepository
{
    public MonRepository(CoffeeShopDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Mon>> GetMonsByLoaiAsync(string loai)
    {
        return await _dbSet.Where(m => m.Loai == loai).ToListAsync();
    }
}


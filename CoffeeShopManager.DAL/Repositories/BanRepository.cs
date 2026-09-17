using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManager.DAL.Repositories;

public interface IBanRepository : IRepository<Ban>
{
    Task<IEnumerable<Ban>> GetBansByTrangThaiAsync(string trangThai);
}

public class BanRepository : Repository<Ban>, IBanRepository
{
    public BanRepository(CoffeeShopDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Ban>> GetBansByTrangThaiAsync(string trangThai)
    {
        return await _dbSet.Where(b => b.TrangThai == trangThai).ToListAsync();
    }
}


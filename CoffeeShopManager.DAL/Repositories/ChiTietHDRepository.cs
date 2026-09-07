using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManager.DAL.Repositories;

public interface IChiTietHDRepository : IRepository<ChiTietHD>
{
    Task<IEnumerable<ChiTietHD>> GetChiTietsByHoaDonAsync(int maHD);
}

public class ChiTietHDRepository : Repository<ChiTietHD>, IChiTietHDRepository
{
    public ChiTietHDRepository(CoffeeShopDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ChiTietHD>> GetChiTietsByHoaDonAsync(int maHD)
    {
        return await _dbSet
            .Include(ct => ct.Mon)
            .Where(ct => ct.MaHD == maHD)
            .ToListAsync();
    }
}


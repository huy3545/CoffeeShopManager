using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManager.DAL.Repositories;

public interface IKhuyenMaiRepository : IRepository<KhuyenMai>
{
    Task<bool> HasBeenUsedAsync(int maKM);
    Task<int> CountUsageAsync(int maKM);
}

public class KhuyenMaiRepository : Repository<KhuyenMai>, IKhuyenMaiRepository
{
    public KhuyenMaiRepository(CoffeeShopDbContext context) : base(context)
    {
    }

    public override async Task<IEnumerable<KhuyenMai>> GetAllAsync()
    {
        return await _dbSet.Include(km => km.Mon).OrderByDescending(km => km.NgayTao).ToListAsync();
    }

    public override async Task<KhuyenMai?> GetByIdAsync(int id)
    {
        return await _dbSet.Include(km => km.Mon).FirstOrDefaultAsync(km => km.MaKM == id);
    }

    public Task<bool> HasBeenUsedAsync(int maKM) =>
        _context.HoaDonKhuyenMais.AnyAsync(h => h.MaKM == maKM);

    public Task<int> CountUsageAsync(int maKM) =>
        _context.HoaDonKhuyenMais.CountAsync(h => h.MaKM == maKM);
}

public interface IHoaDonKhuyenMaiRepository : IRepository<HoaDonKhuyenMai>
{
}

public class HoaDonKhuyenMaiRepository : Repository<HoaDonKhuyenMai>, IHoaDonKhuyenMaiRepository
{
    public HoaDonKhuyenMaiRepository(CoffeeShopDbContext context) : base(context)
    {
    }
}

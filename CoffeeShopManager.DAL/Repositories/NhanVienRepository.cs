using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManager.DAL.Repositories;

public interface INhanVienRepository : IRepository<NhanVien>
{
    Task<IEnumerable<NhanVien>> GetNhanViensByCaLamAsync(string caLam);
}

public class NhanVienRepository : Repository<NhanVien>, INhanVienRepository
{
    public NhanVienRepository(CoffeeShopDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<NhanVien>> GetNhanViensByCaLamAsync(string caLam)
    {
        return await _dbSet.Where(nv => nv.CaLam == caLam).ToListAsync();
    }
}


using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManager.DAL.Repositories;

public interface INhatKyHoatDongRepository : IRepository<NhatKyHoatDong>
{
    Task<IEnumerable<NhatKyHoatDong>> SearchAsync(string? searchText, string? action, DateTime? from, DateTime? to);
}

public class NhatKyHoatDongRepository : Repository<NhatKyHoatDong>, INhatKyHoatDongRepository
{
    public NhatKyHoatDongRepository(CoffeeShopDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<NhatKyHoatDong>> SearchAsync(string? searchText, string? action, DateTime? from, DateTime? to)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var text = searchText.Trim();
            query = query.Where(x => (x.TenNguoiDung ?? "").Contains(text) || x.MoTa.Contains(text) || x.LoaiDoiTuong.Contains(text));
        }
        if (!string.IsNullOrWhiteSpace(action)) query = query.Where(x => x.HanhDong == action);
        if (from.HasValue) query = query.Where(x => x.ThoiGianThucHien >= from.Value.Date);
        if (to.HasValue) query = query.Where(x => x.ThoiGianThucHien < to.Value.Date.AddDays(1));
        return await query.OrderByDescending(x => x.ThoiGianThucHien).Take(1000).ToListAsync();
    }
}

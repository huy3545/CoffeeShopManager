using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManager.DAL.Repositories;

public interface IDatBanRepository : IRepository<DatBan>
{
    Task<IEnumerable<DatBan>> GetHistoryAsync();
    Task<bool> HasOverlapAsync(int maBan, DateTime start, DateTime end, int? excludeId = null);
}

public class DatBanRepository : Repository<DatBan>, IDatBanRepository
{
    public DatBanRepository(CoffeeShopDbContext context) : base(context)
    {
    }

    public Task<IEnumerable<DatBan>> GetHistoryAsync() =>
        _dbSet.Include(d => d.Ban).Include(d => d.KhachHang)
            .OrderByDescending(d => d.ThoiGianBatDau)
            .ToListAsync()
            .ContinueWith(t => (IEnumerable<DatBan>)t.Result);

    public Task<bool> HasOverlapAsync(int maBan, DateTime start, DateTime end, int? excludeId = null) =>
        _dbSet.AnyAsync(d => d.MaBan == maBan
            && d.TrangThai != "Đã hủy"
            && d.TrangThai != "Đã hoàn thành"
            && (!excludeId.HasValue || d.MaDatBan != excludeId.Value)
            && d.ThoiGianBatDau < end
            && start < d.ThoiGianKetThuc);
}

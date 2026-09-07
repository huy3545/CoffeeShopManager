using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManager.DAL.Repositories;

public interface IHoaDonRepository : IRepository<HoaDon>
{
    Task<HoaDon?> GetHoaDonWithDetailsAsync(int maHD);
    Task<IEnumerable<HoaDon>> GetHoaDonsByBanAsync(int maBan);
    Task<IEnumerable<HoaDon>> GetHoaDonsByDateRangeAsync(DateTime tuNgay, DateTime denNgay);
    Task<IEnumerable<HoaDon>> GetHoaDonsByTrangThaiAsync(string trangThai);
}

public class HoaDonRepository : Repository<HoaDon>, IHoaDonRepository
{
    public HoaDonRepository(CoffeeShopDbContext context) : base(context)
    {
    }

    public async Task<HoaDon?> GetHoaDonWithDetailsAsync(int maHD)
    {
        return await _dbSet
            .Include(h => h.Ban)
            .Include(h => h.ChiTietHDs)
                .ThenInclude(ct => ct.Mon)
            .Include(h => h.KhachHang)
            .Include(h => h.NhanVien)
            .FirstOrDefaultAsync(h => h.MaHD == maHD);
    }

    public async Task<IEnumerable<HoaDon>> GetHoaDonsByBanAsync(int maBan)
    {
        return await _dbSet
            .Include(h => h.ChiTietHDs)
            .Where(h => h.MaBan == maBan)
            .ToListAsync();
    }

    public async Task<IEnumerable<HoaDon>> GetHoaDonsByDateRangeAsync(DateTime tuNgay, DateTime denNgay)
    {
        return await _dbSet
            .Include(h => h.ChiTietHDs)
            .Where(h => h.ThoiGianTao >= tuNgay && h.ThoiGianTao <= denNgay)
            .ToListAsync();
    }

    public async Task<IEnumerable<HoaDon>> GetHoaDonsByTrangThaiAsync(string trangThai)
    {
        return await _dbSet
            .Include(h => h.Ban)
            .Include(h => h.ChiTietHDs)
            .Where(h => h.TrangThai == trangThai)
            .ToListAsync();
    }
}


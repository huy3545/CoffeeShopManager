using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManager.DAL.Repositories;

public interface IKhachHangRepository : IRepository<KhachHang>
{
    Task<KhachHang?> GetKhachHangBySoDienThoaiAsync(string soDienThoai);
    Task<IEnumerable<KhachHang>> GetTopKhachHangsByDiemAsync(int top);
}

public class KhachHangRepository : Repository<KhachHang>, IKhachHangRepository
{
    public KhachHangRepository(CoffeeShopDbContext context) : base(context)
    {
    }

    public async Task<KhachHang?> GetKhachHangBySoDienThoaiAsync(string soDienThoai)
    {
        return await _dbSet.FirstOrDefaultAsync(kh => kh.SoDienThoai == soDienThoai);
    }

    public async Task<IEnumerable<KhachHang>> GetTopKhachHangsByDiemAsync(int top)
    {
        return await _dbSet
            .OrderByDescending(kh => kh.DiemTichLuy)
            .Take(top)
            .ToListAsync();
    }
}


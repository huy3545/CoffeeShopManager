using CoffeeShopManager.DAL.Data;
using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManager.DAL.Repositories;

public interface ILichLamViecRepository : IRepository<CaLamViec>
{
    Task<IEnumerable<CaLamViec>> GetShiftsAsync(DateTime? from, DateTime? to);
    Task<CaLamViec?> GetShiftWithRegistrationsAsync(int id);
    Task<int> CountApprovedAsync(int maCa);
    Task<bool> HasRegistrationAsync(int maCa, int maNV);
    Task<bool> HasStaffOverlapAsync(int maNV, DateTime start, DateTime end, int? excludeRegistrationId = null);
    Task<NhanVien?> GetEmployeeAsync(int maNV);
}

public interface IDangKyCaRepository : IRepository<DangKyCa>
{
    Task<IEnumerable<DangKyCa>> GetRegistrationsAsync(int? maCa = null, int? maNV = null);
}

public class LichLamViecRepository : Repository<CaLamViec>, ILichLamViecRepository
{
    public LichLamViecRepository(CoffeeShopDbContext context) : base(context) { }

    public Task<IEnumerable<CaLamViec>> GetShiftsAsync(DateTime? from, DateTime? to)
    {
        var query = _dbSet.Include(x => x.DangKyCas).ThenInclude(x => x.NhanVien).AsNoTracking().AsQueryable();
        if (from.HasValue) query = query.Where(x => x.ThoiGianBatDau >= from.Value);
        if (to.HasValue) query = query.Where(x => x.ThoiGianBatDau < to.Value.AddDays(1));
        return query.OrderBy(x => x.ThoiGianBatDau).ToListAsync().ContinueWith(t => (IEnumerable<CaLamViec>)t.Result);
    }

    public Task<CaLamViec?> GetShiftWithRegistrationsAsync(int id) =>
        _dbSet.Include(x => x.DangKyCas).FirstOrDefaultAsync(x => x.MaCa == id);

    public Task<int> CountApprovedAsync(int maCa) =>
        _context.DangKyCas.CountAsync(x => x.MaCa == maCa && x.TrangThai == "Đã duyệt");

    public Task<bool> HasRegistrationAsync(int maCa, int maNV) =>
        _context.DangKyCas.AnyAsync(x => x.MaCa == maCa && x.MaNV == maNV && x.TrangThai != "Đã hủy");

    public Task<bool> HasStaffOverlapAsync(int maNV, DateTime start, DateTime end, int? excludeRegistrationId = null) =>
        _context.DangKyCas.AnyAsync(x => x.MaNV == maNV && x.TrangThai == "Đã duyệt"
            && (!excludeRegistrationId.HasValue || x.MaDangKy != excludeRegistrationId.Value)
            && x.CaLamViec != null
            && x.CaLamViec.ThoiGianBatDau < end
            && start < x.CaLamViec.ThoiGianKetThuc);

    public Task<NhanVien?> GetEmployeeAsync(int maNV) => _context.NhanViens.AsNoTracking().FirstOrDefaultAsync(x => x.MaNV == maNV);
}

public class DangKyCaRepository : Repository<DangKyCa>, IDangKyCaRepository
{
    public DangKyCaRepository(CoffeeShopDbContext context) : base(context) { }

    public Task<IEnumerable<DangKyCa>> GetRegistrationsAsync(int? maCa = null, int? maNV = null)
    {
        var query = _dbSet.Include(x => x.CaLamViec).Include(x => x.NhanVien).AsNoTracking().AsQueryable();
        if (maCa.HasValue) query = query.Where(x => x.MaCa == maCa.Value);
        if (maNV.HasValue) query = query.Where(x => x.MaNV == maNV.Value);
        return query.OrderByDescending(x => x.ThoiGianDangKy).ToListAsync().ContinueWith(t => (IEnumerable<DangKyCa>)t.Result);
    }
}

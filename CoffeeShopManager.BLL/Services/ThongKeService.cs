using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.DAL.Repositories;

namespace CoffeeShopManager.BLL.Services;

public interface IThongKeService
{
    Task<IEnumerable<DoanhThuTheoNgayDto>> GetDoanhThuTheoNgayAsync(DateTime tuNgay, DateTime denNgay);
    Task<IEnumerable<DoanhThuTheoThangDto>> GetDoanhThuTheoThangAsync(int nam);
    Task<IEnumerable<MonBanChayDto>> GetMonBanChayAsync(DateTime tuNgay, DateTime denNgay, int top = 10);
    Task<decimal> GetTongDoanhThuAsync(DateTime tuNgay, DateTime denNgay);
}

public class ThongKeService : IThongKeService
{
    private readonly IHoaDonRepository _hoaDonRepository;
    private readonly IChiTietHDRepository _chiTietRepository;

    public ThongKeService(IHoaDonRepository hoaDonRepository, IChiTietHDRepository chiTietRepository)
    {
        _hoaDonRepository = hoaDonRepository;
        _chiTietRepository = chiTietRepository;
    }

    public async Task<IEnumerable<DoanhThuTheoNgayDto>> GetDoanhThuTheoNgayAsync(DateTime tuNgay, DateTime denNgay)
    {
        try
        {
            var hoaDons = await _hoaDonRepository.GetHoaDonsByDateRangeAsync(tuNgay, denNgay);
            var hoaDonsDaThanhToan = hoaDons.Where(h => h.TrangThai == "Đã thanh toán");

            var result = hoaDonsDaThanhToan
                .GroupBy(h => h.ThoiGianTao.Date)
                .Select(g => new DoanhThuTheoNgayDto
                {
                    Ngay = g.Key,
                    SoHoaDon = g.Count(),
                    TongDoanhThu = g.Sum(h => h.TongTien)
                })
                .OrderBy(d => d.Ngay)
                .ToList();

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi thống kê doanh thu theo ngày: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<DoanhThuTheoThangDto>> GetDoanhThuTheoThangAsync(int nam)
    {
        try
        {
            var tuNgay = new DateTime(nam, 1, 1);
            var denNgay = new DateTime(nam, 12, 31);

            var hoaDons = await _hoaDonRepository.GetHoaDonsByDateRangeAsync(tuNgay, denNgay);
            var hoaDonsDaThanhToan = hoaDons.Where(h => h.TrangThai == "Đã thanh toán");

            var result = hoaDonsDaThanhToan
                .GroupBy(h => h.ThoiGianTao.Month)
                .Select(g => new DoanhThuTheoThangDto
                {
                    Thang = g.Key,
                    Nam = nam,
                    SoHoaDon = g.Count(),
                    TongDoanhThu = g.Sum(h => h.TongTien)
                })
                .OrderBy(d => d.Thang)
                .ToList();

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi thống kê doanh thu theo tháng: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<MonBanChayDto>> GetMonBanChayAsync(DateTime tuNgay, DateTime denNgay, int top = 10)
    {
        try
        {
            var hoaDons = await _hoaDonRepository.GetHoaDonsByDateRangeAsync(tuNgay, denNgay);
            var hoaDonIds = hoaDons.Where(h => h.TrangThai == "Đã thanh toán").Select(h => h.MaHD).ToList();

            var allChiTiets = new List<ChiTietHDDto>();
            foreach (var maHD in hoaDonIds)
            {
                var chiTiets = await _chiTietRepository.GetChiTietsByHoaDonAsync(maHD);
                foreach (var ct in chiTiets)
                {
                    allChiTiets.Add(new ChiTietHDDto
                    {
                        MaMon = ct.MaMon,
                        TenMon = ct.Mon?.TenMon ?? "",
                        SoLuong = ct.SoLuong,
                        ThanhTien = ct.ThanhTien
                    });
                }
            }

            var result = allChiTiets
                .GroupBy(ct => new { ct.MaMon, ct.TenMon })
                .Select(g => new MonBanChayDto
                {
                    MaMon = g.Key.MaMon,
                    TenMon = g.Key.TenMon,
                    SoLuongBan = g.Sum(ct => ct.SoLuong),
                    DoanhThu = g.Sum(ct => ct.ThanhTien)
                })
                .OrderByDescending(m => m.SoLuongBan)
                .Take(top)
                .ToList();

            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi thống kê món bán chạy: {ex.Message}", ex);
        }
    }

    public async Task<decimal> GetTongDoanhThuAsync(DateTime tuNgay, DateTime denNgay)
    {
        try
        {
            var hoaDons = await _hoaDonRepository.GetHoaDonsByDateRangeAsync(tuNgay, denNgay);
            var tongDoanhThu = hoaDons
                .Where(h => h.TrangThai == "Đã thanh toán")
                .Sum(h => h.TongTien);

            return tongDoanhThu;
        }
        catch (Exception ex)
        {
            throw new Exception($"Lỗi khi tính tổng doanh thu: {ex.Message}", ex);
        }
    }
}


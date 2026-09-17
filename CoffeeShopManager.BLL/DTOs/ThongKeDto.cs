namespace CoffeeShopManager.BLL.DTOs;

public class DoanhThuTheoNgayDto
{
    public DateTime Ngay { get; set; }
    public int SoHoaDon { get; set; }
    public decimal TongDoanhThu { get; set; }
}

public class DoanhThuTheoThangDto
{
    public int Thang { get; set; }
    public int Nam { get; set; }
    public int SoHoaDon { get; set; }
    public decimal TongDoanhThu { get; set; }
}

public class MonBanChayDto
{
    public int MaMon { get; set; }
    public string TenMon { get; set; } = string.Empty;
    public int SoLuongBan { get; set; }
    public decimal DoanhThu { get; set; }
}

public class DashboardDoanhThuDto
{
    public decimal TongDoanhThu { get; set; }
    public int SoHoaDon { get; set; }
    public decimal GiaTriTrungBinhHoaDon { get; set; }
    public decimal TienGiamKhuyenMai { get; set; }
    public decimal TienGiamTuDiem { get; set; }
    public IEnumerable<DoanhThuTheoNgayDto> TheoNgay { get; set; } = Array.Empty<DoanhThuTheoNgayDto>();
    public IEnumerable<DoanhThuTheoThangDto> TheoThang { get; set; } = Array.Empty<DoanhThuTheoThangDto>();
    public IEnumerable<MonBanChayDto> MonBanChay { get; set; } = Array.Empty<MonBanChayDto>();
    public IEnumerable<PhuongThucThanhToanDto> TheoPhuongThucThanhToan { get; set; } = Array.Empty<PhuongThucThanhToanDto>();
}

public class PhuongThucThanhToanDto
{
    public string PhuongThuc { get; set; } = "Chưa xác định";
    public int SoHoaDon { get; set; }
    public decimal DoanhThu { get; set; }
}

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


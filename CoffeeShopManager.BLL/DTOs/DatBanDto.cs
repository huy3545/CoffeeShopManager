namespace CoffeeShopManager.BLL.DTOs;

public class DatBanDto
{
    public int MaDatBan { get; set; }
    public int MaBan { get; set; }
    public string TenBan { get; set; } = string.Empty;
    public int MaKH { get; set; }
    public string TenKhachHang { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public DateTime ThoiGianBatDau { get; set; }
    public DateTime ThoiGianKetThuc { get; set; }
    public int SoLuongKhach { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string? GhiChu { get; set; }
    public DateTime NgayTao { get; set; }
}

public class CreateDatBanDto
{
    public int MaBan { get; set; }
    public int MaKH { get; set; }
    public DateTime ThoiGianBatDau { get; set; }
    public DateTime ThoiGianKetThuc { get; set; }
    public int SoLuongKhach { get; set; }
    public string? GhiChu { get; set; }
    public string? NguoiTao { get; set; }
}

public class UpdateDatBanDto
{
    public int MaBan { get; set; }
    public int MaKH { get; set; }
    public DateTime ThoiGianBatDau { get; set; }
    public DateTime ThoiGianKetThuc { get; set; }
    public int SoLuongKhach { get; set; }
    public string? GhiChu { get; set; }
}

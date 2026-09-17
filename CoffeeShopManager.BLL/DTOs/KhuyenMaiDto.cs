namespace CoffeeShopManager.BLL.DTOs;

public class KhuyenMaiDto
{
    public int MaKM { get; set; }
    public string TenKhuyenMai { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public string LoaiKhuyenMai { get; set; } = string.Empty;
    public decimal GiaTriGiam { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public decimal HoaDonToiThieu { get; set; }
    public int? MaMon { get; set; }
    public string? TenMon { get; set; }
    public string? LoaiMon { get; set; }
    public int? SoLanSuDungToiDa { get; set; }
    public int SoLanDaSuDung { get; set; }
    public bool DaKichHoat { get; set; }
    public DateTime NgayTao { get; set; }
    public string TrangThai { get; set; } = string.Empty;
}

public class CreateKhuyenMaiDto
{
    public string TenKhuyenMai { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public string LoaiKhuyenMai { get; set; } = string.Empty;
    public decimal GiaTriGiam { get; set; }
    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }
    public decimal HoaDonToiThieu { get; set; }
    public int? MaMon { get; set; }
    public string? LoaiMon { get; set; }
    public int? SoLanSuDungToiDa { get; set; }
    public bool DaKichHoat { get; set; } = true;
    public string? MaNguoiTao { get; set; }
}

public class UpdateKhuyenMaiDto : CreateKhuyenMaiDto
{
}

public class ApDungKhuyenMaiDto
{
    public int MaKM { get; set; }
    public string TenKhuyenMai { get; set; } = string.Empty;
    public decimal TongTienTruocGiam { get; set; }
    public decimal SoTienGiam { get; set; }
    public decimal TongTienSauGiam { get; set; }
}

namespace CoffeeShopManager.BLL.DTOs;

public class NhatKyHoatDongDto
{
    public long MaNhatKy { get; set; }
    public string? MaNguoiDung { get; set; }
    public string? TenNguoiDung { get; set; }
    public string? VaiTro { get; set; }
    public string HanhDong { get; set; } = string.Empty;
    public string LoaiDoiTuong { get; set; } = string.Empty;
    public string? MaDoiTuong { get; set; }
    public string MoTa { get; set; } = string.Empty;
    public DateTime ThoiGianThucHien { get; set; }
    public string? DiaChiIP { get; set; }
    public string KetQua { get; set; } = string.Empty;
    public string? DuLieuCu { get; set; }
    public string? DuLieuMoi { get; set; }
}

public class GhiNhatKyDto
{
    public string? MaNguoiDung { get; set; }
    public string? TenNguoiDung { get; set; }
    public string? VaiTro { get; set; }
    public string HanhDong { get; set; } = string.Empty;
    public string LoaiDoiTuong { get; set; } = string.Empty;
    public string? MaDoiTuong { get; set; }
    public string MoTa { get; set; } = string.Empty;
    public string? DiaChiIP { get; set; }
    public string KetQua { get; set; } = "Thành công";
    public string? DuLieuCu { get; set; }
    public string? DuLieuMoi { get; set; }
}

namespace CoffeeShopManager.BLL.DTOs;

public class CaLamViecDto
{
    public int MaCa { get; set; }
    public string TenCa { get; set; } = string.Empty;
    public DateTime ThoiGianBatDau { get; set; }
    public DateTime ThoiGianKetThuc { get; set; }
    public string LoaiCa { get; set; } = string.Empty;
    public string LoaiNhanVien { get; set; } = string.Empty;
    public int SoLuongNhanVienCan { get; set; }
    public int SoLuongDaDuyet { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string? NguoiTao { get; set; }
    public string? GhiChu { get; set; }
    public List<DangKyCaDto> DangKyCas { get; set; } = new();
}

public class DangKyCaDto
{
    public int MaDangKy { get; set; }
    public int MaCa { get; set; }
    public int MaNV { get; set; }
    public string TenNhanVien { get; set; } = string.Empty;
    public DateTime ThoiGianDangKy { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string? GhiChu { get; set; }
    public string? NguoiDuyet { get; set; }
}

public class CreateCaLamViecDto
{
    public string TenCa { get; set; } = string.Empty;
    public DateTime ThoiGianBatDau { get; set; }
    public DateTime ThoiGianKetThuc { get; set; }
    public string LoaiCa { get; set; } = "Tùy chỉnh";
    public string LoaiNhanVien { get; set; } = "Part-time";
    public int SoLuongNhanVienCan { get; set; }
    public string? NguoiTao { get; set; }
    public string? GhiChu { get; set; }
}

public class UpdateCaLamViecDto : CreateCaLamViecDto { }

public class DangKyCaRequestDto
{
    public int MaCa { get; set; }
    public int MaNV { get; set; }
    public string? MaNguoiDung { get; set; }
    public string? GhiChu { get; set; }
}

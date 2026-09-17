namespace CoffeeShopManager.BLL.DTOs;

public class HoaDonDto
{
    public int MaHD { get; set; }
    public int MaBan { get; set; }
    public string TenBan { get; set; } = string.Empty;
    public DateTime ThoiGianTao { get; set; }
    public decimal TongTien { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string? PhuongThucThanhToan { get; set; }
    public string? TenKH { get; set; } // Tên khách hàng từ hóa đơn (nếu không có MaKH)
    public int? MaKH { get; set; }
    public string? TenKHFromCustomer { get; set; } // Tên khách hàng từ bảng KhachHang (nếu có MaKH)
    public int? MaNV { get; set; }
    public string? TenNV { get; set; }
    public decimal TienGiamTuDiem { get; set; } = 0; // Số tiền được giảm từ điểm
    public int DiemSuDung { get; set; } = 0; // Số điểm đã sử dụng
    public decimal TienGiamKhuyenMai { get; set; } = 0;
    public int? MaKM { get; set; }
    public List<ChiTietHDDto> ChiTiets { get; set; } = new();
}

public class CreateHoaDonDto
{
    public int MaBan { get; set; }
    public int? MaKH { get; set; }
    public int? MaNV { get; set; }
}

public class UpdateHoaDonDto
{
    public string? TrangThai { get; set; }
    public int? MaKH { get; set; }
    public int? MaNV { get; set; }
}

public class GoiMonDto
{
    public int MaHD { get; set; }
    public int MaMon { get; set; }
    public int SoLuong { get; set; }
}

public class ThanhToanDto
{
    public int MaHD { get; set; }
    public int? MaKH { get; set; }
    public string PhuongThucThanhToan { get; set; } = "Tiền mặt"; // Tiền mặt, Qua ngân hàng
    public string? SoDienThoai { get; set; }
    public string? TenKH { get; set; }
    public int DiemSuDung { get; set; } = 0; // Số điểm muốn sử dụng để thanh toán
    public int? MaKM { get; set; }
}

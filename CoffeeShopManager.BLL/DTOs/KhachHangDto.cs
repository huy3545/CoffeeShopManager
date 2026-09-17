namespace CoffeeShopManager.BLL.DTOs;

public class KhachHangDto
{
    public int MaKH { get; set; }
    public string TenKH { get; set; } = string.Empty;
    public int DiemTichLuy { get; set; }
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
}

public class CreateKhachHangDto
{
    public string TenKH { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
}

public class UpdateKhachHangDto
{
    public string? TenKH { get; set; }
    public int? DiemTichLuy { get; set; }
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
}


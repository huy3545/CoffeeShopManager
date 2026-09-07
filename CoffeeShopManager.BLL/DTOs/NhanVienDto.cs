namespace CoffeeShopManager.BLL.DTOs;

public class NhanVienDto
{
    public int MaNV { get; set; }
    public string TenNV { get; set; } = string.Empty;
    public string CaLam { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
}

public class CreateNhanVienDto
{
    public string TenNV { get; set; } = string.Empty;
    public string CaLam { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
}

public class UpdateNhanVienDto
{
    public string? TenNV { get; set; }
    public string? CaLam { get; set; }
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
}


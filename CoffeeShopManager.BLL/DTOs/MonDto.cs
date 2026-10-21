namespace CoffeeShopManager.BLL.DTOs;

public class MonDto
{
    public int MaMon { get; set; }
    public string TenMon { get; set; } = string.Empty;
    public decimal DonGia { get; set; }
    public string Loai { get; set; } = string.Empty;
    public string? HinhAnh { get; set; }
}

public class CreateMonDto
{
    public string TenMon { get; set; } = string.Empty;
    public decimal DonGia { get; set; }
    public string Loai { get; set; } = string.Empty;
    public string? HinhAnh { get; set; }
}

public class UpdateMonDto
{
    public string? TenMon { get; set; }
    public decimal? DonGia { get; set; }
    public string? Loai { get; set; }
    public string? HinhAnh { get; set; }
}

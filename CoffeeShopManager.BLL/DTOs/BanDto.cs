namespace CoffeeShopManager.BLL.DTOs;

public class BanDto
{
    public int MaBan { get; set; }
    public string TenBan { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
}

public class CreateBanDto
{
    public string TenBan { get; set; } = string.Empty;
    public string TrangThai { get; set; } = "Trống";
}

public class UpdateBanDto
{
    public string? TenBan { get; set; }
    public string? TrangThai { get; set; }
}


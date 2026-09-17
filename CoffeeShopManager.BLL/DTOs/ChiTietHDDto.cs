namespace CoffeeShopManager.BLL.DTOs;

public class ChiTietHDDto
{
    public int MaChiTiet { get; set; }
    public int MaHD { get; set; }
    public int MaMon { get; set; }
    public string TenMon { get; set; } = string.Empty;
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal ThanhTien { get; set; }
}


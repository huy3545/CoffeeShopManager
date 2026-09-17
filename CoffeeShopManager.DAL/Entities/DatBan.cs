using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("DatBan")]
public class DatBan
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaDatBan { get; set; }

    public int MaBan { get; set; }
    public int MaKH { get; set; }

    [Required, MaxLength(200)]
    public string TenKhachHang { get; set; } = string.Empty;

    [MaxLength(15)]
    public string? SoDienThoai { get; set; }

    public DateTime ThoiGianBatDau { get; set; }
    public DateTime ThoiGianKetThuc { get; set; }
    public int SoLuongKhach { get; set; }

    [Required, MaxLength(50)]
    public string TrangThai { get; set; } = "Chờ xác nhận";

    [MaxLength(500)]
    public string? GhiChu { get; set; }

    public DateTime NgayTao { get; set; } = DateTime.Now;

    [MaxLength(450)]
    public string? NguoiTao { get; set; }

    public virtual Ban? Ban { get; set; }
    public virtual KhachHang? KhachHang { get; set; }
}

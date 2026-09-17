using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("NhatKyHoatDong")]
public class NhatKyHoatDong
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long MaNhatKy { get; set; }

    [MaxLength(450)]
    public string? MaNguoiDung { get; set; }

    [MaxLength(256)]
    public string? TenNguoiDung { get; set; }

    [MaxLength(50)]
    public string? VaiTro { get; set; }

    [Required, MaxLength(100)]
    public string HanhDong { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string LoaiDoiTuong { get; set; } = string.Empty;

    public string? MaDoiTuong { get; set; }

    [Required, MaxLength(1000)]
    public string MoTa { get; set; } = string.Empty;

    public DateTime ThoiGianThucHien { get; set; } = DateTime.Now;

    [MaxLength(100)]
    public string? DiaChiIP { get; set; }

    [Required, MaxLength(30)]
    public string KetQua { get; set; } = "Thành công";

    public string? DuLieuCu { get; set; }
    public string? DuLieuMoi { get; set; }
}

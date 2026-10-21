using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("CaLamViec")]
public class CaLamViec
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaCa { get; set; }

    [Required, MaxLength(100)]
    public string TenCa { get; set; } = string.Empty;

    public DateTime ThoiGianBatDau { get; set; }
    public DateTime ThoiGianKetThuc { get; set; }

    [Required, MaxLength(30)]
    public string LoaiCa { get; set; } = "Tùy chỉnh";

    [Required, MaxLength(20)]
    public string LoaiNhanVien { get; set; } = "Part-time";

    public int SoLuongNhanVienCan { get; set; }

    [Required, MaxLength(30)]
    public string TrangThai { get; set; } = "Đang mở";

    [MaxLength(450)]
    public string? NguoiTao { get; set; }

    [MaxLength(500)]
    public string? GhiChu { get; set; }

    public DateTime NgayTao { get; set; } = DateTime.Now;

    public virtual ICollection<DangKyCa> DangKyCas { get; set; } = new List<DangKyCa>();
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("DangKyCa")]
public class DangKyCa
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaDangKy { get; set; }

    public int MaCa { get; set; }
    public int MaNV { get; set; }

    [MaxLength(450)]
    public string? MaNguoiDung { get; set; }

    public DateTime ThoiGianDangKy { get; set; } = DateTime.Now;

    [Required, MaxLength(30)]
    public string TrangThai { get; set; } = "Chờ duyệt";

    [MaxLength(500)]
    public string? GhiChu { get; set; }

    [MaxLength(450)]
    public string? NguoiDuyet { get; set; }

    public DateTime? ThoiGianDuyet { get; set; }

    public virtual CaLamViec? CaLamViec { get; set; }
    public virtual NhanVien? NhanVien { get; set; }
}

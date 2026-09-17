using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("KhuyenMai")]
public class KhuyenMai
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaKM { get; set; }

    [Required, MaxLength(200)]
    public string TenKhuyenMai { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? MoTa { get; set; }

    [Required, MaxLength(50)]
    public string LoaiKhuyenMai { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal GiaTriGiam { get; set; }

    public DateTime NgayBatDau { get; set; }
    public DateTime NgayKetThuc { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal HoaDonToiThieu { get; set; }

    public int? MaMon { get; set; }

    [MaxLength(100)]
    public string? LoaiMon { get; set; }

    public int? SoLanSuDungToiDa { get; set; }
    public int SoLanDaSuDung { get; set; }
    public bool DaKichHoat { get; set; } = true;
    public DateTime NgayTao { get; set; } = DateTime.Now;

    [MaxLength(450)]
    public string? MaNguoiTao { get; set; }

    public virtual Mon? Mon { get; set; }
    public virtual ICollection<HoaDonKhuyenMai> HoaDonKhuyenMais { get; set; } = new List<HoaDonKhuyenMai>();
}

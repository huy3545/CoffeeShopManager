using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("HoaDonKhuyenMai")]
public class HoaDonKhuyenMai
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaHDKM { get; set; }

    public int MaHD { get; set; }
    public int MaKM { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SoTienGiam { get; set; }

    public DateTime ThoiGianApDung { get; set; } = DateTime.Now;

    public virtual HoaDon? HoaDon { get; set; }
    public virtual KhuyenMai? KhuyenMai { get; set; }
}

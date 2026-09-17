using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("KhachHang")]
public class KhachHang
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaKH { get; set; }

    [Required]
    [MaxLength(200)]
    public string TenKH { get; set; } = string.Empty;

    [Required]
    public int DiemTichLuy { get; set; } = 0;

    [MaxLength(15)]
    public string? SoDienThoai { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    // Navigation properties
    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
    public virtual ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();
}

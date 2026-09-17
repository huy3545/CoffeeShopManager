using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("NhanVien")]
public class NhanVien
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaNV { get; set; }

    [Required]
    [MaxLength(200)]
    public string TenNV { get; set; } = string.Empty;

    [MaxLength(100)]
    public string CaLam { get; set; } = string.Empty; // Sáng, Chiều, Tối

    [MaxLength(15)]
    public string? SoDienThoai { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    // Navigation properties
    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("Mon")]
public class Mon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaMon { get; set; }

    [Required]
    [MaxLength(200)]
    public string TenMon { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal DonGia { get; set; }

    [MaxLength(100)]
    public string Loai { get; set; } = string.Empty; // Cà phê, Trà, Nước ép, Đồ ăn nhẹ, etc.

    // Navigation properties
    public virtual ICollection<ChiTietHD> ChiTietHDs { get; set; } = new List<ChiTietHD>();
}


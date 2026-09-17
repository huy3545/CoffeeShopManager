using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("ChiTietHD")]
public class ChiTietHD
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaChiTiet { get; set; }

    [Required]
    public int MaHD { get; set; }

    [Required]
    public int MaMon { get; set; }

    [Required]
    public int SoLuong { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ThanhTien { get; set; }

    // Navigation properties
    [ForeignKey("MaHD")]
    public virtual HoaDon? HoaDon { get; set; }

    [ForeignKey("MaMon")]
    public virtual Mon? Mon { get; set; }
}


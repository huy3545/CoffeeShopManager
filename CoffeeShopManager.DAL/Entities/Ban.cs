using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("Ban")]
public class Ban
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaBan { get; set; }

    [Required]
    [MaxLength(100)]
    public string TenBan { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string TrangThai { get; set; } = "Trống"; // Trống, Đang sử dụng, Đã đặt

    // Navigation properties
    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}


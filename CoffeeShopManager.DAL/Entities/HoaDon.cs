using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopManager.DAL.Entities;

[Table("HoaDon")]
public class HoaDon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MaHD { get; set; }

    [Required]
    public int MaBan { get; set; }

    [Required]
    public DateTime ThoiGianTao { get; set; } = DateTime.Now;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TongTien { get; set; }

    [Required]
    [MaxLength(50)]
    public string TrangThai { get; set; } = "Chưa thanh toán"; // Chưa thanh toán, Đã thanh toán, Đã hủy

    [MaxLength(50)]
    public string? PhuongThucThanhToan { get; set; } // Tiền mặt, Qua ngân hàng

    [MaxLength(200)]
    public string? TenKH { get; set; } // Tên khách hàng (lưu trực tiếp, không cần tạo khách hàng nếu không có số điện thoại)

    public int? MaKH { get; set; }

    public int? MaNV { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TienGiamTuDiem { get; set; } = 0; // Số tiền được giảm từ điểm (100 điểm = 10,000 VNĐ)

    public int DiemSuDung { get; set; } = 0; // Số điểm đã sử dụng để thanh toán

    // Navigation properties
    [ForeignKey("MaBan")]
    public virtual Ban? Ban { get; set; }

    [ForeignKey("MaKH")]
    public virtual KhachHang? KhachHang { get; set; }

    [ForeignKey("MaNV")]
    public virtual NhanVien? NhanVien { get; set; }

    public virtual ICollection<ChiTietHD> ChiTietHDs { get; set; } = new List<ChiTietHD>();
}


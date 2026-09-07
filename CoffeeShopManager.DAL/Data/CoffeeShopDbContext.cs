using CoffeeShopManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopManager.DAL.Data;

public class CoffeeShopDbContext : DbContext
{
    public CoffeeShopDbContext(DbContextOptions<CoffeeShopDbContext> options) : base(options)
    {
    }

    public DbSet<Ban> Bans { get; set; }
    public DbSet<Mon> Mons { get; set; }
    public DbSet<HoaDon> HoaDons { get; set; }
    public DbSet<ChiTietHD> ChiTietHDs { get; set; }
    public DbSet<NhanVien> NhanViens { get; set; }
    public DbSet<KhachHang> KhachHangs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<HoaDon>()
            .HasOne(h => h.Ban)
            .WithMany(b => b.HoaDons)
            .HasForeignKey(h => h.MaBan)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HoaDon>()
            .HasOne(h => h.KhachHang)
            .WithMany(k => k.HoaDons)
            .HasForeignKey(h => h.MaKH)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<HoaDon>()
            .HasOne(h => h.NhanVien)
            .WithMany(n => n.HoaDons)
            .HasForeignKey(h => h.MaNV)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ChiTietHD>()
            .HasOne(ct => ct.HoaDon)
            .WithMany(h => h.ChiTietHDs)
            .HasForeignKey(ct => ct.MaHD)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChiTietHD>()
            .HasOne(ct => ct.Mon)
            .WithMany(m => m.ChiTietHDs)
            .HasForeignKey(ct => ct.MaMon)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Bàn
        modelBuilder.Entity<Ban>().HasData(
            new Ban { MaBan = 1, TenBan = "Bàn 1", TrangThai = "Trống" },
            new Ban { MaBan = 2, TenBan = "Bàn 2", TrangThai = "Trống" },
            new Ban { MaBan = 3, TenBan = "Bàn 3", TrangThai = "Trống" },
            new Ban { MaBan = 4, TenBan = "Bàn 4", TrangThai = "Trống" },
            new Ban { MaBan = 5, TenBan = "Bàn 5", TrangThai = "Trống" },
            new Ban { MaBan = 6, TenBan = "Bàn VIP", TrangThai = "Trống" }
        );

        // Seed Món
        modelBuilder.Entity<Mon>().HasData(
            new Mon { MaMon = 1, TenMon = "Cà phê đen", DonGia = 25000, Loai = "Cà phê" },
            new Mon { MaMon = 2, TenMon = "Cà phê sữa", DonGia = 30000, Loai = "Cà phê" },
            new Mon { MaMon = 3, TenMon = "Bạc xỉu", DonGia = 30000, Loai = "Cà phê" },
            new Mon { MaMon = 4, TenMon = "Cappuccino", DonGia = 45000, Loai = "Cà phê" },
            new Mon { MaMon = 5, TenMon = "Trà đào cam sả", DonGia = 35000, Loai = "Trà" },
            new Mon { MaMon = 6, TenMon = "Trà sữa trân châu", DonGia = 40000, Loai = "Trà" },
            new Mon { MaMon = 7, TenMon = "Nước cam ép", DonGia = 35000, Loai = "Nước ép" },
            new Mon { MaMon = 8, TenMon = "Sinh tố bơ", DonGia = 40000, Loai = "Nước ép" },
            new Mon { MaMon = 9, TenMon = "Bánh mì", DonGia = 20000, Loai = "Đồ ăn nhẹ" },
            new Mon { MaMon = 10, TenMon = "Bánh ngọt", DonGia = 25000, Loai = "Đồ ăn nhẹ" }
        );

        // Seed Nhân viên
        modelBuilder.Entity<NhanVien>().HasData(
            new NhanVien { MaNV = 1, TenNV = "Nguyễn Văn A", CaLam = "Sáng", SoDienThoai = "0901234567", Email = "nva@coffee.com" },
            new NhanVien { MaNV = 2, TenNV = "Trần Thị B", CaLam = "Chiều", SoDienThoai = "0902345678", Email = "ttb@coffee.com" },
            new NhanVien { MaNV = 3, TenNV = "Lê Văn C", CaLam = "Tối", SoDienThoai = "0903456789", Email = "lvc@coffee.com" }
        );

        // Seed Khách hàng
        modelBuilder.Entity<KhachHang>().HasData(
            new KhachHang { MaKH = 1, TenKH = "Khách lẻ", DiemTichLuy = 0, SoDienThoai = "", Email = "" },
            new KhachHang { MaKH = 2, TenKH = "Phạm Văn D", DiemTichLuy = 100, SoDienThoai = "0904567890", Email = "pvd@gmail.com" },
            new KhachHang { MaKH = 3, TenKH = "Hoàng Thị E", DiemTichLuy = 250, SoDienThoai = "0905678901", Email = "hte@gmail.com" }
        );
    }
}


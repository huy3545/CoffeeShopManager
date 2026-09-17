using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;
using Moq;
using Xunit;

namespace CoffeeShopManager.Tests.Services;

public class KhuyenMaiServiceTests
{
    [Fact]
    public async Task CalculateDiscountAsync_PercentagePromotion_ReturnsDiscount()
    {
        var promotionRepository = new Mock<IKhuyenMaiRepository>();
        var invoiceRepository = new Mock<IHoaDonRepository>();
        var promotion = new KhuyenMai
        {
            MaKM = 1,
            TenKhuyenMai = "Giảm 10%",
            LoaiKhuyenMai = KhuyenMaiService.PhanTram,
            GiaTriGiam = 10,
            NgayBatDau = DateTime.Now.AddDays(-1),
            NgayKetThuc = DateTime.Now.AddDays(1),
            DaKichHoat = true
        };
        var invoice = new HoaDon
        {
            MaHD = 10,
            TrangThai = "Chưa thanh toán",
            ChiTietHDs = new List<ChiTietHD> { new() { ThanhTien = 100_000 } }
        };

        promotionRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(promotion);
        invoiceRepository.Setup(r => r.GetHoaDonWithDetailsAsync(10)).ReturnsAsync(invoice);
        var service = new KhuyenMaiService(promotionRepository.Object, invoiceRepository.Object);

        var result = await service.CalculateDiscountAsync(1, 10);

        Assert.Equal(10_000m, result.SoTienGiam);
        Assert.Equal(90_000m, result.TongTienSauGiam);
    }

    [Fact]
    public async Task CalculateDiscountAsync_ExpiredPromotion_Throws()
    {
        var promotionRepository = new Mock<IKhuyenMaiRepository>();
        var invoiceRepository = new Mock<IHoaDonRepository>();
        promotionRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new KhuyenMai
        {
            MaKM = 1,
            TenKhuyenMai = "Đã hết hạn",
            LoaiKhuyenMai = KhuyenMaiService.SoTien,
            GiaTriGiam = 10_000,
            NgayBatDau = DateTime.Now.AddDays(-3),
            NgayKetThuc = DateTime.Now.AddDays(-1),
            DaKichHoat = true
        });
        invoiceRepository.Setup(r => r.GetHoaDonWithDetailsAsync(10)).ReturnsAsync(new HoaDon
        {
            MaHD = 10,
            TrangThai = "Chưa thanh toán",
            ChiTietHDs = new List<ChiTietHD> { new() { ThanhTien = 100_000 } }
        });
        var service = new KhuyenMaiService(promotionRepository.Object, invoiceRepository.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CalculateDiscountAsync(1, 10));
    }

    [Fact]
    public async Task CreateAsync_PercentageOver100_Throws()
    {
        var promotionRepository = new Mock<IKhuyenMaiRepository>();
        var invoiceRepository = new Mock<IHoaDonRepository>();
        var service = new KhuyenMaiService(promotionRepository.Object, invoiceRepository.Object);
        var dto = new CreateKhuyenMaiDto
        {
            TenKhuyenMai = "Không hợp lệ",
            LoaiKhuyenMai = KhuyenMaiService.PhanTram,
            GiaTriGiam = 101,
            NgayBatDau = DateTime.Today,
            NgayKetThuc = DateTime.Today.AddDays(1)
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(dto));
    }
}

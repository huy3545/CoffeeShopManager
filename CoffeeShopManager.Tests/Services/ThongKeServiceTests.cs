using CoffeeShopManager.BLL.Services;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;
using Moq;
using Xunit;

namespace CoffeeShopManager.Tests.Services;

public class ThongKeServiceTests
{
    private readonly Mock<IHoaDonRepository> _mockHoaDonRepository;
    private readonly Mock<IChiTietHDRepository> _mockChiTietRepository;
    private readonly ThongKeService _thongKeService;

    public ThongKeServiceTests()
    {
        _mockHoaDonRepository = new Mock<IHoaDonRepository>();
        _mockChiTietRepository = new Mock<IChiTietHDRepository>();
        _thongKeService = new ThongKeService(_mockHoaDonRepository.Object, _mockChiTietRepository.Object);
    }

    [Fact]
    public async Task GetTongDoanhThuAsync_CalculatesCorrectly()
    {
        // Arrange
        var tuNgay = new DateTime(2024, 1, 1);
        var denNgay = new DateTime(2024, 1, 31);

        var hoaDons = new List<HoaDon>
        {
            new HoaDon { MaHD = 1, TongTien = 100000, TrangThai = "Đã thanh toán", ThoiGianTao = new DateTime(2024, 1, 5) },
            new HoaDon { MaHD = 2, TongTien = 150000, TrangThai = "Đã thanh toán", ThoiGianTao = new DateTime(2024, 1, 10) },
            new HoaDon { MaHD = 3, TongTien = 80000, TrangThai = "Chưa thanh toán", ThoiGianTao = new DateTime(2024, 1, 15) }
        };

        _mockHoaDonRepository.Setup(repo => repo.GetHoaDonsByDateRangeAsync(tuNgay, denNgay)).ReturnsAsync(hoaDons);

        // Act
        var result = await _thongKeService.GetTongDoanhThuAsync(tuNgay, denNgay);

        // Assert
        Assert.Equal(250000, result); // Chỉ tính hóa đơn đã thanh toán
    }

    [Fact]
    public async Task GetDoanhThuTheoNgayAsync_GroupsCorrectly()
    {
        // Arrange
        var tuNgay = new DateTime(2024, 1, 1);
        var denNgay = new DateTime(2024, 1, 3);

        var hoaDons = new List<HoaDon>
        {
            new HoaDon { MaHD = 1, TongTien = 100000, TrangThai = "Đã thanh toán", ThoiGianTao = new DateTime(2024, 1, 1, 10, 0, 0) },
            new HoaDon { MaHD = 2, TongTien = 150000, TrangThai = "Đã thanh toán", ThoiGianTao = new DateTime(2024, 1, 1, 14, 0, 0) },
            new HoaDon { MaHD = 3, TongTien = 80000, TrangThai = "Đã thanh toán", ThoiGianTao = new DateTime(2024, 1, 2, 11, 0, 0) }
        };

        _mockHoaDonRepository.Setup(repo => repo.GetHoaDonsByDateRangeAsync(tuNgay, denNgay)).ReturnsAsync(hoaDons);

        // Act
        var result = await _thongKeService.GetDoanhThuTheoNgayAsync(tuNgay, denNgay);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal(250000, resultList[0].TongDoanhThu); // Ngày 1/1
        Assert.Equal(80000, resultList[1].TongDoanhThu);  // Ngày 2/1
    }

    [Fact]
    public async Task GetDoanhThuTheoThangAsync_GroupsByMonth()
    {
        // Arrange
        var nam = 2024;
        var tuNgay = new DateTime(2024, 1, 1);
        var denNgay = new DateTime(2024, 12, 31);

        var hoaDons = new List<HoaDon>
        {
            new HoaDon { MaHD = 1, TongTien = 100000, TrangThai = "Đã thanh toán", ThoiGianTao = new DateTime(2024, 1, 5) },
            new HoaDon { MaHD = 2, TongTien = 150000, TrangThai = "Đã thanh toán", ThoiGianTao = new DateTime(2024, 1, 10) },
            new HoaDon { MaHD = 3, TongTien = 200000, TrangThai = "Đã thanh toán", ThoiGianTao = new DateTime(2024, 2, 5) }
        };

        _mockHoaDonRepository.Setup(repo => repo.GetHoaDonsByDateRangeAsync(tuNgay, denNgay)).ReturnsAsync(hoaDons);

        // Act
        var result = await _thongKeService.GetDoanhThuTheoThangAsync(nam);

        // Assert
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Equal(1, resultList[0].Thang);
        Assert.Equal(250000, resultList[0].TongDoanhThu);
        Assert.Equal(2, resultList[1].Thang);
        Assert.Equal(200000, resultList[1].TongDoanhThu);
    }
}


using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;
using Moq;
using Xunit;

namespace CoffeeShopManager.Tests.Services;

public class HoaDonServiceTests
{
    private readonly Mock<IHoaDonRepository> _mockHoaDonRepository;
    private readonly Mock<IBanRepository> _mockBanRepository;
    private readonly Mock<IMonRepository> _mockMonRepository;
    private readonly Mock<IChiTietHDRepository> _mockChiTietRepository;
    private readonly Mock<IKhachHangRepository> _mockKhachHangRepository;
    private readonly Mock<IKhuyenMaiService> _mockKhuyenMaiService;
    private readonly Mock<IKhuyenMaiRepository> _mockKhuyenMaiRepository;
    private readonly Mock<IHoaDonKhuyenMaiRepository> _mockHoaDonKhuyenMaiRepository;
    private readonly HoaDonService _hoaDonService;

    public HoaDonServiceTests()
    {
        _mockHoaDonRepository = new Mock<IHoaDonRepository>();
        _mockBanRepository = new Mock<IBanRepository>();
        _mockMonRepository = new Mock<IMonRepository>();
        _mockChiTietRepository = new Mock<IChiTietHDRepository>();
        _mockKhachHangRepository = new Mock<IKhachHangRepository>();
        _mockKhuyenMaiService = new Mock<IKhuyenMaiService>();
        _mockKhuyenMaiRepository = new Mock<IKhuyenMaiRepository>();
        _mockHoaDonKhuyenMaiRepository = new Mock<IHoaDonKhuyenMaiRepository>();

        _hoaDonService = new HoaDonService(
            _mockHoaDonRepository.Object,
            _mockBanRepository.Object,
            _mockMonRepository.Object,
            _mockChiTietRepository.Object,
            _mockKhachHangRepository.Object,
            _mockKhuyenMaiService.Object,
            _mockKhuyenMaiRepository.Object,
            _mockHoaDonKhuyenMaiRepository.Object
        );
    }

    [Fact]
    public async Task TinhTongTienHoaDonAsync_CalculatesCorrectly()
    {
        // Arrange
        var chiTiets = new List<ChiTietHD>
        {
            new ChiTietHD { MaChiTiet = 1, MaHD = 1, MaMon = 1, SoLuong = 2, ThanhTien = 50000 },
            new ChiTietHD { MaChiTiet = 2, MaHD = 1, MaMon = 2, SoLuong = 1, ThanhTien = 30000 }
        };

        _mockChiTietRepository.Setup(repo => repo.GetChiTietsByHoaDonAsync(1)).ReturnsAsync(chiTiets);

        // Act
        var result = await _hoaDonService.TinhTongTienHoaDonAsync(1);

        // Assert
        Assert.Equal(80000, result);
    }

    [Fact]
    public async Task CreateHoaDonAsync_BanTrong_CreatesSuccessfully()
    {
        // Arrange
        var ban = new Ban { MaBan = 1, TenBan = "Bàn 1", TrangThai = "Trống" };
        var createDto = new CreateHoaDonDto { MaBan = 1, MaNV = 1 };

        var createdHoaDon = new HoaDon
        {
            MaHD = 1,
            MaBan = 1,
            ThoiGianTao = DateTime.Now,
            TongTien = 0,
            TrangThai = "Chưa thanh toán",
            MaNV = 1,
            Ban = ban,
            ChiTietHDs = new List<ChiTietHD>()
        };

        _mockBanRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(ban);
        _mockHoaDonRepository.Setup(repo => repo.AddAsync(It.IsAny<HoaDon>())).ReturnsAsync(createdHoaDon);
        _mockHoaDonRepository.Setup(repo => repo.GetHoaDonWithDetailsAsync(1)).ReturnsAsync(createdHoaDon);
        _mockBanRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Ban>())).Returns(Task.CompletedTask);

        // Act
        var result = await _hoaDonService.CreateHoaDonAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.MaHD);
        Assert.Equal("Chưa thanh toán", result.TrangThai);
        _mockBanRepository.Verify(repo => repo.UpdateAsync(It.Is<Ban>(b => b.TrangThai == "Đang sử dụng")), Times.Once);
    }

    [Fact]
    public async Task CreateHoaDonAsync_BanDangSuDung_ThrowsInvalidOperationException()
    {
        // Arrange
        var ban = new Ban { MaBan = 1, TenBan = "Bàn 1", TrangThai = "Đang sử dụng" };
        var createDto = new CreateHoaDonDto { MaBan = 1 };

        _mockBanRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(ban);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _hoaDonService.CreateHoaDonAsync(createDto));
    }

    [Fact]
    public async Task GoiMonAsync_ValidData_AddsMonSuccessfully()
    {
        // Arrange
        var hoaDon = new HoaDon
        {
            MaHD = 1,
            MaBan = 1,
            TongTien = 0,
            TrangThai = "Chưa thanh toán",
            ChiTietHDs = new List<ChiTietHD>()
        };

        var mon = new Mon { MaMon = 1, TenMon = "Cà phê đen", DonGia = 25000, Loai = "Cà phê" };

        var goiMonDto = new GoiMonDto
        {
            MaHD = 1,
            MaMon = 1,
            SoLuong = 2
        };

        _mockHoaDonRepository.Setup(repo => repo.GetHoaDonWithDetailsAsync(1)).ReturnsAsync(hoaDon);
        _mockMonRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(mon);
        _mockChiTietRepository.Setup(repo => repo.AddAsync(It.IsAny<ChiTietHD>())).ReturnsAsync(new ChiTietHD());
        _mockChiTietRepository.Setup(repo => repo.GetChiTietsByHoaDonAsync(1)).ReturnsAsync(new List<ChiTietHD>
        {
            new ChiTietHD { ThanhTien = 50000 }
        });
        _mockHoaDonRepository.Setup(repo => repo.UpdateAsync(It.IsAny<HoaDon>())).Returns(Task.CompletedTask);

        // Act
        var result = await _hoaDonService.GoiMonAsync(goiMonDto);

        // Assert
        Assert.NotNull(result);
        _mockChiTietRepository.Verify(repo => repo.AddAsync(It.IsAny<ChiTietHD>()), Times.Once);
    }

    [Fact]
    public async Task ThanhToanAsync_ValidData_CompletesSuccessfully()
    {
        // Arrange
        var ban = new Ban { MaBan = 1, TenBan = "Bàn 1", TrangThai = "Đang sử dụng" };
        var khachHang = new KhachHang { MaKH = 1, TenKH = "Khách hàng", DiemTichLuy = 0 };

        var hoaDon = new HoaDon
        {
            MaHD = 1,
            MaBan = 1,
            TongTien = 100000,
            TrangThai = "Chưa thanh toán",
            Ban = ban,
            ChiTietHDs = new List<ChiTietHD>
            {
                new ChiTietHD { MaChiTiet = 1, SoLuong = 1, ThanhTien = 100000 }
            }
        };

        var thanhToanDto = new ThanhToanDto { MaHD = 1, MaKH = 1 };

        _mockHoaDonRepository.Setup(repo => repo.GetHoaDonWithDetailsAsync(1)).ReturnsAsync(hoaDon);
        _mockHoaDonRepository.Setup(repo => repo.UpdateAsync(It.IsAny<HoaDon>())).Returns(Task.CompletedTask);
        _mockBanRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(ban);
        _mockBanRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Ban>())).Returns(Task.CompletedTask);
        _mockKhachHangRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(khachHang);
        _mockKhachHangRepository.Setup(repo => repo.UpdateAsync(It.IsAny<KhachHang>())).Returns(Task.CompletedTask);

        // Act
        var result = await _hoaDonService.ThanhToanAsync(thanhToanDto);

        // Assert
        Assert.NotNull(result);
        _mockHoaDonRepository.Verify(repo => repo.UpdateAsync(It.Is<HoaDon>(h => h.TrangThai == "Đã thanh toán")), Times.Once);
        _mockBanRepository.Verify(repo => repo.UpdateAsync(It.Is<Ban>(b => b.TrangThai == "Trống")), Times.Once);
        _mockKhachHangRepository.Verify(repo => repo.UpdateAsync(It.Is<KhachHang>(k => k.DiemTichLuy == 10)), Times.Once);
    }

    [Fact]
    public async Task ThanhToanAsync_HoaDonDaThanhToan_ThrowsInvalidOperationException()
    {
        // Arrange
        var hoaDon = new HoaDon
        {
            MaHD = 1,
            TrangThai = "Đã thanh toán",
            ChiTietHDs = new List<ChiTietHD> { new ChiTietHD() }
        };

        var thanhToanDto = new ThanhToanDto { MaHD = 1 };

        _mockHoaDonRepository.Setup(repo => repo.GetHoaDonWithDetailsAsync(1)).ReturnsAsync(hoaDon);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _hoaDonService.ThanhToanAsync(thanhToanDto));
    }
}

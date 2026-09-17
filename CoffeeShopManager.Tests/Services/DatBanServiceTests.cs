using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;
using Moq;
using Xunit;

namespace CoffeeShopManager.Tests.Services;

public class DatBanServiceTests
{
    private readonly Mock<IDatBanRepository> _datBanRepository = new();
    private readonly Mock<IBanRepository> _banRepository = new();
    private readonly Mock<IKhachHangRepository> _khachHangRepository = new();

    private DatBanService CreateService() => new(_datBanRepository.Object, _banRepository.Object, _khachHangRepository.Object);

    [Fact]
    public async Task CreateAsync_ValidData_CreatesBooking()
    {
        var start = DateTime.Now.AddDays(1).Date.AddHours(19);
        var end = start.AddHours(2);
        var ban = new Ban { MaBan = 1, TenBan = "Bàn 1", TrangThai = "Trống" };
        var customer = new KhachHang { MaKH = 2, TenKH = "Nguyễn Văn A", SoDienThoai = "0900000000" };
        _banRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ban);
        _khachHangRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(customer);
        _datBanRepository.Setup(r => r.HasOverlapAsync(1, start, end, null)).ReturnsAsync(false);
        _datBanRepository.Setup(r => r.AddAsync(It.IsAny<DatBan>())).ReturnsAsync((DatBan d) =>
        {
            d.MaDatBan = 10;
            return d;
        });

        var result = await CreateService().CreateAsync(new CreateDatBanDto
        {
            MaBan = 1, MaKH = 2, ThoiGianBatDau = start, ThoiGianKetThuc = end, SoLuongKhach = 4
        });

        Assert.Equal(10, result.MaDatBan);
        Assert.Equal("Nguyễn Văn A", result.TenKhachHang);
        Assert.Equal(DatBanService.ChoXacNhan, result.TrangThai);
    }

    [Fact]
    public async Task CreateAsync_OverlappingBooking_Throws()
    {
        var start = DateTime.Now.AddDays(1).Date.AddHours(19);
        var end = start.AddHours(2);
        _banRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Ban { MaBan = 1, TenBan = "Bàn 1" });
        _khachHangRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new KhachHang { MaKH = 2, TenKH = "Khách" });
        _datBanRepository.Setup(r => r.HasOverlapAsync(1, start, end, null)).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() => CreateService().CreateAsync(new CreateDatBanDto
        {
            MaBan = 1, MaKH = 2, ThoiGianBatDau = start, ThoiGianKetThuc = end, SoLuongKhach = 2
        }));
    }

    [Fact]
    public async Task CreateAsync_EndBeforeStart_Throws()
    {
        var start = DateTime.Now.AddDays(1);
        await Assert.ThrowsAsync<ArgumentException>(() => CreateService().CreateAsync(new CreateDatBanDto
        {
            MaBan = 1, MaKH = 2, ThoiGianBatDau = start, ThoiGianKetThuc = start.AddMinutes(-1), SoLuongKhach = 2
        }));
    }

    [Fact]
    public async Task UpdateStatusAsync_Confirm_UpdatesTableStatus()
    {
        var booking = new DatBan { MaDatBan = 1, MaBan = 2, MaKH = 3, TrangThai = DatBanService.ChoXacNhan };
        var ban = new Ban { MaBan = 2, TenBan = "Bàn 2", TrangThai = "Trống" };
        _datBanRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);
        _banRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(ban);

        var result = await CreateService().UpdateStatusAsync(1, DatBanService.DaXacNhan);

        Assert.Equal(DatBanService.DaXacNhan, result.TrangThai);
        Assert.Equal("Đã đặt", ban.TrangThai);
        _banRepository.Verify(r => r.UpdateAsync(ban), Times.Once);
    }
}

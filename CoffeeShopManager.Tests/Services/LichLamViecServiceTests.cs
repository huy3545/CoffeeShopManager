using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;
using Moq;
using Xunit;

namespace CoffeeShopManager.Tests.Services;

public class LichLamViecServiceTests
{
    [Fact]
    public async Task CreateShiftAsync_InvalidTime_Throws()
    {
        var service = CreateService();

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateShiftAsync(new CreateCaLamViecDto
        {
            TenCa = "Ca sáng",
            ThoiGianBatDau = DateTime.Now.AddDays(1).AddHours(8),
            ThoiGianKetThuc = DateTime.Now.AddDays(1).AddHours(7),
            SoLuongNhanVienCan = 1
        }));
    }

    [Fact]
    public async Task CreateShiftAsync_TodayShift_IsAllowedForEmergencyReplacement()
    {
        var repository = new Mock<ILichLamViecRepository>();
        repository.Setup(x => x.AddAsync(It.IsAny<CaLamViec>()))
            .ReturnsAsync((CaLamViec shift) => { shift.MaCa = 10; return shift; });
        var service = new LichLamViecService(repository.Object, new Mock<IDangKyCaRepository>().Object);

        var result = await service.CreateShiftAsync(new CreateCaLamViecDto
        {
            TenCa = "Ca bổ sung hôm nay",
            ThoiGianBatDau = DateTime.Today.AddHours(7),
            ThoiGianKetThuc = DateTime.Today.AddHours(12),
            SoLuongNhanVienCan = 1
        });

        Assert.Equal(10, result.MaCa);
    }

    [Fact]
    public async Task CreateShiftAsync_EveningShift_CanEndNextDay()
    {
        var repository = new Mock<ILichLamViecRepository>();
        repository.Setup(x => x.AddAsync(It.IsAny<CaLamViec>()))
            .ReturnsAsync((CaLamViec shift) => shift);
        var service = new LichLamViecService(repository.Object, new Mock<IDangKyCaRepository>().Object);
        var start = DateTime.Today.AddHours(22);

        var result = await service.CreateShiftAsync(new CreateCaLamViecDto
        {
            TenCa = "Ca tối",
            LoaiCa = "Tối",
            ThoiGianBatDau = start,
            ThoiGianKetThuc = DateTime.Today.AddHours(6),
            SoLuongNhanVienCan = 1
        });

        Assert.Equal(start.AddDays(1).Date.AddHours(6), result.ThoiGianKetThuc);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateRegistration_Throws()
    {
        var shifts = new Mock<ILichLamViecRepository>();
        var registrations = new Mock<IDangKyCaRepository>();
        shifts.Setup(x => x.GetShiftWithRegistrationsAsync(1)).ReturnsAsync(new CaLamViec
        {
            MaCa = 1,
            TenCa = "Ca chiều",
            ThoiGianBatDau = DateTime.Now.AddDays(1).AddHours(14),
            ThoiGianKetThuc = DateTime.Now.AddDays(1).AddHours(22),
            SoLuongNhanVienCan = 2,
            TrangThai = LichLamViecService.DangMo
        });
        shifts.Setup(x => x.GetEmployeeAsync(3)).ReturnsAsync(new NhanVien { MaNV = 3, LoaiNhanVien = "Part-time" });
        shifts.Setup(x => x.HasRegistrationAsync(1, 3)).ReturnsAsync(true);
        var service = new LichLamViecService(shifts.Object, registrations.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterAsync(new DangKyCaRequestDto { MaCa = 1, MaNV = 3 }));
        registrations.Verify(x => x.AddAsync(It.IsAny<DangKyCa>()), Times.Never);
    }

    [Fact]
    public async Task ReviewRegistrationAsync_ApprovedRegistration_UpdatesStatus()
    {
        var shifts = new Mock<ILichLamViecRepository>();
        var registrations = new Mock<IDangKyCaRepository>();
        var registration = new DangKyCa { MaDangKy = 5, MaCa = 1, MaNV = 3, TrangThai = LichLamViecService.ChoDuyet };
        registrations.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(registration);
        shifts.Setup(x => x.GetShiftWithRegistrationsAsync(1)).ReturnsAsync(new CaLamViec
        {
            MaCa = 1,
            ThoiGianBatDau = DateTime.Now.AddDays(1).AddHours(8),
            ThoiGianKetThuc = DateTime.Now.AddDays(1).AddHours(16),
            SoLuongNhanVienCan = 1,
            TrangThai = LichLamViecService.DangMo
        });
        shifts.Setup(x => x.CountApprovedAsync(1)).ReturnsAsync(0);
        shifts.Setup(x => x.GetEmployeeAsync(3)).ReturnsAsync(new NhanVien { MaNV = 3, LoaiNhanVien = "Part-time" });
        shifts.Setup(x => x.HasStaffOverlapAsync(3, It.IsAny<DateTime>(), It.IsAny<DateTime>(), 5)).ReturnsAsync(false);
        var service = new LichLamViecService(shifts.Object, registrations.Object);

        await service.ReviewRegistrationAsync(5, LichLamViecService.DaDuyet, "manager");

        Assert.Equal(LichLamViecService.DaDuyet, registration.TrangThai);
        Assert.Equal("manager", registration.NguoiDuyet);
        registrations.Verify(x => x.UpdateAsync(registration), Times.Once);
    }

    private static LichLamViecService CreateService() =>
        new(new Mock<ILichLamViecRepository>().Object, new Mock<IDangKyCaRepository>().Object);
}

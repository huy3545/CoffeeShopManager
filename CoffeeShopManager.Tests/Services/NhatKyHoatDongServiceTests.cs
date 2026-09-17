using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;
using Moq;
using Xunit;

namespace CoffeeShopManager.Tests.Services;

public class NhatKyHoatDongServiceTests
{
    [Fact]
    public async Task GhiNhanAsync_ValidData_SavesWithoutSensitivePassword()
    {
        var repository = new Mock<INhatKyHoatDongRepository>();
        repository.Setup(x => x.AddAsync(It.IsAny<NhatKyHoatDong>()))
            .ReturnsAsync((NhatKyHoatDong item) => { item.MaNhatKy = 1; return item; });
        var service = new NhatKyHoatDongService(repository.Object);

        var result = await service.GhiNhanAsync(new GhiNhatKyDto
        {
            TenNguoiDung = "admin",
            HanhDong = "Đăng nhập",
            LoaiDoiTuong = "Xác thực",
            MoTa = "Đăng nhập thành công"
        });

        Assert.Equal(1, result.MaNhatKy);
        repository.Verify(x => x.AddAsync(It.Is<NhatKyHoatDong>(item => item.MoTa == "Đăng nhập thành công" && item.DuLieuMoi == null)), Times.Once);
    }

    [Fact]
    public async Task GhiNhanAsync_EmptyAction_Throws()
    {
        var service = new NhatKyHoatDongService(new Mock<INhatKyHoatDongRepository>().Object);
        await Assert.ThrowsAsync<ArgumentException>(() => service.GhiNhanAsync(new GhiNhatKyDto
        {
            LoaiDoiTuong = "Tài khoản",
            MoTa = "Mô tả"
        }));
    }
}

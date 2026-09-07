using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;
using Moq;
using Xunit;

namespace CoffeeShopManager.Tests.Services;

public class BanServiceTests
{
    private readonly Mock<IBanRepository> _mockBanRepository;
    private readonly BanService _banService;

    public BanServiceTests()
    {
        _mockBanRepository = new Mock<IBanRepository>();
        _banService = new BanService(_mockBanRepository.Object);
    }

    [Fact]
    public async Task GetAllBansAsync_ReturnsAllBans()
    {
        // Arrange
        var bans = new List<Ban>
        {
            new Ban { MaBan = 1, TenBan = "Bàn 1", TrangThai = "Trống" },
            new Ban { MaBan = 2, TenBan = "Bàn 2", TrangThai = "Đang sử dụng" }
        };
        _mockBanRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(bans);

        // Act
        var result = await _banService.GetAllBansAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockBanRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetBanByIdAsync_ValidId_ReturnsBan()
    {
        // Arrange
        var ban = new Ban { MaBan = 1, TenBan = "Bàn 1", TrangThai = "Trống" };
        _mockBanRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(ban);

        // Act
        var result = await _banService.GetBanByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.MaBan);
        Assert.Equal("Bàn 1", result.TenBan);
    }

    [Fact]
    public async Task GetBanByIdAsync_InvalidId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _banService.GetBanByIdAsync(0));
        await Assert.ThrowsAsync<ArgumentException>(() => _banService.GetBanByIdAsync(-1));
    }

    [Fact]
    public async Task CreateBanAsync_ValidData_CreatesSuccessfully()
    {
        // Arrange
        var createDto = new CreateBanDto
        {
            TenBan = "Bàn mới",
            TrangThai = "Trống"
        };

        var createdBan = new Ban
        {
            MaBan = 1,
            TenBan = "Bàn mới",
            TrangThai = "Trống"
        };

        _mockBanRepository.Setup(repo => repo.AddAsync(It.IsAny<Ban>())).ReturnsAsync(createdBan);

        // Act
        var result = await _banService.CreateBanAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Bàn mới", result.TenBan);
        _mockBanRepository.Verify(repo => repo.AddAsync(It.IsAny<Ban>()), Times.Once);
    }

    [Fact]
    public async Task CreateBanAsync_EmptyTenBan_ThrowsArgumentException()
    {
        // Arrange
        var createDto = new CreateBanDto { TenBan = "", TrangThai = "Trống" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _banService.CreateBanAsync(createDto));
    }

    [Fact]
    public async Task CreateBanAsync_InvalidTrangThai_ThrowsArgumentException()
    {
        // Arrange
        var createDto = new CreateBanDto { TenBan = "Bàn 1", TrangThai = "Invalid" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _banService.CreateBanAsync(createDto));
    }

    [Fact]
    public async Task DeleteBanAsync_ValidId_DeletesSuccessfully()
    {
        // Arrange
        _mockBanRepository.Setup(repo => repo.ExistsAsync(1)).ReturnsAsync(true);
        _mockBanRepository.Setup(repo => repo.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act
        await _banService.DeleteBanAsync(1);

        // Assert
        _mockBanRepository.Verify(repo => repo.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteBanAsync_NonExistentId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockBanRepository.Setup(repo => repo.ExistsAsync(999)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _banService.DeleteBanAsync(999));
    }
}


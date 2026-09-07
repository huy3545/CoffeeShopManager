using CoffeeShopManager.BLL.DTOs;
using CoffeeShopManager.BLL.Services;
using CoffeeShopManager.DAL.Entities;
using CoffeeShopManager.DAL.Repositories;
using Moq;
using Xunit;

namespace CoffeeShopManager.Tests.Services;

public class MonServiceTests
{
    private readonly Mock<IMonRepository> _mockMonRepository;
    private readonly MonService _monService;

    public MonServiceTests()
    {
        _mockMonRepository = new Mock<IMonRepository>();
        _monService = new MonService(_mockMonRepository.Object);
    }

    [Fact]
    public async Task GetAllMonsAsync_ReturnsAllMons()
    {
        // Arrange
        var mons = new List<Mon>
        {
            new Mon { MaMon = 1, TenMon = "Cà phê đen", DonGia = 25000, Loai = "Cà phê" },
            new Mon { MaMon = 2, TenMon = "Cà phê sữa", DonGia = 30000, Loai = "Cà phê" }
        };
        _mockMonRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mons);

        // Act
        var result = await _monService.GetAllMonsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateMonAsync_ValidData_CreatesSuccessfully()
    {
        // Arrange
        var createDto = new CreateMonDto
        {
            TenMon = "Trà đào",
            DonGia = 35000,
            Loai = "Trà"
        };

        var createdMon = new Mon
        {
            MaMon = 1,
            TenMon = "Trà đào",
            DonGia = 35000,
            Loai = "Trà"
        };

        _mockMonRepository.Setup(repo => repo.AddAsync(It.IsAny<Mon>())).ReturnsAsync(createdMon);

        // Act
        var result = await _monService.CreateMonAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Trà đào", result.TenMon);
        Assert.Equal(35000, result.DonGia);
    }

    [Fact]
    public async Task CreateMonAsync_NegativePrice_ThrowsArgumentException()
    {
        // Arrange
        var createDto = new CreateMonDto
        {
            TenMon = "Món test",
            DonGia = -1000,
            Loai = "Test"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _monService.CreateMonAsync(createDto));
    }

    [Fact]
    public async Task CreateMonAsync_EmptyTenMon_ThrowsArgumentException()
    {
        // Arrange
        var createDto = new CreateMonDto
        {
            TenMon = "",
            DonGia = 25000,
            Loai = "Cà phê"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _monService.CreateMonAsync(createDto));
    }

    [Fact]
    public async Task UpdateMonAsync_ValidData_UpdatesSuccessfully()
    {
        // Arrange
        var existingMon = new Mon
        {
            MaMon = 1,
            TenMon = "Cà phê đen",
            DonGia = 25000,
            Loai = "Cà phê"
        };

        var updateDto = new UpdateMonDto
        {
            DonGia = 30000
        };

        _mockMonRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingMon);
        _mockMonRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Mon>())).Returns(Task.CompletedTask);

        // Act
        var result = await _monService.UpdateMonAsync(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(30000, result.DonGia);
        _mockMonRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Mon>()), Times.Once);
    }
}


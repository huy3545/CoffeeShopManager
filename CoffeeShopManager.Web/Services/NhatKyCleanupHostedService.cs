using CoffeeShopManager.BLL.Services;

namespace CoffeeShopManager.Web.Services;

public sealed class NhatKyCleanupHostedService : BackgroundService
{
    private static readonly TimeSpan CleanupInterval = TimeSpan.FromHours(24);
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NhatKyCleanupHostedService> _logger;

    public NhatKyCleanupHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<NhatKyCleanupHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await CleanupAsync(stoppingToken);

        using var timer = new PeriodicTimer(CleanupInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await CleanupAsync(stoppingToken);
        }
    }

    private async Task CleanupAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<INhatKyHoatDongService>();
            var deletedCount = await service.CleanupExpiredAsync();

            if (deletedCount > 0)
            {
                _logger.LogInformation("Đã tự động xóa {DeletedCount} nhật ký thao tác đủ 30 ngày.", deletedCount);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Ứng dụng đang dừng, không cần ghi lỗi.
        }
        catch (Exception ex)
        {
            // Không để lỗi dọn nhật ký làm dừng ứng dụng.
            _logger.LogError(ex, "Không thể tự động dọn nhật ký thao tác.");
        }
    }
}

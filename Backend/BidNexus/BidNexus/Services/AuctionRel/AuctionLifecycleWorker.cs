using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Services;
using Microsoft.Extensions.Options;

namespace API.Services.AuctionRel;

public sealed class AuctionLifecycleWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<AuctionEngineOptions> options,
    ILogger<AuctionLifecycleWorker> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly AuctionEngineOptions _options = options.Value;
    private readonly ILogger<AuctionLifecycleWorker> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = TimeSpan.FromSeconds(
            Math.Max(1, _options.PollingIntervalSeconds));

        _logger.LogInformation(
            "Auction lifecycle worker started. Poll interval: {IntervalSeconds}s.",
            interval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessLifecycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing auction lifecycle.");
            }

            await Task.Delay(interval, stoppingToken);
        }

        _logger.LogInformation("Auction lifecycle worker stopped.");
    }

    private async Task ProcessLifecycleAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAuctionRepository>();
        var statementService = scope.ServiceProvider.GetRequiredService<IAuctionStatementService>();
        var realtimeService = scope.ServiceProvider.GetRequiredService<IAuctionRealtimeService>();
        var now = DateTimeOffset.UtcNow;

        var auctionsToStart = await repository.GetAuctionsForLifecycleAsync(
            now,
            _options.ScheduledStatusName,
            _options.ActiveStatusName,
            _options.ClosedStatusName);

        foreach (var auction in auctionsToStart)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (auction.ShouldStart)
            {
                await repository.UpdateStatusAsync(
                    auction.Id,
                    _options.ActiveStatusName);
            }

            if (auction.ShouldClose)
            {
                await repository.UpdateStatusAsync(
                    auction.Id,
                    _options.ClosedStatusName);

                await statementService.GenerateAsync(
                    auction.Id,
                    auction.TenantId,
                    cancellationToken);

                await realtimeService.PublishAuctionClosedAsync(
                    auction.Id,
                    cancellationToken);
            }
        }
    }
}

using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Services;
using Core.Enumeration;

namespace API.Services.AuctionRel;

public sealed class AuctionLifecycleWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<AuctionLifecycleWorker> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<AuctionLifecycleWorker> _logger = logger;
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Auction lifecycle worker started. Poll interval: {IntervalSeconds}s.",
            PollingInterval.TotalSeconds);

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

            await Task.Delay(PollingInterval, stoppingToken);
        }

        _logger.LogInformation("Auction lifecycle worker stopped.");
    }

    private async Task ProcessLifecycleAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAuctionRepository>();
        var statementService = scope.ServiceProvider.GetRequiredService<IAuctionStatementCoreService>();
        var realtimeService = scope.ServiceProvider.GetRequiredService<IAuctionRealtimeCoreService>();
        var now = DateTimeOffset.UtcNow;

        var auctionsToProcess = await repository.GetAuctionsForLifecycleAsync(now);

        foreach (var auction in auctionsToProcess)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (auction.ShouldStart)
            {
                await repository.UpdateStatusAsync(
                    auction.Id,
                    (short)StatusEnum.Open);

                await realtimeService.PublishAuctionStartedAsync(
                    auction.Id,
                    cancellationToken);
            }

            if (auction.ShouldComplete)
            {
                await repository.UpdateStatusAsync(
                    auction.Id,
                    (short)StatusEnum.Completed);

                await statementService.GenerateAsync(
                    auction.Id,
                    cancellationToken);

                await realtimeService.PublishAuctionCompletedAsync(
                    auction.Id,
                    cancellationToken);
            }
            else if (auction.NeedsStatementGeneration)
            {
                await statementService.GenerateAsync(
                    auction.Id,
                    cancellationToken);
            }
        }
    }
}

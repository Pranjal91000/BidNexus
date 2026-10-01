using System.Collections.Concurrent;
using API.Abstraction.AuctionRel;
using API.Models.AuctionRel;
using Core.Abstraction.AuctionRelated;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace API.Services.AuctionRel;

public sealed class AuctionEngine(
    IServiceScopeFactory scopeFactory,
    IOptions<AuctionEngineOptions> options) : IAuctionEngine
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly AuctionEngineOptions _options = options.Value;

    // Singleton engine + one semaphore per auction gives us one shared
    // serialization point across all HTTP requests handled by this process.
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _auctionLocks = new();

    public async Task<BidResponseDataModel> ProcessBidAsync(
        BidCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.AuctionId <= 0)
            throw new ArgumentException("AuctionId must be greater than zero.", nameof(request));

        var auctionLock = _auctionLocks.GetOrAdd(
            request.AuctionId,
            static _ => new SemaphoreSlim(1, 1));

        await auctionLock.WaitAsync(cancellationToken);

        try
        {
            using var scope = _scopeFactory.CreateScope();

            var auctionRepository =
                scope.ServiceProvider.GetRequiredService<IAuctionRepository>();
            var bidService =
                scope.ServiceProvider.GetRequiredService<IBidService>();
            var validator =
                scope.ServiceProvider.GetRequiredService<IValidator<BidCreateRequest>>();

            var auction = await auctionRepository.GetById(request.AuctionId);
            var now = DateTimeOffset.UtcNow;

            if (now < auction.AuctionStartTime)
                throw new InvalidOperationException("Auction has not started yet.");

            if (now >= auction.AuctionEndTime)
                throw new InvalidOperationException("Auction has already ended.");

            if (!string.Equals(
                    auction.StatusName,
                    _options.ActiveStatusName,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Auction is not active. Current status: {auction.StatusName ?? "Unknown"}.");
            }

            var validation = await validator.ValidateAsync(request, cancellationToken);

            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            return await bidService.ProcessBidAsync(request, cancellationToken);
        }
        finally
        {
            auctionLock.Release();
        }
    }
}

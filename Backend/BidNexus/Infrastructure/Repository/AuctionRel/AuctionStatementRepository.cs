using Core.Abstraction.AuctionRelated;
using Core.Entities.Auction;
using Core.Models.AuctionRelated;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.AuctionRel;

public sealed class AuctionStatementRepository(
    AppDbContext dbContext) : IAuctionStatementRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task GenerateAsync(
        int auctionId,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var auctionExists = await _dbContext.Auctions
                .AsNoTracking()
                .AnyAsync(
                    a => a.Id == auctionId &&
                         a.TenantId == tenantId,
                    cancellationToken);

            if (!auctionExists)
                throw new KeyNotFoundException($"Auction with ID {auctionId} not found.");

            var alreadyGenerated = await _dbContext.AuctionStatements
                .AnyAsync(
                    x => x.AuctionId == auctionId &&
                         x.TenantId == tenantId,
                    cancellationToken);

            if (alreadyGenerated)
            {
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            var auction = await _dbContext.Auctions
                .AsNoTracking()
                .Where(a => a.Id == auctionId && a.TenantId == tenantId)
                .Select(a => new { a.Id, a.IsForwardAuction })
                .SingleAsync(cancellationToken);

            var currentBids = await _dbContext.Bids
                .AsNoTracking()
                .Where(b =>
                    b.AuctionId == auctionId &&
                    b.IsCurrent &&
                    b.Auction.TenantId == tenantId)
                .OrderByDescending(b => auction.IsForwardAuction
                    ? b.NetAmount
                    : -b.NetAmount)
                .ThenBy(b => b.CreatedAt)
                .Select(b => new
                {
                    b.Id,
                    b.VendorId,
                    b.NetAmount
                })
                .ToListAsync(cancellationToken);

            short rank = 1;
            foreach (var bid in currentBids)
            {
                _dbContext.AuctionStatements.Add(new AuctionStatement
                {
                    AuctionId = auctionId,
                    BidId = bid.Id,
                    VendorId = bid.VendorId,
                    Rank = rank,
                    IsWinner = rank == 1,
                    TenantId = tenantId
                });

                rank++;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<List<AuctionStatementDataModel>> GetAsync(
        int auctionId,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.AuctionStatements
            .AsNoTracking()
            .Where(x =>
                x.AuctionId == auctionId &&
                x.TenantId == tenantId)
            .OrderBy(x => x.Rank)
            .Select(x => new AuctionStatementDataModel
            {
                Id = x.Id,
                AuctionId = x.AuctionId,
                BidId = x.BidId,
                VendorId = x.VendorId,
                VendorName = x.Vendor.Name,
                NetAmount = x.Bid.NetAmount,
                Rank = x.Rank,
                IsWinner = x.IsWinner
            })
            .ToListAsync(cancellationToken);
    }
}

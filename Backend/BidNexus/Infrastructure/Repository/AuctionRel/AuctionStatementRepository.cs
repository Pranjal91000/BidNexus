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
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var auction = await _dbContext.Auctions
                .AsNoTracking()
                .Where(a => a.Id == auctionId)
                .Select(a => new { a.Id, a.TenantId, a.IsForwardAuction })
                .FirstOrDefaultAsync(cancellationToken);

            if (auction == null)
                throw new KeyNotFoundException($"Auction with ID {auctionId} not found.");

            var tenantId = auction.TenantId;

            var alreadyGenerated = await _dbContext.AuctionStatements
                .AnyAsync(
                    x => x.AuctionId == auctionId,
                    cancellationToken);

            if (alreadyGenerated)
            {
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            var bidsQuery = _dbContext.Bids
                .AsNoTracking()
                .Where(b => b.AuctionId == auctionId && b.IsCurrent);

            if (auction.IsForwardAuction)
            {
                bidsQuery = bidsQuery.OrderByDescending(b => b.NetAmount).ThenBy(b => b.CreatedAt);
            }
            else
            {
                bidsQuery = bidsQuery.OrderBy(b => b.NetAmount).ThenBy(b => b.CreatedAt);
            }

            var currentBids = await bidsQuery
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
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            // Race condition: if concurrent invocation already inserted, confirm idempotency
            var alreadyGenerated = await _dbContext.AuctionStatements
                .AnyAsync(
                    x => x.AuctionId == auctionId,
                    CancellationToken.None);

            if (alreadyGenerated)
                return;

            throw;
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

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
                BasicAmount = x.Bid.BasicAmount,
                TaxAmount = x.Bid.TaxAmount,
                NetAmount = x.Bid.NetAmount,
                Rank = x.Rank,
                IsWinner = x.IsWinner,
                BidRevisionNo = x.Bid.BidRevisionNo,
                SubmittedAt = x.Bid.CreatedAt,
                Lines = x.Bid.BidDetails
                    .OrderBy(d => d.AuctionRequirement.LineNo)
                    .Select(d => new StatementLineDataModel
                    {
                        AuctionRequirementId = d.AuctionRequirementId,
                        LineNo = d.AuctionRequirement.LineNo,
                        ItemName = d.AuctionRequirement.Item.Name,
                        Quantity = d.AuctionRequirement.Quantity,
                        UnitName = d.AuctionRequirement.Unit.Name,
                        Rate = d.Rate,
                        BaseAmount = d.BaseAmount,
                        NetAmount = d.NetAmount
                    }).ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<VendorAuctionResultDataModel?> GetVendorResultAsync(
        int auctionId,
        int vendorId,
        CancellationToken cancellationToken = default)
    {
        var auction = await _dbContext.Auctions
            .AsNoTracking()
            .Where(a => a.Id == auctionId)
            .Select(a => new { a.Id, a.IsBidPriceHidden })
            .FirstOrDefaultAsync(cancellationToken);

        if (auction == null)
            return null;

        var rows = await _dbContext.AuctionStatements
            .AsNoTracking()
            .Where(x => x.AuctionId == auctionId)
            .Select(x => new { x.VendorId, x.Rank, x.IsWinner, x.BidId, x.Bid.NetAmount })
            .ToListAsync(cancellationToken);

        var result = new VendorAuctionResultDataModel
        {
            AuctionId = auctionId,
            Bidders = rows.Count,
            PricesHidden = auction.IsBidPriceHidden
        };

        var mine = rows.FirstOrDefault(r => r.VendorId == vendorId);
        if (mine == null)
            return result;

        var winner = rows.FirstOrDefault(r => r.IsWinner);
        var myBid = await _dbContext.Bids
            .AsNoTracking()
            .Where(b => b.Id == mine.BidId)
            .Select(b => new
            {
                b.BasicAmount,
                b.TaxAmount,
                b.NetAmount,
                b.BidRevisionNo,
                Lines = b.BidDetails
                    .OrderBy(d => d.AuctionRequirement.LineNo)
                    .Select(d => new StatementLineDataModel
                    {
                        AuctionRequirementId = d.AuctionRequirementId,
                        LineNo = d.AuctionRequirement.LineNo,
                        ItemName = d.AuctionRequirement.Item.Name,
                        Quantity = d.AuctionRequirement.Quantity,
                        UnitName = d.AuctionRequirement.Unit.Name,
                        Rate = d.Rate,
                        BaseAmount = d.BaseAmount,
                        NetAmount = d.NetAmount
                    }).ToList()
            })
            .FirstAsync(cancellationToken);

        result.Participated = true;
        result.Rank = mine.Rank;
        result.IsWinner = mine.IsWinner;
        result.MyNetAmount = myBid.NetAmount;
        result.MyBasicAmount = myBid.BasicAmount;
        result.MyTaxAmount = myBid.TaxAmount;
        result.BidRevisionNo = myBid.BidRevisionNo;
        result.Lines = myBid.Lines;
        result.WinningAmount = mine.IsWinner || !auction.IsBidPriceHidden ? winner?.NetAmount : null;
        return result;
    }
}

using Core.Abstraction.Dashboard;
using Core.Enumeration;
using Core.Models.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Dashboard;

/// <summary>
/// Read-only aggregates for the overview screens. Numbers are computed in memory from small
/// projections (auctions of one tenant / bids of one vendor), which keeps the SQL simple.
/// </summary>
public sealed class DashboardRepository(AppDbContext dbContext) : IDashboardRepository
{
    private const int MonthsInChart = 6;
    private readonly AppDbContext _db = dbContext;

    public async Task<OrganizationDashboardDataModel> GetOrganizationDashboardAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var chartStart = MonthStart(now).AddMonths(-(MonthsInChart - 1));

        var auctions = await _db.Auctions
            .AsNoTracking()
            .Where(a => a.TenantId == tenantId)
            .Select(a => new { a.Id, a.AuctionName, a.DocNoYearly, a.StatusId, a.AuctionStartTime, a.AuctionEndTime, a.IsForwardAuction })
            .ToListAsync(cancellationToken);

        var completed = auctions
            .Where(a => a.StatusId == (short)StatusEnum.Completed && a.AuctionEndTime >= chartStart)
            .ToList();
        var completedIds = completed.Select(a => a.Id).ToList();

        var bids = await _db.Bids
            .AsNoTracking()
            .Where(b => completedIds.Contains(b.AuctionId))
            .Select(b => new { b.AuctionId, b.VendorId, b.NetAmount, b.BidRevisionNo })
            .ToListAsync(cancellationToken);

        var winners = await _db.AuctionStatements
            .AsNoTracking()
            .Where(s => completedIds.Contains(s.AuctionId) && s.IsWinner)
            .Select(s => new { s.AuctionId, VendorName = s.Vendor.Name, s.Bid.NetAmount })
            .ToListAsync(cancellationToken);

        var results = completed
            .OrderByDescending(a => a.AuctionEndTime)
            .Select(a =>
            {
                var auctionBids = bids.Where(b => b.AuctionId == a.Id).ToList();
                var winner = winners.FirstOrDefault(w => w.AuctionId == a.Id);
                var openingBids = auctionBids.Where(b => b.BidRevisionNo <= 1).Select(b => b.NetAmount).ToList();
                decimal? improvement = null;
                if (winner != null && openingBids.Count > 0)
                {
                    var opening = a.IsForwardAuction ? openingBids.Max() : openingBids.Min();
                    if (opening > 0)
                    {
                        improvement = a.IsForwardAuction
                            ? (winner.NetAmount - opening) / opening * 100m
                            : (opening - winner.NetAmount) / opening * 100m;
                        improvement = Math.Round(improvement.Value, 1);
                    }
                }

                return new AuctionResultSummaryDataModel
                {
                    AuctionId = a.Id,
                    AuctionName = a.AuctionName,
                    DocNoYearly = a.DocNoYearly,
                    ClosedAt = a.AuctionEndTime,
                    WinnerName = winner?.VendorName,
                    WinningAmount = winner?.NetAmount,
                    Bidders = auctionBids.Select(b => b.VendorId).Distinct().Count(),
                    PriceImprovementPercent = improvement
                };
            })
            .ToList();

        var last90 = results.Where(r => r.ClosedAt >= now.AddDays(-90)).ToList();
        var improvements = results.Where(r => r.PriceImprovementPercent.HasValue).Select(r => r.PriceImprovementPercent!.Value).ToList();
        var withBids = results.Where(r => r.Bidders > 0).ToList();

        return new OrganizationDashboardDataModel
        {
            Live = auctions.Count(a => a.StatusId == (short)StatusEnum.Open),
            Upcoming = auctions.Count(a => a.StatusId != (short)StatusEnum.Draft &&
                                           a.StatusId != (short)StatusEnum.Open &&
                                           a.StatusId != (short)StatusEnum.Completed &&
                                           a.AuctionEndTime > now),
            Drafts = auctions.Count(a => a.StatusId == (short)StatusEnum.Draft),
            ClosedLast90Days = last90.Count,
            AwardedValueLast90Days = last90.Sum(r => r.WinningAmount ?? 0),
            AveragePriceImprovementPercent = improvements.Count > 0 ? Math.Round(improvements.Average(), 1) : null,
            AverageBiddersPerAuction = withBids.Count > 0 ? Math.Round((decimal)withBids.Average(r => r.Bidders), 1) : 0,
            Monthly = Months(chartStart).Select(m => new MonthlyPointDataModel
            {
                Month = m.ToString("yyyy-MM"),
                Auctions = results.Count(r => MonthStart(r.ClosedAt) == m),
                Value = results.Where(r => MonthStart(r.ClosedAt) == m).Sum(r => r.WinningAmount ?? 0)
            }).ToList(),
            RecentResults = results.Take(8).ToList()
        };
    }

    public async Task<VendorDashboardDataModel> GetVendorDashboardAsync(
        int vendorId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var chartStart = MonthStart(now).AddMonths(-(MonthsInChart - 1));
        var yearAgo = now.AddMonths(-12);

        var myAuctions = await _db.Bids
            .AsNoTracking()
            .Where(b => b.VendorId == vendorId)
            .Select(b => new { b.AuctionId, b.Auction.AuctionName, b.Auction.StatusId, b.Auction.AuctionEndTime, b.Auction.IsForwardAuction })
            .Distinct()
            .ToListAsync(cancellationToken);

        var liveIds = myAuctions.Where(a => a.StatusId == (short)StatusEnum.Open).Select(a => a.AuctionId).ToList();
        var liveBids = await _db.Bids
            .AsNoTracking()
            .Where(b => liveIds.Contains(b.AuctionId) && b.IsCurrent)
            .Select(b => new { b.AuctionId, b.VendorId, b.NetAmount, b.CreatedAt })
            .ToListAsync(cancellationToken);

        var positions = myAuctions
            .Where(a => liveIds.Contains(a.AuctionId))
            .Select(a =>
            {
                var ordered = liveBids.Where(b => b.AuctionId == a.AuctionId);
                ordered = a.IsForwardAuction
                    ? ordered.OrderByDescending(b => b.NetAmount).ThenBy(b => b.CreatedAt)
                    : ordered.OrderBy(b => b.NetAmount).ThenBy(b => b.CreatedAt);
                var list = ordered.ToList();
                return new VendorLivePositionDataModel
                {
                    AuctionId = a.AuctionId,
                    AuctionName = a.AuctionName,
                    EndsAt = a.AuctionEndTime,
                    Rank = list.FindIndex(b => b.VendorId == vendorId) + 1,
                    Bidders = list.Count
                };
            })
            .OrderBy(p => p.EndsAt)
            .ToList();

        var closed = myAuctions.Where(a => a.StatusId == (short)StatusEnum.Completed && a.AuctionEndTime >= yearAgo).ToList();
        var closedIds = closed.Select(a => a.AuctionId).ToList();
        var wins = await _db.AuctionStatements
            .AsNoTracking()
            .Where(s => s.VendorId == vendorId && s.IsWinner && closedIds.Contains(s.AuctionId))
            .Select(s => new { s.AuctionId, s.Bid.NetAmount })
            .ToListAsync(cancellationToken);

        var upcomingOpen = await _db.Auctions
            .AsNoTracking()
            .CountAsync(a => a.StatusId != (short)StatusEnum.Draft &&
                             a.StatusId != (short)StatusEnum.Completed &&
                             a.AuctionStartTime > now &&
                             a.OpenToAll, cancellationToken);

        return new VendorDashboardDataModel
        {
            LiveParticipating = positions.Count,
            Leading = positions.Count(p => p.Rank == 1),
            UpcomingOpen = upcomingOpen,
            ParticipatedLast12Months = closed.Count,
            WonLast12Months = wins.Count,
            WonValueLast12Months = wins.Sum(w => w.NetAmount),
            Monthly = Months(chartStart).Select(m => new MonthlyPointDataModel
            {
                Month = m.ToString("yyyy-MM"),
                Auctions = closed.Count(a => MonthStart(a.AuctionEndTime) == m),
                Won = closed.Count(a => MonthStart(a.AuctionEndTime) == m && wins.Any(w => w.AuctionId == a.AuctionId)),
                Value = wins.Where(w => closed.Any(a => a.AuctionId == w.AuctionId && MonthStart(a.AuctionEndTime) == m)).Sum(w => w.NetAmount)
            }).ToList(),
            LivePositions = positions
        };
    }

    private static DateTimeOffset MonthStart(DateTimeOffset value)
        => new(value.UtcDateTime.Year, value.UtcDateTime.Month, 1, 0, 0, 0, TimeSpan.Zero);

    private static IEnumerable<DateTimeOffset> Months(DateTimeOffset start)
        => Enumerable.Range(0, MonthsInChart).Select(i => start.AddMonths(i));
}

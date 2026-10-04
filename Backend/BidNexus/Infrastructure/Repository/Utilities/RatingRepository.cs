using Core.Abstraction.Utilities;
using Core.Entities.Utilities;
using Core.Models.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Utilities;

public class RatingRepository(AppDbContext dbContext) : IRatingRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<RatingDataModel> SubmitRatingAsync(
        Rating rating,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.Ratings.AddAsync(rating, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return await GetRatingModelAsync(
                rating.AuctionId,
                rating.RatingForId,
                rating.SubmittedByTenant,
                rating.AgainstTenant,
                cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<TenantReputationDataModel> GetTenantReputationAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var tenant = await _dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

        var tenantName = tenant?.Name ?? $"Tenant #{tenantId}";

        var ratings = await _dbContext.Ratings
            .AsNoTracking()
            .Include(r => r.RatingValues)
                .ThenInclude(v => v.RatingParameter)
            .Where(r => r.AgainstTenant == tenantId)
            .ToListAsync(cancellationToken);

        if (ratings.Count == 0)
        {
            return new TenantReputationDataModel
            {
                TenantId = tenantId,
                TenantName = tenantName,
                TotalRatingsCount = 0,
                OverallAverageScore = 0m,
                ParameterAverages = [],
                RecentRatings = []
            };
        }

        var auctionIds = ratings.Select(r => r.AuctionId).Distinct().ToList();
        var submitterTenantIds = ratings.Select(r => r.SubmittedByTenant).Distinct().ToList();

        var auctionNames = await _dbContext.Auctions
            .AsNoTracking()
            .Where(a => auctionIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => a.AuctionName, cancellationToken);

        var submitterNames = await _dbContext.Tenants
            .AsNoTracking()
            .Where(t => submitterTenantIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

        var allRatingValues = ratings.SelectMany(r => r.RatingValues ?? []).ToList();

        var allScores = allRatingValues.Select(v => (decimal)v.RatingScore).ToList();
        var overallAverage = allScores.Count > 0
            ? Math.Round(allScores.Average(), 2)
            : 0m;

        var parameterAverages = allRatingValues
            .GroupBy(v => new { v.RatingParameterId, Name = v.RatingParameter?.ParameterName ?? string.Empty })
            .Select(g => new RatingParameterAverageDataModel
            {
                ParameterId = g.Key.RatingParameterId,
                ParameterName = g.Key.Name,
                AverageScore = Math.Round(g.Average(v => (decimal)v.RatingScore), 2),
                RatingCount = g.Count()
            })
            .OrderBy(p => p.ParameterId)
            .ToList();

        var recentRatings = ratings
            .OrderByDescending(r => r.AuctionId)
            .Take(10)
            .Select(r =>
            {
                var valuesForRating = (r.RatingValues ?? []).ToList();
                var avgScore = valuesForRating.Count > 0
                    ? Math.Round(valuesForRating.Average(v => (decimal)v.RatingScore), 2)
                    : 0m;

                return new RatingSummaryDataModel
                {
                    AuctionId = r.AuctionId,
                    AuctionName = auctionNames.GetValueOrDefault(r.AuctionId, $"Auction #{r.AuctionId}"),
                    SubmittedByTenant = r.SubmittedByTenant,
                    SubmittedByTenantName = submitterNames.GetValueOrDefault(r.SubmittedByTenant, $"Tenant #{r.SubmittedByTenant}"),
                    RatingForId = r.RatingForId,
                    Remark = r.Remark,
                    AverageScore = avgScore,
                    ParameterScores = valuesForRating.Select(v => new RatingValueDataModel
                    {
                        Id = v.Id,
                        RatingParameterId = v.RatingParameterId,
                        ParameterName = v.RatingParameter?.ParameterName ?? string.Empty,
                        RatingScore = v.RatingScore
                    }).ToList()
                };
            })
            .ToList();

        return new TenantReputationDataModel
        {
            TenantId = tenantId,
            TenantName = tenantName,
            TotalRatingsCount = ratings.Count,
            OverallAverageScore = overallAverage,
            ParameterAverages = parameterAverages,
            RecentRatings = recentRatings
        };
    }

    public async Task<List<RatingDataModel>> GetRatingsAgainstTenantAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var ratings = await _dbContext.Ratings
            .AsNoTracking()
            .Include(r => r.RatingValues)
                .ThenInclude(v => v.RatingParameter)
            .Where(r => r.AgainstTenant == tenantId)
            .ToListAsync(cancellationToken);

        return await BuildRatingDataModelsAsync(ratings, cancellationToken);
    }

    public async Task<List<RatingDataModel>> GetRatingsForAuctionAsync(
        int auctionId,
        CancellationToken cancellationToken = default)
    {
        var ratings = await _dbContext.Ratings
            .AsNoTracking()
            .Include(r => r.RatingValues)
                .ThenInclude(v => v.RatingParameter)
            .Where(r => r.AuctionId == auctionId)
            .ToListAsync(cancellationToken);

        return await BuildRatingDataModelsAsync(ratings, cancellationToken);
    }

    public async Task<bool> HasRatingBeenSubmittedAsync(
        int auctionId,
        short ratingForId,
        int submittedByTenant,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Ratings
            .AsNoTracking()
            .AnyAsync(
                r => r.AuctionId == auctionId &&
                     r.RatingForId == ratingForId &&
                     r.SubmittedByTenant == submittedByTenant,
                cancellationToken);
    }

    private async Task<RatingDataModel> GetRatingModelAsync(
        int auctionId,
        short ratingForId,
        int submittedByTenant,
        int againstTenant,
        CancellationToken cancellationToken)
    {
        var rating = await _dbContext.Ratings
            .AsNoTracking()
            .Include(r => r.RatingValues)
                .ThenInclude(v => v.RatingParameter)
            .FirstOrDefaultAsync(
                r => r.AuctionId == auctionId &&
                     r.RatingForId == ratingForId &&
                     r.SubmittedByTenant == submittedByTenant &&
                     r.AgainstTenant == againstTenant,
                cancellationToken);

        if (rating == null)
            throw new KeyNotFoundException("Rating record was not found after creation.");

        var list = await BuildRatingDataModelsAsync([rating], cancellationToken);
        return list.First();
    }

    private async Task<List<RatingDataModel>> BuildRatingDataModelsAsync(
        List<Rating> ratings,
        CancellationToken cancellationToken)
    {
        if (ratings.Count == 0) return [];

        var auctionIds = ratings.Select(r => r.AuctionId).Distinct().ToList();
        var tenantIds = ratings.Select(r => r.AgainstTenant)
            .Concat(ratings.Select(r => r.SubmittedByTenant))
            .Distinct()
            .ToList();

        var auctionNames = await _dbContext.Auctions
            .AsNoTracking()
            .Where(a => auctionIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, a => a.AuctionName, cancellationToken);

        var tenantNames = await _dbContext.Tenants
            .AsNoTracking()
            .Where(t => tenantIds.Contains(t.Id))
            .ToDictionaryAsync(t => t.Id, t => t.Name, cancellationToken);

        var ratingForNames = await _dbContext.RatingFors
            .AsNoTracking()
            .ToDictionaryAsync(f => f.Id, f => f.For, cancellationToken);

        return ratings.Select(r =>
        {
            var values = (r.RatingValues ?? [])
                .Select(v => new RatingValueDataModel
                {
                    Id = v.Id,
                    RatingParameterId = v.RatingParameterId,
                    ParameterName = v.RatingParameter?.ParameterName ?? string.Empty,
                    RatingScore = v.RatingScore
                })
                .ToList();

            return new RatingDataModel
            {
                RatingForId = r.RatingForId,
                RatingForName = ratingForNames.GetValueOrDefault(r.RatingForId, string.Empty),
                AuctionId = r.AuctionId,
                AuctionName = auctionNames.GetValueOrDefault(r.AuctionId, string.Empty),
                AgainstTenant = r.AgainstTenant,
                AgainstTenantName = tenantNames.GetValueOrDefault(r.AgainstTenant, string.Empty),
                SubmittedByTenant = r.SubmittedByTenant,
                SubmittedByTenantName = tenantNames.GetValueOrDefault(r.SubmittedByTenant, string.Empty),
                Remark = r.Remark,
                RatingValues = values
            };
        }).ToList();
    }
}

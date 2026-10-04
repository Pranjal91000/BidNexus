using API.Abstraction.Utilities;
using Core.Abstraction.Services;
using Core.Abstraction.Utilities;
using Core.Entities.Utilities;
using Core.Models.Utilities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Utilities;

public class RatingService(
    AppDbContext dbContext,
    IRatingRepository ratingRepository,
    IJwtHelperService jwtHelper) : IRatingService
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IRatingRepository _ratingRepository = ratingRepository;
    private readonly IJwtHelperService _jwtHelper = jwtHelper;

    public async Task<RatingDataModel> SubmitRatingAsync(
        RatingCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. User is authenticated
        var callerUserId = _jwtHelper.GetUserId();
        var callerTenantId = _jwtHelper.GetTenantId();
        var callerRole = _jwtHelper.GetRole();

        if (callerUserId <= 0 || callerTenantId <= 0)
            throw new UnauthorizedAccessException("Authenticated user and tenant context are required.");

        // 2. User's tenant is valid and active
        var callerTenant = await _dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == callerTenantId, cancellationToken);

        if (callerTenant == null || callerTenant.IsBlocked)
            throw new UnauthorizedAccessException("The authenticated tenant is invalid or blocked.");

        // 3. Auction exists
        var auction = await _dbContext.Auctions
            .AsNoTracking()
            .Include(a => a.Status)
            .Include(a => a.Organization)
            .FirstOrDefaultAsync(a => a.Id == request.AuctionId, cancellationToken);

        if (auction == null)
            throw new KeyNotFoundException($"Auction with ID {request.AuctionId} not found.");

        // 5. Auction is Completed
        var statusName = auction.Status?.Name?.Trim() ?? string.Empty;
        if (!string.Equals(statusName, "Completed", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Ratings can only be submitted for completed auctions. Current auction status is '{statusName}'.");
        }

        // Fetch winning statement for the auction
        var winningStatement = await _dbContext.AuctionStatements
            .AsNoTracking()
            .Include(s => s.Vendor)
            .FirstOrDefaultAsync(
                s => s.AuctionId == request.AuctionId && s.IsWinner,
                cancellationToken);

        if (winningStatement == null)
        {
            throw new InvalidOperationException(
                "Auction statement or official winner has not been recorded for this completed auction.");
        }

        short ratingForId;
        int targetTenantId;

        var isOrg = string.Equals(callerRole, "Organization", StringComparison.OrdinalIgnoreCase) ||
                    (!callerTenant.IsVendor);

        if (isOrg)
        {
            // 4 & 6 & 7: Organization rates the winning Vendor
            if (auction.TenantId != callerTenantId)
            {
                throw new UnauthorizedAccessException(
                    "Only the Organization that created this auction is permitted to rate the winning vendor.");
            }

            var winningVendor = winningStatement.Vendor;
            if (winningVendor == null)
            {
                throw new InvalidOperationException("Winning vendor record could not be resolved.");
            }

            ratingForId = 1; // Vendor
            targetTenantId = winningVendor.TenantId;

            // 13: Prevent client from supplying unrelated tenant ID
            if (request.AgainstTenant.HasValue && request.AgainstTenant.Value != targetTenantId)
            {
                throw new InvalidOperationException(
                    "You cannot rate a tenant that is not the official winning vendor of this auction.");
            }

            // 9: RatingForId matches the direction
            if (request.RatingForId.HasValue && request.RatingForId.Value != ratingForId)
            {
                throw new InvalidOperationException(
                    $"Invalid RatingForId {request.RatingForId.Value}. Organization rating winning vendor must specify RatingForId = 1.");
            }
        }
        else
        {
            // 6 & 8: Winning Vendor rates the Organization that created the auction
            if (winningStatement.VendorId != callerUserId)
            {
                throw new UnauthorizedAccessException(
                    "Only the winning vendor of this auction is authorized to rate the creating organization.");
            }

            ratingForId = 2; // Organization
            targetTenantId = auction.TenantId;

            // 13: Prevent client from supplying unrelated tenant ID
            if (request.AgainstTenant.HasValue && request.AgainstTenant.Value != targetTenantId)
            {
                throw new InvalidOperationException(
                    "You cannot rate a tenant that is not the creating organization of this auction.");
            }

            // 9: RatingForId matches the direction
            if (request.RatingForId.HasValue && request.RatingForId.Value != ratingForId)
            {
                throw new InvalidOperationException(
                    $"Invalid RatingForId {request.RatingForId.Value}. Winning vendor rating organization must specify RatingForId = 2.");
            }
        }

        // 12: A participant cannot submit the same rating direction twice for the same auction
        var alreadySubmitted = await _ratingRepository.HasRatingBeenSubmittedAsync(
            request.AuctionId,
            ratingForId,
            callerTenantId,
            cancellationToken);

        if (alreadySubmitted)
        {
            throw new InvalidOperationException(
                "A rating for this auction and direction has already been submitted by your tenant.");
        }

        // 10: Every supplied RatingParameterId belongs to the selected RatingFor
        if (request.RatingValues == null || request.RatingValues.Count == 0)
        {
            throw new InvalidOperationException("At least one rating parameter score must be supplied.");
        }

        var validParameters = await _dbContext.RatingParameters
            .AsNoTracking()
            .Where(p => p.RatingForId == ratingForId && !p.Inactive)
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var suppliedParameterIds = new HashSet<short>();

        foreach (var val in request.RatingValues)
        {
            if (!validParameters.ContainsKey(val.RatingParameterId))
            {
                throw new InvalidOperationException(
                    $"RatingParameter ID {val.RatingParameterId} is invalid or does not belong to the selected RatingFor ({ratingForId}).");
            }

            if (!suppliedParameterIds.Add(val.RatingParameterId))
            {
                throw new InvalidOperationException(
                    $"RatingParameter ID {val.RatingParameterId} was supplied more than once.");
            }

            // 11: Rating scores are within the defined allowed range (1 to 5)
            if (val.RatingScore < 1 || val.RatingScore > 5)
            {
                throw new InvalidOperationException(
                    $"Rating score for parameter {val.RatingParameterId} must be between 1 and 5. Received: {val.RatingScore}.");
            }
        }

        // 4. Rating Persistence
        var rating = new Rating
        {
            RatingForId = ratingForId,
            AuctionId = request.AuctionId,
            AgainstTenant = targetTenantId,
            SubmittedByTenant = callerTenantId,
            Remark = string.IsNullOrWhiteSpace(request.Remark) ? null : request.Remark.Trim(),
            RatingValues = request.RatingValues.Select(v => new RatingValue
            {
                RatingParameterId = v.RatingParameterId,
                RatingScore = v.RatingScore
            }).ToList()
        };

        return await _ratingRepository.SubmitRatingAsync(rating, cancellationToken);
    }

    public Task<TenantReputationDataModel> GetTenantReputationAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0)
            throw new ArgumentException("TenantId must be greater than zero.", nameof(tenantId));

        return _ratingRepository.GetTenantReputationAsync(tenantId, cancellationToken);
    }

    public Task<List<RatingDataModel>> GetRatingsAgainstTenantAsync(
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        if (tenantId <= 0)
            throw new ArgumentException("TenantId must be greater than zero.", nameof(tenantId));

        return _ratingRepository.GetRatingsAgainstTenantAsync(tenantId, cancellationToken);
    }

    public Task<List<RatingDataModel>> GetRatingsForAuctionAsync(
        int auctionId,
        CancellationToken cancellationToken = default)
    {
        if (auctionId <= 0)
            throw new ArgumentException("AuctionId must be greater than zero.", nameof(auctionId));

        return _ratingRepository.GetRatingsForAuctionAsync(auctionId, cancellationToken);
    }

    public Task<TenantReputationDataModel> GetMyReputationAsync(
        CancellationToken cancellationToken = default)
    {
        var tenantId = _jwtHelper.GetTenantId();
        if (tenantId <= 0)
            throw new UnauthorizedAccessException("Authenticated tenant context is missing.");

        return _ratingRepository.GetTenantReputationAsync(tenantId, cancellationToken);
    }
}

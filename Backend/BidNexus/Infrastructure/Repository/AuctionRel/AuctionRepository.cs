using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Services;
using Core.Entities.Auction;
using Core.Models.AuctionRelated;
using Core.Models.Master;
using Core.Models.Models;
using Microsoft.EntityFrameworkCore;
using Core.Enumeration;

namespace Infrastructure.Repository.AuctionRel
{
    public class AuctionRepository(AppDbContext dbContext, IJwtHelperService jwtHelperService) : IAuctionRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly IJwtHelperService jwtHelper = jwtHelperService;

        public async Task<AuctionResponseModel> AddAsync(Auction auction)
        {
            if (auction.TenantId == 0)
            {
                auction.TenantId = jwtHelper.GetTenantId();
            }

            if (auction.CreatedDateTime == default)
            {
                auction.CreatedDateTime = DateTimeOffset.UtcNow;
            }

            if (auction.LastModifiedDateTime == default)
            {
                auction.LastModifiedDateTime = DateTimeOffset.UtcNow;
            }

            if (auction.AuctionRequirements != null)
            {
                foreach (var req in auction.AuctionRequirements)
                {
                    if (req.TenantId == 0)
                    {
                        req.TenantId = auction.TenantId;
                    }
                }
            }

            await _dbContext.Auctions.AddAsync(auction);
            var success = await _dbContext.SaveChangesAsync() > 0;

            if (!success) throw new InvalidOperationException("Failed to Save Auction");

            return new AuctionResponseModel
            {
                Id = auction.Id,
                DocDate = auction.DocDate,
                DocNoYearly = auction.DocNoYearly,
                OrganizationId = auction.OrganizationId != 0 ? auction.OrganizationId : auction.Organization?.Id ?? 0
            };
        }

        public async Task<AuctionResponseModel> UpdateAsync(Auction auction)
        {
            var tenantId = jwtHelper.GetTenantId();
            var existingAuction = await _dbContext.Auctions
                .Include(x => x.AuctionRequirements)
                .FirstOrDefaultAsync(x => x.Id == auction.Id && x.TenantId == tenantId);

            if (existingAuction == null) throw new InvalidOperationException("Auction Not Found");

            existingAuction.Update(
                auction.AuctionName,
                auction.About,
                auction.DocNoYearly,
                auction.DocDate,
                auction.IsForwardAuction,
                auction.AuctionStartTime,
                auction.AuctionEndTime,
                auction.DocAttachmentId,
                auction.OpenToAll,
                auction.IsBidPriceHidden,
                auction.OrganizationId,
                auction.StatusId);

            if (auction.AuctionRequirements != null)
            {
                _dbContext.AuctionRequirements.RemoveRange(existingAuction.AuctionRequirements);
                foreach (var req in auction.AuctionRequirements)
                {
                    req.AuctionId = existingAuction.Id;
                    req.TenantId = tenantId;
                    existingAuction.AuctionRequirements.Add(req);
                }
            }

            var success = await _dbContext.SaveChangesAsync() > 0;

            if (!success) throw new InvalidOperationException("Failed to Update Auction");

            return new AuctionResponseModel
            {
                Id = existingAuction.Id,
                DocDate = existingAuction.DocDate,
                DocNoYearly = existingAuction.DocNoYearly,
                OrganizationId = existingAuction.OrganizationId != 0 ? existingAuction.OrganizationId : auction.Organization?.Id ?? 0
            };
        }

        public async Task<AuctionDataModel> GetById(int Id)
        {
            var auction = await _dbContext.Auctions
                .AsNoTracking()
                .Include(a => a.Organization)
                .Include(a => a.Status)
                .Include(a => a.AuctionRequirements)
                    .ThenInclude(r => r.Item)
                .Include(a => a.AuctionRequirements)
                    .ThenInclude(r => r.Unit)
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (auction == null) throw new KeyNotFoundException($"Auction with the provided {Id} not found.");

            return new AuctionDataModel
            {
                Id = auction.Id,
                AuctionName = auction.AuctionName,
                About = auction.About,
                DocNoYearly = auction.DocNoYearly,
                DocDate = auction.DocDate,
                IsForwardAuction = auction.IsForwardAuction,
                AuctionStartTime = auction.AuctionStartTime,
                AuctionEndTime = auction.AuctionEndTime,
                DocAttachmentId = auction.DocAttachmentId,
                OpenToAll = auction.OpenToAll,
                IsBidPriceHidden = auction.IsBidPriceHidden,
                OrganizationId = auction.OrganizationId,
                Organization = auction.Organization,
                StatusId = auction.StatusId,
                StatusName = auction.Status?.Name ?? string.Empty,
                AuctionRequirements = auction.AuctionRequirements?.Select(r => new AuctionRequirementDataModel
                {
                    Id = r.Id,
                    LineNo = r.LineNo,
                    AuctionId = r.AuctionId,
                    ItemId = r.ItemId,
                    TechnicalSpecification = r.TechnicalSpecification,
                    Quantity = r.Quantity,
                    UnitId = r.UnitId,
                    DocumentAttachmentId = r.DocumentAttachmentId,
                    Item = new ItemDataModel
                    {
                        Id = r.Item.Id,
                        Name = r.Item.Name,
                        Code = r.Item.Code,
                        CategoryId = r.Item.CategoryId
                    },
                    Unit = new UnitDataModel
                    {
                        Id = r.Unit.Id,
                        Name = r.Unit.Name,
                        Code = r.Unit.Code
                    }
                }).ToList() ?? new List<AuctionRequirementDataModel>(),
                CreatedDateTime = auction.CreatedDateTime,
                LastModifiedDateTime = auction.LastModifiedDateTime
            };
        }

        public async Task<List<AuctionLifecycleDataModel>> GetAuctionsForLifecycleAsync(
            DateTimeOffset now,
            string scheduledStatusName,
            string activeStatusName,
            string completedStatusName)
        {
            var normalizedScheduled = scheduledStatusName.Trim();
            var normalizedActive = activeStatusName.Trim();
            var normalizedCompleted = completedStatusName.Trim();

            return await _dbContext.Auctions
                .AsNoTracking()
                .Include(a => a.Status)
                .Where(a =>
                    (a.StatusId == (short)StatusEnum.Scheduled && a.AuctionStartTime <= now) ||
                    (a.StatusId == (short)StatusEnum.Open && a.AuctionEndTime <= now) ||
                    (a.StatusId == (short)StatusEnum.Completed && !_dbContext.AuctionStatements.Any(s => s.AuctionId == a.Id)))
                .Select(a => new AuctionLifecycleDataModel
                {
                    Id = a.Id,
                    TenantId = a.TenantId,
                    StatusId = a.StatusId,
                    StatusName = a.Status.Name,
                    AuctionStartTime = a.AuctionStartTime,
                    AuctionEndTime = a.AuctionEndTime,
                    ShouldStart = (a.StatusId == (short)StatusEnum.Authorized || a.StatusId == (short)StatusEnum.Scheduled) &&
                                  a.AuctionStartTime <= now &&
                                  a.AuctionEndTime > now,
                    ShouldComplete = (a.StatusId == (short)StatusEnum.Authorized ||
                                      a.StatusId == (short)StatusEnum.Scheduled ||
                                      a.StatusId == (short)StatusEnum.Open) &&
                                     a.AuctionEndTime <= now,
                    NeedsStatementGeneration = a.StatusId == (short)StatusEnum.Completed &&
                                               !_dbContext.AuctionStatements.Any(s => s.AuctionId == a.Id)
                })
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(int auctionId, short statusId)
        {
            var auction = await _dbContext.Auctions
                .FirstOrDefaultAsync(a => a.Id == auctionId);

            if (auction == null)
                return;

            auction.StatusId = statusId;
            auction.LastModifiedDateTime = DateTimeOffset.UtcNow;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int auctionId, string statusName)
        {
            var auction = await _dbContext.Auctions
                .FirstOrDefaultAsync(a => a.Id == auctionId);

            if (auction == null)
                return;

            var status = await _dbContext.Statuses
                .FirstOrDefaultAsync(s => s.Name == statusName.Trim() && !s.Inactive);

            if (status == null)
                throw new InvalidOperationException(
                    $"Auction status '{statusName}' was not found or is inactive.");

            auction.StatusId = status.Id;
            auction.LastModifiedDateTime = DateTimeOffset.UtcNow;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> CanAccessAuctionAsync(
            int auctionId,
            int userId,
            int tenantId,
            string role,
            CancellationToken cancellationToken = default)
        {
            if (string.Equals(role, "Organization", StringComparison.OrdinalIgnoreCase))
            {
                return await _dbContext.Auctions.AnyAsync(
                    a => a.Id == auctionId && a.TenantId == tenantId,
                    cancellationToken);
            }

            if (string.Equals(role, "Vendor", StringComparison.OrdinalIgnoreCase))
            {
                var auction = await _dbContext.Auctions
                    .AsNoTracking()
                    .Where(a => a.Id == auctionId)
                    .Select(a => a.OpenToAll)
                    .FirstOrDefaultAsync(cancellationToken);


                var vendorExists = await _dbContext.Vendors
                    .AsNoTracking()
                    .AnyAsync(
                        v => v.Id == userId && v.TenantId == tenantId,
                        cancellationToken);

                if (!vendorExists)
                    return false;

                var intent = await _dbContext.VendorIntents
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.AuctionId == auctionId &&
                             x.VendorId == userId &&
                             x.TenantId == tenantId,
                        cancellationToken);

                if(auction)
                {
                    return true;
                }

                return intent == null
                    ? auction
                    : intent.IsInterested && intent.IsQualified;
            }

            return false;
        }

        public async Task<bool> DeleteAsync(int Id)
        {
            var auction = await _dbContext.Auctions
                .Where(x => x.TenantId == jwtHelper.GetTenantId())
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (auction == null) throw new KeyNotFoundException($"Auction with the provided {Id} not found.");

            _dbContext.Auctions.Remove(auction);
            var success = await _dbContext.SaveChangesAsync() > 0;
            if (!success) throw new InvalidOperationException("Failed to Delete Auction");
            return true;
        }

        public async Task<List<AuctionGetDataModel>> Get(short categoryId, short pageNo, short pageSize)
        {
            var query = _dbContext.Auctions
                .AsNoTracking()
                .Where(a => a.TenantId == jwtHelper.GetTenantId());

            if (categoryId > 0)
            {
                query = query.Where(a => a.AuctionRequirements.Any(r => r.Item.CategoryId == categoryId));
            }

            var currentPage = pageNo > 0 ? pageNo : 1;
            var takeSize = pageSize > 0 ? pageSize : 10;
            var skip = (currentPage - 1) * takeSize;

            return await query
                .OrderByDescending(a => a.Id)
                .Skip(skip)
                .Take(takeSize)
                .Select(a => new AuctionGetDataModel
                {
                    Id = a.Id,
                    AuctionName = a.AuctionName,
                    About = a.About,
                    DocNoYearly = a.DocNoYearly,
                    DocDate = a.DocDate,
                    IsForwardAuction = a.IsForwardAuction,
                    StatusName = a.Status.Name,
                    StatusId = a.StatusId,
                    AuctionStartTime = a.AuctionStartTime,
                    AuctionEndTime = a.AuctionEndTime,
                    OpenToAll = a.OpenToAll,
                    IsBidPriceHidden = a.IsBidPriceHidden,
                    OrganizationId = a.OrganizationId,
                    AuctionIntentSubmissionDate = a.AuctionIntentSubmissionDate,
                    Organization = new OrganizationOverviewDataModel
                    {
                        Name = a.Organization.Name,
                        OfficialAddress = a.Organization.OfficialAddress,
                        ForegroundImageId = a.Organization.ForegroundImageId,
                        About = a.Organization.About
                    },
                    AuctionRequirements = a.AuctionRequirements.Select(r => new AuctionRequirementDataModel
                    {
                        Id = r.Id,
                        LineNo = r.LineNo,
                        AuctionId = r.AuctionId,
                        ItemId = r.ItemId,
                        TechnicalSpecification = r.TechnicalSpecification,
                        Quantity = r.Quantity,
                        UnitId = r.UnitId,
                        DocumentAttachmentId = r.DocumentAttachmentId,
                        Item = new ItemDataModel
                        {
                            Id = r.Item.Id,
                            Name = r.Item.Name,
                            Code = r.Item.Code,
                            CategoryId = r.Item.CategoryId
                        },
                        Unit = new UnitDataModel
                        {
                            Id = r.Unit.Id,
                            Name = r.Unit.Name,
                            Code = r.Unit.Code
                        }
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<List<AuctionGetDataModel>> GetPendingAuctionsAsync(short categoryId, short pageNo, short pageSize, CancellationToken cancellationToken = default)
        {
            // Provides all auctions that are NOT in Draft (1) and NOT Completed (7) across all organizations without tenant filtering
            var query = _dbContext.Auctions
                .AsNoTracking()
                .Where(a => a.StatusId != 1 && a.StatusId != 7 && a.Status.Name != "Draft" && a.Status.Name != "Completed");

            if (categoryId > 0)
            {
                query = query.Where(a => a.AuctionRequirements.Any(r => r.Item.CategoryId == categoryId));
            }

            var currentPage = pageNo > 0 ? pageNo : 1;
            var takeSize = pageSize > 0 ? pageSize : 10;
            var skip = (currentPage - 1) * takeSize;

            return await query
                .OrderByDescending(a => a.Id)
                .Skip(skip)
                .Take(takeSize)
                .Select(a => new AuctionGetDataModel
                {
                    Id = a.Id,
                    AuctionName = a.AuctionName,
                    About = a.About,
                    DocNoYearly = a.DocNoYearly,
                    DocDate = a.DocDate,
                    IsForwardAuction = a.IsForwardAuction,
                    StatusName = a.Status.Name,
                    StatusId = a.StatusId,
                    AuctionStartTime = a.AuctionStartTime,
                    AuctionEndTime = a.AuctionEndTime,
                    OpenToAll = a.OpenToAll,
                    IsBidPriceHidden = a.IsBidPriceHidden,
                    OrganizationId = a.OrganizationId,
                    AuctionIntentSubmissionDate = a.AuctionIntentSubmissionDate,
                    Organization = new OrganizationOverviewDataModel
                    {
                        Name = a.Organization.Name,
                        OfficialAddress = a.Organization.OfficialAddress,
                        ForegroundImageId = a.Organization.ForegroundImageId,
                        About = a.Organization.About
                    },
                    AuctionRequirements = a.AuctionRequirements.Select(r => new AuctionRequirementDataModel
                    {
                        Id = r.Id,
                        LineNo = r.LineNo,
                        AuctionId = r.AuctionId,
                        ItemId = r.ItemId,
                        TechnicalSpecification = r.TechnicalSpecification,
                        Quantity = r.Quantity,
                        UnitId = r.UnitId,
                        DocumentAttachmentId = r.DocumentAttachmentId,
                        Item = new ItemDataModel
                        {
                            Id = r.Item.Id,
                            Name = r.Item.Name,
                            Code = r.Item.Code,
                            CategoryId = r.Item.CategoryId
                        },
                        Unit = new UnitDataModel
                        {
                            Id = r.Unit.Id,
                            Name = r.Unit.Name,
                            Code = r.Unit.Code
                        }
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}

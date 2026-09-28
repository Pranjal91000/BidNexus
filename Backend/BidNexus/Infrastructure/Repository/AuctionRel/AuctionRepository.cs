using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Services;
using Core.Entities.Auction;
using Core.Models.AuctionRelated;
using Core.Models.Master;
using Core.Models.Models;
using Microsoft.EntityFrameworkCore;

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
                .Include(a => a.AuctionRequirements)
                    .ThenInclude(r => r.Item)
                .Include(a => a.AuctionRequirements)
                    .ThenInclude(r => r.Unit)
                .Where(x => x.TenantId == jwtHelper.GetTenantId())
                .FirstOrDefaultAsync(x => x.Id == Id);

            if (auction == null) throw new KeyNotFoundException($"Auction with the provided {Id} not found.");

            return new AuctionDataModel
            {
                Id = auction.Id,
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
                AuctionRequirements = auction.AuctionRequirements?.ToList() ?? new List<AuctionRequirement>(),
                CreatedDateTime = auction.CreatedDateTime,
                LastModifiedDateTime = auction.LastModifiedDateTime
            };
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
                    DocNoYearly = a.DocNoYearly,
                    DocDate = a.DocDate,
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
    }
}

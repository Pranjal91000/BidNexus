using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Services;
using Core.Entities.Auction;
using Core.Models.AuctionRelated;
using Core.Models.GlobalData;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.AuctionRel
{
    public class BiddingRepository(AppDbContext appDbContext, IJwtHelperService jwtHelperService) : IBidRepository
    {
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly IJwtHelperService _jwtHelper = jwtHelperService;

        private int CurrentTenantId => _jwtHelper.GetTenantId();

        public async Task<BidProcessingContext> GetBidProcessingContextAsync(
            int auctionId,
            int vendorId,
            IReadOnlyCollection<int> requirementIds,
            IReadOnlyCollection<int> taxIds,
            IReadOnlyCollection<short> taxNatureIds,
            IReadOnlyCollection<short> chargeTypeIds,
            int tenantId,
            bool isForwardAuction,
            CancellationToken cancellationToken = default)
        {
            var auctionForTenant = await _appDbContext.Auctions
                .AsNoTracking()
                .Where(a => a.Id == auctionId)
                .Select(a => new { a.Id, a.OpenToAll })
                .FirstOrDefaultAsync(cancellationToken);

            var vendorExistsForTenant = await _appDbContext.Vendors
                .AsNoTracking()
                .AnyAsync(
                    v => v.Id == vendorId && v.TenantId == tenantId,
                    cancellationToken);

            var vendorIntent = await _appDbContext.VendorIntents
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.AuctionId == auctionId &&
                         x.VendorId == vendorId &&
                         x.TenantId == tenantId,
                    cancellationToken);

            var requirements = await _appDbContext.AuctionRequirements
                .AsNoTracking()
                .Where(x =>
                    x.AuctionId == auctionId &&
                    requirementIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var taxMasters = taxIds.Count == 0
                ? []
                : await _appDbContext.TaxMasters
                    .AsNoTracking()
                    .Where(x => taxIds.Contains(x.Id) && !x.Status.Inactive)
                    .ToListAsync(cancellationToken);

            var resolvedTaxNatureIds = taxNatureIds
                .Concat(taxMasters.Select(x => x.TaxNatureId))
                .Distinct()
                .ToArray();

            var resolvedChargeTypeIds = chargeTypeIds
                .Concat(taxMasters.Select(x => x.ChargeTypeId))
                .Distinct()
                .ToArray();

            var taxNatures = resolvedTaxNatureIds.Length == 0
                ? []
                : await _appDbContext.TaxNatures
                    .AsNoTracking()
                    .Where(x => resolvedTaxNatureIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);

            var chargeTypes = resolvedChargeTypeIds.Length == 0
                ? []
                : await _appDbContext.ChargeTypes
                    .AsNoTracking()
                    .Where(x => resolvedChargeTypeIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);

            var currentBids = _appDbContext.Bids
                .AsNoTracking()
                .Where(x =>
                    x.AuctionId == auctionId &&
                    x.IsCurrent &&
                    x.Auction.Organization.TenantId == tenantId);

            var currentBestNetAmount = isForwardAuction
                ? await currentBids
                    .OrderByDescending(x => x.NetAmount)
                    .Select(x => (decimal?)x.NetAmount)
                    .FirstOrDefaultAsync(cancellationToken)
                : await currentBids
                    .OrderBy(x => x.NetAmount)
                    .Select(x => (decimal?)x.NetAmount)
                    .FirstOrDefaultAsync(cancellationToken);

            return new BidProcessingContext
            {
                IsForwardAuction = isForwardAuction,
                AuctionExistsForTenant = auctionForTenant != null,
                OpenToAll = auctionForTenant?.OpenToAll ?? false,
                VendorExistsForTenant = vendorExistsForTenant,
                VendorIntent = vendorIntent,
                Requirements = requirements,
                TaxMasters = taxMasters,
                TaxNatures = taxNatures,
                ChargeTypes = chargeTypes,
                CurrentBestNetAmount = currentBestNetAmount
            };
        }

        public async Task<BidResponseDataModel> ProcessBidAsync(Bid currbid, long? invalidateBidId, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _appDbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                
                var bidToInvalidate = invalidateBidId.HasValue
                    ? await _appDbContext.Bids.FindAsync([invalidateBidId.Value], cancellationToken)
                    : null;

                if (bidToInvalidate != null)
                {
                    currbid.MainBidId = bidToInvalidate.MainBidId ?? bidToInvalidate.Id;
                    currbid.BidRevisionNo = (short)(bidToInvalidate.BidRevisionNo + 1);
                }

                await _appDbContext.Bids.AddAsync(currbid, cancellationToken);
                await _appDbContext.SaveChangesAsync(cancellationToken);

                if (bidToInvalidate == null)
                {
                    currbid.InitializeAsMainBid();
                    await _appDbContext.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    bidToInvalidate.IsCurrent = false;
                    await _appDbContext.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);

                return new BidResponseDataModel
                {
                    Id = currbid.Id,
                    MainBidId = currbid.MainBidId,
                    AuctionId = currbid.AuctionId,
                    VendorId = currbid.VendorId,
                    NetAmount = currbid.NetAmount,
                    BidRevisionNo = currbid.BidRevisionNo
                };
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }
        public async Task<List<BidDataModel>> GetAuctionBidsAsync(int auctionId)
        {
            var tenantId = CurrentTenantId;
            if (tenantId > 0)
            {
                var hasAccess = await _appDbContext.Auctions
                    .AsNoTracking()
                    .Where(a => a.Id == auctionId)
                    .AnyAsync(a => a.Organization.TenantId == tenantId || a.OpenToAll ||
                        _appDbContext.VendorIntents.Any(vi => vi.AuctionId == auctionId && vi.TenantId == tenantId && vi.IsInterested && vi.IsQualified));

                if (!hasAccess)
                    return [];
            }

            var data = await _appDbContext.Bids
                .AsNoTracking()
                .Where(x => x.AuctionId == auctionId && x.IsCurrent)
                .Select(x => new BidDataModel
                {
                    Id = x.Id,
                    IsCurrent = x.IsCurrent,
                    MainBidId = x.MainBidId,
                    AuctionId = x.AuctionId,
                    VendorId = x.VendorId,
                    BasicAmount = x.BasicAmount,
                    TaxAmount = x.TaxAmount,
                    DiscountAmount = x.DiscountAmount,
                    NetAmount = x.NetAmount,
                    CreatedAt = x.CreatedAt,
                    BidRevisionNo = x.BidRevisionNo,
                    Vendor = new Core.Models.Tenant.VendorDataModel
                    {
                        Id = x.Vendor.Id,
                        Name = x.Vendor.Name,
                        About = x.Vendor.About
                    },
                    BidDetails = x.BidDetails.Select(bd => new BidDetailDataModel
                    {
                        Id = bd.Id,
                        BidId = bd.BidId,
                        AuctionRequirementId = bd.AuctionRequirementId,
                        Rate = bd.Rate,
                        BaseAmount = bd.BaseAmount,
                        NetAmount = bd.NetAmount,
                        Taxes = bd.Taxes.Select(t => new BidTaxDetailDataModel
                        {
                            Id = t.Id,
                            BidDetailId = t.BidDetailId,
                            TaxId = t.TaxId,
                            TaxName = t.TaxName,
                            TaxCode = t.TaxCode,
                            TaxNatureId = t.TaxNatureId,
                            ChargeTypeId = t.ChargeTypeId,
                            TaxNature = new TaxNatureDataModel
                            {
                                Id = t.TaxNatureId,
                                Name = t.TaxNature.Name,
                                Code = t.TaxNature.Code,
                                IsActive = t.TaxNature.IsActive
                            },
                            ChargeType = new ChargeTypeDataModel
                            {
                                Id = t.ChargeTypeId,
                                Name = t.ChargeType.Name,
                                Code = t.ChargeType.Code,
                                IsActive = t.ChargeType.IsActive
                            },
                            TaxValue = t.TaxValue,
                            TaxAmount = t.TaxAmount
                        }).ToList()
                    }).ToList()
                }).ToListAsync();

            return data;
        }

        public async Task<List<BidDataModel>> GetBidHistory(int vendorId, int auctionId)
        {
            var tenantId = CurrentTenantId;
            var query = _appDbContext.Bids
                .AsNoTracking()
                .Where(x => x.AuctionId == auctionId && x.VendorId == vendorId);

            if (tenantId > 0)
            {
                query = query.Where(x => x.Auction.Organization.TenantId == tenantId || (x.Vendor.TenantId == tenantId && x.VendorId == vendorId));
            }

            var data = await query
                .OrderByDescending(x => x.BidRevisionNo)
                .Select(x => new BidDataModel
                {
                    Id = x.Id,
                    IsCurrent = x.IsCurrent,
                    MainBidId = x.MainBidId,
                    AuctionId = x.AuctionId,
                    VendorId = x.VendorId,
                    BasicAmount = x.BasicAmount,
                    TaxAmount = x.TaxAmount,
                    DiscountAmount = x.DiscountAmount,
                    NetAmount = x.NetAmount,
                    CreatedAt = x.CreatedAt,
                    BidRevisionNo = x.BidRevisionNo,
                    Vendor = new Core.Models.Tenant.VendorDataModel
                    {
                        Id = x.Vendor.Id,
                        Name = x.Vendor.Name,
                        About = x.Vendor.About
                    },
                    BidDetails = x.BidDetails.Select(bd => new BidDetailDataModel
                    {
                        Id = bd.Id,
                        BidId = bd.BidId,
                        AuctionRequirementId = bd.AuctionRequirementId,
                        Rate = bd.Rate,
                        BaseAmount = bd.BaseAmount,
                        NetAmount = bd.NetAmount,
                        Taxes = bd.Taxes.Select(t => new BidTaxDetailDataModel
                        {
                            Id = t.Id,
                            BidDetailId = t.BidDetailId,
                            TaxId = t.TaxId,
                            TaxName = t.TaxName,
                            TaxCode = t.TaxCode,
                            TaxNatureId = t.TaxNatureId,
                            ChargeTypeId = t.ChargeTypeId,
                            TaxNature = new TaxNatureDataModel
                            {
                                Id = t.TaxNatureId,
                                Name = t.TaxNature.Name,
                                Code = t.TaxNature.Code,
                                IsActive = t.TaxNature.IsActive
                            },
                            ChargeType = new ChargeTypeDataModel
                            {
                                Id = t.ChargeTypeId,
                                Name = t.ChargeType.Name,
                                Code = t.ChargeType.Code,
                                IsActive = t.ChargeType.IsActive
                            },
                            TaxValue = t.TaxValue,
                            TaxAmount = t.TaxAmount
                        }).ToList()
                    }).ToList()
                }).ToListAsync();

            return data;
        }

        public async Task<List<BidDataModel>> GetLeaderBoard(int auctionId)
        {
            var tenantId = CurrentTenantId;
            if (tenantId > 0)
            {
                var hasAccess = await _appDbContext.Auctions
                    .AsNoTracking()
                    .Where(a => a.Id == auctionId)
                    .AnyAsync(a => a.Organization.TenantId == tenantId || a.OpenToAll ||
                        _appDbContext.VendorIntents.Any(vi => vi.AuctionId == auctionId && vi.TenantId == tenantId && vi.IsInterested && vi.IsQualified));

                if (!hasAccess)
                    return [];
            }

            var isForwardAuction = await _appDbContext.Auctions
                .Where(a => a.Id == auctionId)
                .Select(a => a.IsForwardAuction)
                .FirstOrDefaultAsync();

            var query = _appDbContext.Bids
                .AsNoTracking()
                .Where(x => x.AuctionId == auctionId && x.IsCurrent);

            query = isForwardAuction
                ? query.OrderByDescending(x => x.NetAmount).ThenBy(x => x.CreatedAt)
                : query.OrderBy(x => x.NetAmount).ThenBy(x => x.CreatedAt);

            var data = await query
                .Select(x => new BidDataModel
                {
                    Id = x.Id,
                    IsCurrent = x.IsCurrent,
                    MainBidId = x.MainBidId,
                    AuctionId = x.AuctionId,
                    VendorId = x.VendorId,
                    BasicAmount = x.BasicAmount,
                    TaxAmount = x.TaxAmount,
                    DiscountAmount = x.DiscountAmount,
                    NetAmount = x.NetAmount,
                    CreatedAt = x.CreatedAt,
                    BidRevisionNo = x.BidRevisionNo,
                    Vendor = new Core.Models.Tenant.VendorDataModel
                    {
                        Id = x.Vendor.Id,
                        Name = x.Vendor.Name,
                        About = x.Vendor.About
                    },
                    BidDetails = x.BidDetails.Select(bd => new BidDetailDataModel
                    {
                        Id = bd.Id,
                        BidId = bd.BidId,
                        AuctionRequirementId = bd.AuctionRequirementId,
                        Rate = bd.Rate,
                        BaseAmount = bd.BaseAmount,
                        NetAmount = bd.NetAmount,
                        Taxes = bd.Taxes.Select(t => new BidTaxDetailDataModel
                        {
                            Id = t.Id,
                            BidDetailId = t.BidDetailId,
                            TaxId = t.TaxId,
                            TaxName = t.TaxName,
                            TaxCode = t.TaxCode,
                            TaxNatureId = t.TaxNatureId,
                            ChargeTypeId = t.ChargeTypeId,
                            TaxNature = new TaxNatureDataModel
                            {
                                Id = t.TaxNatureId,
                                Name = t.TaxNature.Name,
                                Code = t.TaxNature.Code,
                                IsActive = t.TaxNature.IsActive
                            },
                            ChargeType = new ChargeTypeDataModel
                            {
                                Id = t.ChargeTypeId,
                                Name = t.ChargeType.Name,
                                Code = t.ChargeType.Code,
                                IsActive = t.ChargeType.IsActive
                            },
                            TaxValue = t.TaxValue,
                            TaxAmount = t.TaxAmount
                        }).ToList()
                    }).ToList()
                }).ToListAsync();

            return data;
        }

        public async Task<BidDataModel> GetById(long BidId)
        {
            var tenantId = CurrentTenantId;
            var query = _appDbContext.Bids
                .AsNoTracking()
                .Where(x => x.Id == BidId);

            if (tenantId > 0)
            {
                query = query.Where(x => x.Auction.Organization.TenantId == tenantId || x.Vendor.TenantId == tenantId);
            }

            var bid = await query
                .Select(x => new BidDataModel
                {
                    Id = x.Id,
                    IsCurrent = x.IsCurrent,
                    MainBidId = x.MainBidId,
                    AuctionId = x.AuctionId,
                    VendorId = x.VendorId,
                    BasicAmount = x.BasicAmount,
                    TaxAmount = x.TaxAmount,
                    DiscountAmount = x.DiscountAmount,
                    NetAmount = x.NetAmount,
                    CreatedAt = x.CreatedAt,
                    BidRevisionNo = x.BidRevisionNo,
                    Vendor = new Core.Models.Tenant.VendorDataModel
                    {
                        Id = x.Vendor.Id,
                        Name = x.Vendor.Name,
                        About = x.Vendor.About
                    },
                    BidDetails = x.BidDetails.Select(bd => new BidDetailDataModel
                    {
                        Id = bd.Id,
                        BidId = bd.BidId,
                        AuctionRequirementId = bd.AuctionRequirementId,
                        Rate = bd.Rate,
                        BaseAmount = bd.BaseAmount,
                        NetAmount = bd.NetAmount,
                        Taxes = bd.Taxes.Select(t => new BidTaxDetailDataModel
                        {
                            Id = t.Id,
                            BidDetailId = t.BidDetailId,
                            TaxId = t.TaxId,
                            TaxName = t.TaxName,
                            TaxCode = t.TaxCode,
                            TaxNatureId = t.TaxNatureId,
                            ChargeTypeId = t.ChargeTypeId,
                            TaxNature = new TaxNatureDataModel
                            {
                                Id = t.TaxNatureId,
                                Name = t.TaxNature.Name,
                                Code = t.TaxNature.Code,
                                IsActive = t.TaxNature.IsActive
                            },
                            ChargeType = new ChargeTypeDataModel
                            {
                                Id = t.ChargeTypeId,
                                Name = t.ChargeType.Name,
                                Code = t.ChargeType.Code,
                                IsActive = t.ChargeType.IsActive
                            },
                            TaxValue = t.TaxValue,
                            TaxAmount = t.TaxAmount
                        }).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (bid == null)
            {
                throw new KeyNotFoundException($"Bid with ID {BidId} not found.");
            }

            return bid;
        }

        public async Task<BidDataModel?> GetVendorsCurrentBidAsync(int vendorId, int auctionId)
        {
            var tenantId = CurrentTenantId;
            var query = _appDbContext.Bids
                .AsNoTracking()
                .Where(x => x.AuctionId == auctionId && x.VendorId == vendorId && x.IsCurrent);

            if (tenantId > 0)
            {
                query = query.Where(x => x.Auction.Organization.TenantId == tenantId || (x.Vendor.TenantId == tenantId && x.VendorId == vendorId));
            }

            var bid = await query
                .Select(x => new BidDataModel
                {
                    Id = x.Id,
                    IsCurrent = x.IsCurrent,
                    MainBidId = x.MainBidId,
                    AuctionId = x.AuctionId,
                    VendorId = x.VendorId,
                    BasicAmount = x.BasicAmount,
                    TaxAmount = x.TaxAmount,
                    DiscountAmount = x.DiscountAmount,
                    NetAmount = x.NetAmount,
                    CreatedAt = x.CreatedAt,
                    BidRevisionNo = x.BidRevisionNo,
                    Vendor = new Core.Models.Tenant.VendorDataModel
                    {
                        Id = x.Vendor.Id,
                        Name = x.Vendor.Name,
                        About = x.Vendor.About
                    },
                    BidDetails = x.BidDetails.Select(bd => new BidDetailDataModel
                    {
                        Id = bd.Id,
                        BidId = bd.BidId,
                        AuctionRequirementId = bd.AuctionRequirementId,
                        Rate = bd.Rate,
                        BaseAmount = bd.BaseAmount,
                        NetAmount = bd.NetAmount,
                        Taxes = bd.Taxes.Select(t => new BidTaxDetailDataModel
                        {
                            Id = t.Id,
                            BidDetailId = t.BidDetailId,
                            TaxId = t.TaxId,
                            TaxName = t.TaxName,
                            TaxCode = t.TaxCode,
                            TaxNatureId = t.TaxNatureId,
                            ChargeTypeId = t.ChargeTypeId,
                            TaxNature = new TaxNatureDataModel
                            {
                                Id = t.TaxNatureId,
                                Name = t.TaxNature.Name,
                                Code = t.TaxNature.Code,
                                IsActive = t.TaxNature.IsActive
                            },
                            ChargeType = new ChargeTypeDataModel
                            {
                                Id = t.ChargeTypeId,
                                Name = t.ChargeType.Name,
                                Code = t.ChargeType.Code,
                                IsActive = t.ChargeType.IsActive
                            },
                            TaxValue = t.TaxValue,
                            TaxAmount = t.TaxAmount
                        }).ToList()
                    }).ToList()
                }).FirstOrDefaultAsync();

            return bid;
        }

        public async Task<BidDataModel?> GetLeadingBidForAuctionAsync(int auctionId, bool isForwardAuction)
        {
            var query = _appDbContext.Bids
                .AsNoTracking()
                .Where(x => x.AuctionId == auctionId && x.IsCurrent);

            query = isForwardAuction
                ? query.OrderByDescending(x => x.NetAmount).ThenBy(x => x.CreatedAt)
                : query.OrderBy(x => x.NetAmount).ThenBy(x => x.CreatedAt);

            return await query
                .Select(x => new BidDataModel
                {
                    Id = x.Id,
                    IsCurrent = x.IsCurrent,
                    MainBidId = x.MainBidId,
                    AuctionId = x.AuctionId,
                    VendorId = x.VendorId,
                    BasicAmount = x.BasicAmount,
                    TaxAmount = x.TaxAmount,
                    DiscountAmount = x.DiscountAmount,
                    NetAmount = x.NetAmount,
                    CreatedAt = x.CreatedAt,
                    BidRevisionNo = x.BidRevisionNo,
                    BidDetails = x.BidDetails.Select(bd => new BidDetailDataModel
                    {
                        Id = bd.Id,
                        BidId = bd.BidId,
                        AuctionRequirementId = bd.AuctionRequirementId,
                        Rate = bd.Rate,
                        BaseAmount = bd.BaseAmount,
                        NetAmount = bd.NetAmount
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }
}

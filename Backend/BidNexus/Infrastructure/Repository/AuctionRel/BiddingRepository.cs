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

            return await MaskForCallerAsync(auctionId, data);
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

            return await MaskForCallerAsync(auctionId, data);
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
        /// <summary>
        /// Vendors must never see competitors' identities or line rates, and must not see
        /// competitors' amounts when the auction hides prices. Organisations see everything.
        /// </summary>
        private async Task<List<BidDataModel>> MaskForCallerAsync(int auctionId, List<BidDataModel> bids)
        {
            var role = _jwtHelper.GetRole();
            if (!string.Equals(role, "Vendor", StringComparison.OrdinalIgnoreCase))
                return bids;

            var callerVendorId = _jwtHelper.GetUserId();
            var pricesHidden = await _appDbContext.Auctions
                .AsNoTracking()
                .Where(a => a.Id == auctionId)
                .Select(a => a.IsBidPriceHidden)
                .FirstOrDefaultAsync();

            var position = 0;
            foreach (var bid in bids)
            {
                position++;
                if (bid.VendorId == callerVendorId)
                    continue;

                bid.VendorId = 0;
                bid.Vendor = new Core.Models.Tenant.VendorDataModel { Id = 0, Name = $"Bidder {position}" };
                bid.BidDetails = [];
                bid.BasicAmount = 0;
                bid.TaxAmount = 0;
                bid.DiscountAmount = 0;
                bid.MainBidId = null;

                if (pricesHidden)
                {
                    bid.NetAmount = 0;
                    bid.AmountHidden = true;
                }
            }

            return bids;
        }

        public async Task<List<BidActivityDataModel>> GetActivityAsync(int auctionId)
        {
            var tenantId = CurrentTenantId;
            var auction = await _appDbContext.Auctions
                .AsNoTracking()
                .Where(a => a.Id == auctionId)
                .Select(a => new
                {
                    a.IsBidPriceHidden,
                    a.OpenToAll,
                    OwnerTenantId = a.Organization.TenantId
                })
                .FirstOrDefaultAsync();

            if (auction == null)
                return [];

            var isVendor = string.Equals(_jwtHelper.GetRole(), "Vendor", StringComparison.OrdinalIgnoreCase);
            var callerVendorId = isVendor ? _jwtHelper.GetUserId() : 0;

            if (!isVendor && auction.OwnerTenantId != tenantId)
                return [];

            if (isVendor && !auction.OpenToAll)
            {
                var qualified = await _appDbContext.VendorIntents
                    .AnyAsync(vi => vi.AuctionId == auctionId && vi.TenantId == tenantId && vi.IsInterested && vi.IsQualified);
                if (!qualified)
                    return [];
            }

            var rows = await _appDbContext.Bids
                .AsNoTracking()
                .Where(b => b.AuctionId == auctionId)
                .OrderBy(b => b.CreatedAt)
                .Select(b => new { b.VendorId, VendorName = b.Vendor.Name, b.NetAmount, b.CreatedAt, b.BidRevisionNo })
                .ToListAsync();

            var aliases = new Dictionary<int, string>();
            return rows.Select(r =>
            {
                var isMine = isVendor && r.VendorId == callerVendorId;
                string bidder;
                if (!isVendor || isMine)
                {
                    bidder = isMine ? "You" : r.VendorName;
                }
                else
                {
                    if (!aliases.TryGetValue(r.VendorId, out var alias))
                    {
                        alias = $"Bidder {aliases.Count + 1}";
                        aliases[r.VendorId] = alias;
                    }
                    bidder = alias;
                }

                return new BidActivityDataModel
                {
                    At = r.CreatedAt,
                    NetAmount = isVendor && !isMine && auction.IsBidPriceHidden ? null : r.NetAmount,
                    Bidder = bidder,
                    IsMine = isMine,
                    BidRevisionNo = r.BidRevisionNo
                };
            }).ToList();
        }
    }
}

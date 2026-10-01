using Core.Abstraction.AuctionRelated;
using Core.Entities.Auction;
using Core.Models.AuctionRelated;
using Core.Models.GlobalData;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.AuctionRel
{
    public class BiddingRepository(AppDbContext appDbContext) : IBidRepositoy
    {
        private readonly AppDbContext _appDbContext = appDbContext;

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
                .Where(a => a.Id == auctionId && a.Organization.TenantId == tenantId)
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
                    .Where(x => taxIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);

            var taxNatures = taxNatureIds.Count == 0
                ? []
                : await _appDbContext.TaxNatures
                    .AsNoTracking()
                    .Where(x => taxNatureIds.Contains(x.Id))
                    .ToListAsync(cancellationToken);

            var chargeTypes = chargeTypeIds.Count == 0
                ? []
                : await _appDbContext.ChargeTypes
                    .AsNoTracking()
                    .Where(x => chargeTypeIds.Contains(x.Id))
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

        public async Task<BidResponseDataModel> ProcessBidAsync(
            Bid bid,
            int tenantId,
            CancellationToken cancellationToken = default)
        {
            await using var transaction =
                await _appDbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var previousBid = await _appDbContext.Bids
                    .Where(b =>
                        b.AuctionId == bid.AuctionId &&
                        b.VendorId == bid.VendorId &&
                        b.IsCurrent &&
                        b.Auction.Organization.TenantId == tenantId)
                    .OrderByDescending(b => b.BidRevisionNo)
                    .FirstOrDefaultAsync(cancellationToken);

                if (previousBid != null)
                {
                    previousBid.IsCurrent = false;
                    bid.MainBidId = previousBid.MainBidId;
                    bid.BidRevisionNo = checked((short)(previousBid.BidRevisionNo + 1));
                }
                else
                {
                    bid.BidRevisionNo = 1;
                    bid.MainBidId = 0;
                }

                await _appDbContext.Bids.AddAsync(bid, cancellationToken);
                await _appDbContext.SaveChangesAsync(cancellationToken);

                if (previousBid == null)
                {
                    bid.InitializeAsMainBid();
                    await _appDbContext.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);

                return new BidResponseDataModel
                {
                    Id = bid.Id,
                    MainBidId = bid.MainBidId,
                    AuctionId = bid.AuctionId,
                    BidRevisionNo = bid.BidRevisionNo
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
            var data = await _appDbContext.Bids
                .AsNoTracking()
                .Where(x => x.AuctionId == auctionId && x.VendorId == vendorId)
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
            var bid = await _appDbContext.Bids
                .AsNoTracking()
                .Where(x => x.Id == BidId)
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
            var bid = await _appDbContext.Bids
                .AsNoTracking()
                .Where(x => x.AuctionId == auctionId && x.VendorId == vendorId && x.IsCurrent)
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
    }
}

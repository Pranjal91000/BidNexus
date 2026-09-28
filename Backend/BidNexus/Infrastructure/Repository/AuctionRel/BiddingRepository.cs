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

        public async Task<BidResponseDataModel> ProcessBidAsync(Bid bid)
        {
            var prevBid = await _appDbContext.Bids
                .Where(b => b.AuctionId == bid.AuctionId && b.VendorId == bid.VendorId && b.IsCurrent)
                .FirstOrDefaultAsync();

            if (prevBid != null)
            {
                prevBid.IsCurrent = false;
                bid.MainBidId = prevBid.MainBidId;
                bid.BidRevisionNo = (short)(prevBid.BidRevisionNo + 1);
            }

            await _appDbContext.Bids.AddAsync(bid);
            var success = await _appDbContext.SaveChangesAsync() > 0;

            if (!success) throw new InvalidOperationException("Failed to process Bid.");

            return new BidResponseDataModel
            {
                Id = bid.Id,
                MainBidId = bid.MainBidId,
                AuctionId = bid.AuctionId,
                BidRevisionNo = bid.BidRevisionNo
            };
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

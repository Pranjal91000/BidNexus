using Core.Entities.Auction;
using Core.Entities.GlobalData;
using Core.Entities.Master;

namespace Core.Models.AuctionRelated;

public sealed class BidProcessingContext
{
    public bool IsForwardAuction { get; init; }
    public bool AuctionExistsForTenant { get; init; }
    public bool OpenToAll { get; init; }
    public VendorIntent? VendorIntent { get; init; }
    public bool VendorExistsForTenant { get; init; }
    public List<AuctionRequirement> Requirements { get; init; } = [];
    public List<TaxMaster> TaxMasters { get; init; } = [];
    public List<TaxNature> TaxNatures { get; init; } = [];
    public List<ChargeType> ChargeTypes { get; init; } = [];
    public decimal? CurrentBestNetAmount { get; init; }
}
using Core.Models.GlobalData;
using Core.Models.Master;
using Core.Models.Tenant;

namespace Core.Models.AuctionRelated
{
    public class BidResponseDataModel
    {
        public long Id { get; set; }
        public long? MainBidId { get; set; }
        public int AuctionId { get; set; }
        public short BidRevisionNo { get; set; }
    }

    public class BidDataModel
    {
        public long Id { get; set; }
        public bool IsCurrent { get; set; }
        public long MainBidId { get; set; }
        public int AuctionId { get; set; }
        public int VendorId { get; set; }
        public decimal BasicAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal NetAmount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public short BidRevisionNo { get; set; }
        public VendorDataModel Vendor { get; set; } = null!;
        public List<BidDetailDataModel> BidDetails { get; set; } = null!;
    }

    public class BidDetailDataModel
    {
        public long Id { get; set; }
        public long BidId { get; set; }
        public int AuctionRequirementId { get; set; }
        public decimal Rate { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal NetAmount { get; set; }
        public List<BidTaxDetailDataModel> Taxes { get; set; } = new List<BidTaxDetailDataModel>();
    }

    public class BidTaxDetailDataModel
    {
        public long Id { get; set; }

        public long BidDetailId { get; set; }

        // Snapshot of the tax master information
        public int? TaxId { get; set; }
        public string TaxName { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;

        public short TaxNatureId { get; set; }
        public short ChargeTypeId { get; set; }

        public TaxMasterDataModel? Tax { get; set; }
        public TaxNatureDataModel TaxNature { get; set; } = null!;
        public ChargeTypeDataModel ChargeType { get; set; } = null!;

        public decimal TaxValue { get; set; }
        public decimal TaxAmount { get; set; }
    }
}

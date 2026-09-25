using Core.Entities.GlobalData;
using Core.Entities.Master;
using Core.Entities.Shared;

namespace Core.Entities.Auction
{
    public class BidTaxDetail
    {
        public long Id { get; set; }

        public long BidDetailId { get; set; }

        // Snapshot of the tax master information
        public int? TaxId { get; set; }
        public string TaxName { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;

        public short TaxNatureId { get; set; }
        public short ChargeTypeId { get; set; }

        public TaxMaster? Tax { get; set; }
        public TaxNature TaxNature { get; set; } = null!;
        public ChargeType ChargeType { get; set; } = null!;

        public decimal TaxValue { get; set; }
        public decimal TaxAmount { get; set; }

        public BidDetail BidDetail { get; set; } = null!;
    }
}

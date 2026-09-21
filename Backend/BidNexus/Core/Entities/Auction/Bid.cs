using Core.Entities.Tenant;

namespace Core.Entities.Auction
{
    public class Bid
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
        public Bid MainBid { get; set; } = null!;
        public Auction Auction { get; set; } = null!;
        public Vendor Vendor { get; set; } = null!;
        public ICollection<BidDetail> BidDetails { get; set; } = null!;
        
    }
}

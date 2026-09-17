using Core.Entities.Shared;
using Core.Entities.User;

namespace Core.Entities.Auction
{
    public class AuctionStatement: BaseEntity
    {
        public int AuctionId { get; set; }
        public long BidId { get; set; }
        public int VendorId { get; set; }
        public short Rank { get; set; }
        public bool IsWinner { get; set; }
        public Auction Auction { get; set; } = null!;
        public Bid Bid { get; set; } = null!;
        public Vendor Vendor { get; set; } = null!;
    }
}

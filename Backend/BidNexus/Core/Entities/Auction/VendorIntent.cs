using Core.Entities.Shared;
using Core.Entities.User;

namespace Core.Entities.Auction
{
    public class VendorIntent: BaseEntity
    {
        public int AuctionId { get; set; }
        public int VendorId { get; set; }
        public bool IsInterested { get; set; }
        public bool IsQualified { get; set; }
        public Auction Auction { get; set; } = null!;
        public Vendor Vendor { get; set; } = null!;
    }
}

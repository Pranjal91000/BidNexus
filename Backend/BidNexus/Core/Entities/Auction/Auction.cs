using Core.Entities.Shared;

namespace Core.Entities.Auction
{
    public class Auction: TransactionBaseEntity
    {
        public bool IsForwardAuction { get; set; }
        public DateTimeOffset AcuctionStartTime { get; set; }
        public DateTimeOffset AuctionEndTime { get; set; }
        public short? DocAttachmentId { get; set; }
        public bool OpenToAll { get; set; }
        public bool IsBidPriceHidden { get; set; }
        public ICollection<VendorIntent> VendorIntent { get; set; } = null!;
    }
}

using Core.Entities.GlobalData;
using Core.Entities.Shared;

namespace Core.Entities.Auction
{
    public class Auction: TransactionBaseEntity
    {
        public bool IsForwardAuction { get; set; }
        public DateTimeOffset AuctionStartTime { get; set; }
        public DateTimeOffset AuctionEndTime { get; set; }
        public Guid? DocAttachmentId { get; set; }
        public bool OpenToAll { get; set; }
        public bool IsBidPriceHidden { get; set; }
        public int OrganizationId {get; set;}
        public Organization Organization {get; set;} = null!;
        public ICollection<VendorIntent> VendorIntent { get; set; } = null!;
        public ICollection<AuctionRequirement> AuctionRequirements { get; set; } = null!;
        public ICollection<Bid> Bids { get; set; } = null!;
        public AuctionStatement AuctionStatement { get; set; } = null!;
        public short StatusId { get; set; }
        public Status Status { get; set; } = null!;
        
    }
}

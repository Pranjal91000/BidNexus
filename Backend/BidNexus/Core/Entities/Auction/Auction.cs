using Core.Entities.GlobalData;
using Core.Entities.Shared;
using Core.Entities.TenantRelated;

namespace Core.Entities.Auction
{
    public class Auction: TransactionBaseEntity
    {
        public string About { get; set; } = string.Empty;
        public string AuctionName { get; set; }
        public bool IsForwardAuction { get; set; }
        public DateTimeOffset? AuctionIntentSubmissionDate { get; set; }
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
        public ICollection<AuctionStatement> AuctionStatements { get; set; } = null!;
        public short StatusId { get; set; }
        public Status Status { get; set; } = null!;

        public Auction(string auctionName,string about,string docNoYearly,DateOnly docDate,bool isForwardAuction,
            DateTimeOffset auctionStartTime,DateTimeOffset auctionEndTime,Guid? docAttachmentId,bool openToAll,
            bool isBidPriceHidden,int organizationId,short statusId)
        {
            AuctionName=auctionName; About=about; DocNoYearly=docNoYearly; DocDate=docDate;
            IsForwardAuction=isForwardAuction; AuctionStartTime=auctionStartTime; AuctionEndTime=auctionEndTime;
            DocAttachmentId=docAttachmentId; OpenToAll=openToAll; IsBidPriceHidden=isBidPriceHidden;
            OrganizationId=organizationId; StatusId=statusId;
        }

        public Auction(int id,string auctionName,string about,string docNoYearly,DateOnly docDate,bool isForwardAuction,
            DateTimeOffset auctionStartTime,DateTimeOffset auctionEndTime,Guid? docAttachmentId,bool openToAll,
            bool isBidPriceHidden,int organizationId,short statusId)
            : this(auctionName,about,docNoYearly,docDate,isForwardAuction,auctionStartTime,auctionEndTime,
                docAttachmentId,openToAll,isBidPriceHidden,organizationId,statusId) => Id=id;

        public Auction() { }

        public void Update(string auctionName,string about,string docNoYearly,DateOnly docDate,bool isForwardAuction,
            DateTimeOffset auctionStartTime,DateTimeOffset auctionEndTime,Guid? docAttachmentId,bool openToAll,
            bool isBidPriceHidden,int organizationId,short statusId)
        {
            AuctionName=auctionName; About=about; DocNoYearly=docNoYearly; IsForwardAuction=isForwardAuction;
            AuctionStartTime=auctionStartTime; AuctionEndTime=auctionEndTime; DocAttachmentId=docAttachmentId;
            OpenToAll=openToAll; IsBidPriceHidden=isBidPriceHidden; OrganizationId=organizationId;
            StatusId=statusId; LastModifiedDateTime=DateTimeOffset.UtcNow;
        }
    }
}
using Core.Entities.Auction;
using Core.Entities.Master;
using Core.Entities.TenantRelated;
using Core.Models.Master;
using Core.Models.Models;

namespace Core.Models.AuctionRelated
{
    public class AuctionDataModel
    {
        public int Id { get; set; }
        public string DocNoYearly { get; set; } = string.Empty;
        public DateOnly DocDate { get; set; }
        public bool IsForwardAuction { get; set; }
        public DateTimeOffset AuctionStartTime { get; set; }
        public DateTimeOffset AuctionEndTime { get; set; }
        public Guid? DocAttachmentId { get; set; }
        public bool OpenToAll { get; set; }
        public bool IsBidPriceHidden { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; } = null!;
        public List<AuctionRequirementDataModel> AuctionRequirements { get; set; } = null!;
        public short StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTimeOffset CreatedDateTime { get; set; }
        public DateTimeOffset LastModifiedDateTime { get; set; }
    }

    public class AuctionLifecycleDataModel
    {
        public int Id { get; set; }
        public short StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public DateTimeOffset AuctionStartTime { get; set; }
        public DateTimeOffset AuctionEndTime { get; set; }
        public bool ShouldStart { get; set; }
        public bool ShouldClose { get; set; }
    }

    public class AuctionResponseModel
    {
        public int Id { get; set; }
        public string DocNoYearly { get; set; } = string.Empty;
        public DateOnly DocDate { get; set; }
        public int OrganizationId { get; set; }
    }

    public class AuctionGetDataModel
    {
        public int Id { get; set; }
        public string DocNoYearly { get; set; } = string.Empty;
        public DateOnly DocDate { get; set; }
        public DateTimeOffset? AuctionIntentSubmissionDate { get; set; }
        public OrganizationOverviewDataModel Organization { get; set; } = null!;
        public List<AuctionRequirementDataModel> AuctionRequirements { get; set; } = null!;
    }

    public class AuctionRequirementDataModel
    {
        public int Id { get; set; }
        public short LineNo { get; set; }
        public int AuctionId { get; set; }
        public int ItemId { get; set; }
        public string? TechnicalSpecification { get; set; }
        public decimal Quantity { get; set; }
        public int UnitId { get; set; }
        public long? DocumentAttachmentId { get; set; }
        public ItemDataModel Item { get; set; } = null!;
        public UnitDataModel Unit { get; set; } = null!;
    }
}

using Core.Entities.Shared;

namespace Core.Entities.Auction
{
    public class AuctionRequirement: BaseEntity
    {
        public int ItemId { get; set; }
        public string? TechnicalSpecification { get; set; }
        public decimal Quantity { get; set; }
        public int UnitId { get; set; }
        public long? DocumentAttachmentId { get; set; }
    }
}

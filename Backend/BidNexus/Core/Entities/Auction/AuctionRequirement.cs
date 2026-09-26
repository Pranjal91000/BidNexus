using Core.Entities.Master;
using Core.Entities.Shared;

namespace Core.Entities.Auction
{
    public class AuctionRequirement : BaseEntity
    {
        public short LineNo { get; set; }
        public int AuctionId { get; set; }
        public int ItemId { get; set; }
        public string? TechnicalSpecification { get; set; }
        public decimal Quantity { get; set; }
        public int UnitId { get; set; }
        public long? DocumentAttachmentId { get; set; }
        public Item Item { get; set; } = null!;
        public Auction Auction { get; set; } = null!;
        public Unit Unit { get; set; } = null!;
    }
}

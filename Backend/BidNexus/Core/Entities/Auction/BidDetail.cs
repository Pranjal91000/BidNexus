using System.Text.Json.Serialization;
using Core.Entities.Shared;
namespace Core.Entities.Auction
{
    public class BidDetail
    {
        public long Id { get; set; }
        public long BidId { get; set; }
        public int AuctionRequirementId { get; set; }
        public decimal Rate { get; set; }
        public decimal BaseAmount {get; set;}
        public decimal NetAmount { get; set; }

        [JsonIgnore]
        public AuctionRequirement AuctionRequirement { get; set; } = null!;
        public ICollection<BidTaxDetail> Taxes { get; set; } = new List<BidTaxDetail>();
    }
}

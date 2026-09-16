using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Auction
{
    public class BidDetail
    {
        public long Id { get; set; }
        public long BidId { get; set; }
        public int AuctionRequirementId { get; set; }
        public decimal Price { get; set; }
        public AuctionRequirement AuctionRequirement { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Auction
{
    public class BidDetail
    {
        public int BidId { get; set; }
        public int AuctionRequirementId { get; set; }
        public decimal Price { get; set; }
    }
}

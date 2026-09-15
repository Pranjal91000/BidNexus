using Core.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Core.Entities.Auction
{
    public class Bid
    {
        public long Id { get; set; }
        public bool IsCurrent { get; set; }
        public long MainBidId { get; set; }
        public int AuctionId { get; set; }
        public int VendorId { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public short BidRevisionNo { get; set; }
        public Bid MainBid { get; set; } = null!;
        public Auction Auction { get; set; } = null!;
        public ICollection<BidDetail> BidDetails { get; set; } = null!;
        
    }
}

namespace Core.Models.AuctionRelated;

public sealed class AuctionStatementDataModel
{
    public int Id { get; set; }
    public int AuctionId { get; set; }
    public long BidId { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal NetAmount { get; set; }
    public short Rank { get; set; }
    public bool IsWinner { get; set; }
}

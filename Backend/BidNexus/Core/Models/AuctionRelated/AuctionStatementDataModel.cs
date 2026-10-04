namespace Core.Models.AuctionRelated;

/// <summary>One ranked row of the final auction statement (organisation view).</summary>
public sealed class AuctionStatementDataModel
{
    public int Id { get; set; }
    public int AuctionId { get; set; }
    public long BidId { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public decimal BasicAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public short Rank { get; set; }
    public bool IsWinner { get; set; }
    public short BidRevisionNo { get; set; }
    public DateTimeOffset SubmittedAt { get; set; }
    public List<StatementLineDataModel> Lines { get; set; } = [];
}

/// <summary>Line-item rate of a bid, flattened for display.</summary>
public sealed class StatementLineDataModel
{
    public int AuctionRequirementId { get; set; }
    public short LineNo { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal NetAmount { get; set; }
}

/// <summary>What a vendor is allowed to see about their own outcome once an auction closes.</summary>
public sealed class VendorAuctionResultDataModel
{
    public int AuctionId { get; set; }
    public bool Participated { get; set; }
    public short? Rank { get; set; }
    public int Bidders { get; set; }
    public bool IsWinner { get; set; }
    public bool PricesHidden { get; set; }
    public decimal? MyNetAmount { get; set; }
    public decimal? MyBasicAmount { get; set; }
    public decimal? MyTaxAmount { get; set; }
    /// <summary>Null when the auction hides prices and the caller did not win.</summary>
    public decimal? WinningAmount { get; set; }
    public short? BidRevisionNo { get; set; }
    public List<StatementLineDataModel> Lines { get; set; } = [];
}

/// <summary>One bid event, used for the price-trend chart. Masked for vendors.</summary>
public sealed class BidActivityDataModel
{
    public DateTimeOffset At { get; set; }
    public decimal? NetAmount { get; set; }
    public string Bidder { get; set; } = string.Empty;
    public bool IsMine { get; set; }
    public short BidRevisionNo { get; set; }
}

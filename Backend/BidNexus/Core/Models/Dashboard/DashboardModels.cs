namespace Core.Models.Dashboard;

public sealed class MonthlyPointDataModel
{
    /// <summary>First day of the month, ISO "yyyy-MM".</summary>
    public string Month { get; set; } = string.Empty;
    public int Auctions { get; set; }
    public int Won { get; set; }
    public decimal Value { get; set; }
}

public sealed class AuctionResultSummaryDataModel
{
    public int AuctionId { get; set; }
    public string AuctionName { get; set; } = string.Empty;
    public string DocNoYearly { get; set; } = string.Empty;
    public DateTimeOffset ClosedAt { get; set; }
    public string? WinnerName { get; set; }
    public decimal? WinningAmount { get; set; }
    public int Bidders { get; set; }
    /// <summary>Opening best bid → winning bid, in percent. Positive means the buyer gained.</summary>
    public decimal? PriceImprovementPercent { get; set; }
}

public sealed class OrganizationDashboardDataModel
{
    public int Live { get; set; }
    public int Upcoming { get; set; }
    public int Drafts { get; set; }
    public int ClosedLast90Days { get; set; }
    public decimal AwardedValueLast90Days { get; set; }
    public decimal? AveragePriceImprovementPercent { get; set; }
    public decimal AverageBiddersPerAuction { get; set; }
    public List<MonthlyPointDataModel> Monthly { get; set; } = [];
    public List<AuctionResultSummaryDataModel> RecentResults { get; set; } = [];
}

public sealed class VendorLivePositionDataModel
{
    public int AuctionId { get; set; }
    public string AuctionName { get; set; } = string.Empty;
    public DateTimeOffset EndsAt { get; set; }
    public int Rank { get; set; }
    public int Bidders { get; set; }
}

public sealed class VendorDashboardDataModel
{
    public int LiveParticipating { get; set; }
    public int Leading { get; set; }
    public int UpcomingOpen { get; set; }
    public int ParticipatedLast12Months { get; set; }
    public int WonLast12Months { get; set; }
    public decimal WonValueLast12Months { get; set; }
    public List<MonthlyPointDataModel> Monthly { get; set; } = [];
    public List<VendorLivePositionDataModel> LivePositions { get; set; } = [];
}

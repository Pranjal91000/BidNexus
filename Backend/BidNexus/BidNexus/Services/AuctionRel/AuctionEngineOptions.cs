namespace API.Services.AuctionRel;

public sealed class AuctionEngineOptions
{
    public const string SectionName = "AuctionEngine";

    public string ScheduledStatusName { get; set; } = "Scheduled";
    public string ActiveStatusName { get; set; } = "Active";
    public string ClosedStatusName { get; set; } = "Closed";
    public int PollingIntervalSeconds { get; set; } = 1;
}

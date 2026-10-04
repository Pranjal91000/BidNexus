namespace Core.Models.Utilities;

public class RatingValueCreateRequest
{
    public short RatingParameterId { get; set; }
    public short RatingScore { get; set; }
}

public class RatingCreateRequest
{
    public int AuctionId { get; set; }
    public short? RatingForId { get; set; }
    public int? AgainstTenant { get; set; }
    public string? Remark { get; set; }
    public List<RatingValueCreateRequest> RatingValues { get; set; } = [];
}

public class RatingValueDataModel
{
    public int Id { get; set; }
    public short RatingParameterId { get; set; }
    public string ParameterName { get; set; } = string.Empty;
    public short RatingScore { get; set; }
}

public class RatingDataModel
{
    public short RatingForId { get; set; }
    public string RatingForName { get; set; } = string.Empty;
    public int AuctionId { get; set; }
    public string AuctionName { get; set; } = string.Empty;
    public int AgainstTenant { get; set; }
    public string AgainstTenantName { get; set; } = string.Empty;
    public int SubmittedByTenant { get; set; }
    public string SubmittedByTenantName { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public List<RatingValueDataModel> RatingValues { get; set; } = [];
}

public class RatingParameterAverageDataModel
{
    public short ParameterId { get; set; }
    public string ParameterName { get; set; } = string.Empty;
    public decimal AverageScore { get; set; }
    public int RatingCount { get; set; }
}

public class RatingSummaryDataModel
{
    public int AuctionId { get; set; }
    public string AuctionName { get; set; } = string.Empty;
    public int SubmittedByTenant { get; set; }
    public string SubmittedByTenantName { get; set; } = string.Empty;
    public short RatingForId { get; set; }
    public string? Remark { get; set; }
    public decimal AverageScore { get; set; }
    public List<RatingValueDataModel> ParameterScores { get; set; } = [];
}

public class TenantReputationDataModel
{
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public int TotalRatingsCount { get; set; }
    public decimal OverallAverageScore { get; set; }
    public List<RatingParameterAverageDataModel> ParameterAverages { get; set; } = [];
    public List<RatingSummaryDataModel> RecentRatings { get; set; } = [];
}

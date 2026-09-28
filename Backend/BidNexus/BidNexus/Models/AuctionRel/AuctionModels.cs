using API.ModelValidators.AuctionRel;
using FluentValidation.Results;

namespace API.Models.AuctionRel;

public class AuctionRequirementSaveRequest
{
    public short LineNo { get; set; }
    public int ItemId { get; set; }
    public string? TechnicalSpecification { get; set; }
    public decimal Quantity { get; set; }
    public int UnitId { get; set; }
    public long? DocumentAttachmentId { get; set; }
}

public class AuctionCreateRequest
{
    public string AuctionName { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;
    public string DocNoYearly { get; set; } = string.Empty;
    public DateOnly DocDate { get; set; }
    public bool IsForwardAuction { get; set; }
    public DateTimeOffset AuctionStartTime { get; set; }
    public DateTimeOffset AuctionEndTime { get; set; }
    public Guid? DocAttachmentId { get; set; }
    public bool OpenToAll { get; set; }
    public bool IsBidPriceHidden { get; set; }
    public int OrganizationId { get; set; }
    public short StatusId { get; set; }
    public List<AuctionRequirementSaveRequest> AuctionRequirements { get; set; } = [];

    public ValidationResult Validate() => new AuctionCreateRequestValidator().Validate(this);
}

public class AuctionUpdateRequest
{
    public string AuctionName { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;
    public string DocNoYearly { get; set; } = string.Empty;
    public DateOnly DocDate { get; set; }
    public bool IsForwardAuction { get; set; }
    public DateTimeOffset AuctionStartTime { get; set; }
    public DateTimeOffset AuctionEndTime { get; set; }
    public Guid? DocAttachmentId { get; set; }
    public bool OpenToAll { get; set; }
    public bool IsBidPriceHidden { get; set; }
    public int OrganizationId { get; set; }
    public short StatusId { get; set; }
    public List<AuctionRequirementSaveRequest> AuctionRequirements { get; set; } = [];

    public ValidationResult Validate() => new AuctionUpdateRequestValidator().Validate(this);
}

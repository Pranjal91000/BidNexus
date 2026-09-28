using API.ModelValidators.AuctionRel;
using FluentValidation.Results;

namespace API.Models.AuctionRel;

public class BidTaxDetailSaveRequest
{
    public int? TaxId { get; set; }
    public string TaxName { get; set; } = string.Empty;
    public string TaxCode { get; set; } = string.Empty;
    public short TaxNatureId { get; set; }
    public short ChargeTypeId { get; set; }
    public decimal TaxValue { get; set; }
    public decimal TaxAmount { get; set; }
}

public class BidDetailSaveRequest
{
    public int AuctionRequirementId { get; set; }
    public decimal Rate { get; set; }
    public decimal BaseAmount { get; set; }
    public decimal NetAmount { get; set; }
    public List<BidTaxDetailSaveRequest> Taxes { get; set; } = [];
}

public class BidCreateRequest
{
    public int AuctionId { get; set; }
    public int VendorId { get; set; }
    public decimal BasicAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public long? MainBidId { get; set; }
    public short BidRevisionNo { get; set; }
    public List<BidDetailSaveRequest> BidDetails { get; set; } = [];

    public ValidationResult Validate() => new BidCreateRequestValidator().Validate(this);
}

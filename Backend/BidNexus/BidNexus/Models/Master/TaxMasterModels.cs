using API.ModelValidators.Master;
using FluentValidation.Results;

namespace API.Models.Master;

public class TaxMasterCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public short TaxNatureId { get; set; }
    public short ChargeTypeId { get; set; }
    public decimal TaxValue { get; set; }
    public short StatusId { get; set; }
    public string? StatusRemarks { get; set; }

    public ValidationResult Validate() => new TaxMasterCreateRequestValidator().Validate(this);
}

public class TaxMasterUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public short TaxNatureId { get; set; }
    public short ChargeTypeId { get; set; }
    public decimal TaxValue { get; set; }
    public short StatusId { get; set; }
    public string? StatusRemarks { get; set; }

    public ValidationResult Validate() => new TaxMasterUpdateRequestValidator().Validate(this);
}

public class TaxMasterResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public short TaxNatureId { get; set; }
    public short ChargeTypeId { get; set; }
    public decimal TaxValue { get; set; }
    public short StatusId { get; set; }
    public string? StatusRemarks { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset LastModifiedDateTime { get; set; }
}

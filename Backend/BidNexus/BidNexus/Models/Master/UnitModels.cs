using API.ModelValidators.Master;
using FluentValidation.Results;

namespace API.Models.Master;

public class UnitCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public short StatusId { get; set; }
    public string? StatusRemarks { get; set; }

    public ValidationResult Validate() => new UnitCreateRequestValidator().Validate(this);
}

public class UnitUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public short StatusId { get; set; }
    public string? StatusRemarks { get; set; }

    public ValidationResult Validate() => new UnitUpdateRequestValidator().Validate(this);
}

public class UnitResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public short StatusId { get; set; }
    public string? StatusRemarks { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset LastModifiedDateTime { get; set; }
}

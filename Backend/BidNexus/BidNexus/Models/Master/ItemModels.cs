using API.ModelValidators.Master;
using FluentValidation.Results;

namespace API.Models.Master;

public class ItemCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public short CategoryId { get; set; }
    public string? ItemDescription { get; set; }
    public int? DocAttachmentId { get; set; }
    public short StatusId { get; set; }
    public string? StatusRemarks { get; set; }
    public List<int> UnitIds { get; set; } = [];

    public ValidationResult Validate() => new ItemCreateRequestValidator().Validate(this);
}

public class ItemUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public short CategoryId { get; set; }
    public string? ItemDescription { get; set; }
    public int? DocAttachmentId { get; set; }
    public short StatusId { get; set; }
    public string? StatusRemarks { get; set; }
    public List<int> UnitIds { get; set; } = [];

    public ValidationResult Validate() => new ItemUpdateRequestValidator().Validate(this);
}

public class ItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public short CategoryId { get; set; }
    public string? ItemDescription { get; set; }
    public int? DocAttachmentId { get; set; }
    public string? DocAttachmentUrl => DocAttachmentId.HasValue ? $"/api/attachments/{DocAttachmentId.Value}" : null;
    public short StatusId { get; set; }
    public string? StatusRemarks { get; set; }
    public List<int> UnitIds { get; set; } = [];
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset LastModifiedDateTime { get; set; }
}

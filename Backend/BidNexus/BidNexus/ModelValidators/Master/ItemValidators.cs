using API.Models.Master;
using FluentValidation;

namespace API.ModelValidators.Master;

public class ItemCreateRequestValidator : AbstractValidator<ItemCreateRequest>
{
    public ItemCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(20).WithMessage("Code cannot exceed 20 characters.");

        RuleFor(x => x.CategoryId)
            .GreaterThan((short)0).WithMessage("A valid CategoryId is required.");

        RuleFor(x => x.StatusId)
            .GreaterThan((short)0).WithMessage("A valid StatusId is required.");

        RuleFor(x => x.UnitIds)
            .Must(unitIds => unitIds == null || unitIds.Count == unitIds.Distinct().Count())
            .WithMessage("Duplicate Unit IDs are not allowed in the request.");
    }
}

public class ItemUpdateRequestValidator : AbstractValidator<ItemUpdateRequest>
{
    public ItemUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(20).WithMessage("Code cannot exceed 20 characters.");

        RuleFor(x => x.CategoryId)
            .GreaterThan((short)0).WithMessage("A valid CategoryId is required.");

        RuleFor(x => x.StatusId)
            .GreaterThan((short)0).WithMessage("A valid StatusId is required.");

        RuleFor(x => x.UnitIds)
            .Must(unitIds => unitIds == null || unitIds.Count == unitIds.Distinct().Count())
            .WithMessage("Duplicate Unit IDs are not allowed in the request.");
    }
}

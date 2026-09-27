using API.Models.Master;
using FluentValidation;

namespace API.ModelValidators.Master;

public class UnitCreateRequestValidator : AbstractValidator<UnitCreateRequest>
{
    public UnitCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(20).WithMessage("Code cannot exceed 20 characters.");

        RuleFor(x => x.StatusId)
            .GreaterThan((short)0).WithMessage("A valid StatusId is required.");
    }
}

public class UnitUpdateRequestValidator : AbstractValidator<UnitUpdateRequest>
{
    public UnitUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(20).WithMessage("Code cannot exceed 20 characters.");

        RuleFor(x => x.StatusId)
            .GreaterThan((short)0).WithMessage("A valid StatusId is required.");
    }
}

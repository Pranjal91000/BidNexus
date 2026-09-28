using API.Models.Master;
using FluentValidation;

namespace API.ModelValidators.Master;

public class TaxMasterCreateRequestValidator : AbstractValidator<TaxMasterCreateRequest>
{
    public TaxMasterCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(20).WithMessage("Code cannot exceed 20 characters.");

        RuleFor(x => x.TaxNatureId)
            .GreaterThan((short)0).WithMessage("A valid TaxNatureId is required.");

        RuleFor(x => x.ChargeTypeId)
            .GreaterThan((short)0).WithMessage("A valid ChargeTypeId is required.");

        RuleFor(x => x.StatusId)
            .GreaterThan((short)0).WithMessage("A valid StatusId is required.");
    }
}

public class TaxMasterUpdateRequestValidator : AbstractValidator<TaxMasterUpdateRequest>
{
    public TaxMasterUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MaximumLength(20).WithMessage("Code cannot exceed 20 characters.");

        RuleFor(x => x.TaxNatureId)
            .GreaterThan((short)0).WithMessage("A valid TaxNatureId is required.");

        RuleFor(x => x.ChargeTypeId)
            .GreaterThan((short)0).WithMessage("A valid ChargeTypeId is required.");

        RuleFor(x => x.StatusId)
            .GreaterThan((short)0).WithMessage("A valid StatusId is required.");

    }
}

using API.Models.AuctionRel;
using FluentValidation;

namespace API.ModelValidators.AuctionRel;

public class AuctionRequirementSaveRequestValidator : AbstractValidator<AuctionRequirementSaveRequest>
{
    public AuctionRequirementSaveRequestValidator()
    {
        RuleFor(x => x.LineNo)
            .GreaterThan((short)0).WithMessage("LineNo must be greater than 0.");

        RuleFor(x => x.ItemId)
            .GreaterThan(0).WithMessage("A valid ItemId is required.");

        RuleFor(x => x.UnitId)
            .GreaterThan(0).WithMessage("A valid UnitId is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.TechnicalSpecification)
            .MaximumLength(1000).WithMessage("Technical specification cannot exceed 1000 characters.");
    }
}

public class AuctionCreateRequestValidator : AbstractValidator<AuctionCreateRequest>
{
    public AuctionCreateRequestValidator()
    {
        RuleFor(x => x.AuctionName)
            .NotEmpty().WithMessage("AuctionName is required.")
            .MaximumLength(150).WithMessage("AuctionName cannot exceed 150 characters.");

        RuleFor(x => x.About)
            .MaximumLength(1000).WithMessage("About cannot exceed 1000 characters.");

        RuleFor(x => x.DocNoYearly)
            .NotEmpty().WithMessage("DocNoYearly is required.")
            .MaximumLength(50).WithMessage("DocNoYearly cannot exceed 50 characters.");

        RuleFor(x => x.DocDate)
            .NotEmpty().WithMessage("DocDate is required.");

        RuleFor(x => x.AuctionStartTime)
            .NotEmpty().WithMessage("AuctionStartTime is required.");

        RuleFor(x => x.AuctionEndTime)
            .NotEmpty().WithMessage("AuctionEndTime is required.")
            .GreaterThan(x => x.AuctionStartTime).WithMessage("AuctionEndTime must be greater than AuctionStartTime.");

        RuleFor(x => x.OrganizationId)
            .GreaterThan(0).WithMessage("A valid OrganizationId is required.");

        RuleFor(x => x.StatusId)
            .GreaterThan((short)0).WithMessage("A valid StatusId is required.");

        RuleFor(x => x.AuctionRequirements)
            .Must(reqs => reqs == null || reqs.Select(r => r.LineNo).Distinct().Count() == reqs.Count)
            .WithMessage("Duplicate LineNo in auction requirements is not allowed.");

        RuleForEach(x => x.AuctionRequirements)
            .SetValidator(new AuctionRequirementSaveRequestValidator());
    }
}

public class AuctionUpdateRequestValidator : AbstractValidator<AuctionUpdateRequest>
{
    public AuctionUpdateRequestValidator()
    {
        RuleFor(x => x.AuctionName)
            .NotEmpty().WithMessage("AuctionName is required.")
            .MaximumLength(150).WithMessage("AuctionName cannot exceed 150 characters.");

        RuleFor(x => x.About)
            .MaximumLength(1000).WithMessage("About cannot exceed 1000 characters.");

        RuleFor(x => x.DocNoYearly)
            .NotEmpty().WithMessage("DocNoYearly is required.")
            .MaximumLength(50).WithMessage("DocNoYearly cannot exceed 50 characters.");

        RuleFor(x => x.DocDate)
            .NotEmpty().WithMessage("DocDate is required.");

        RuleFor(x => x.AuctionStartTime)
            .NotEmpty().WithMessage("AuctionStartTime is required.");

        RuleFor(x => x.AuctionEndTime)
            .NotEmpty().WithMessage("AuctionEndTime is required.")
            .GreaterThan(x => x.AuctionStartTime).WithMessage("AuctionEndTime must be greater than AuctionStartTime.");

        RuleFor(x => x.OrganizationId)
            .GreaterThan(0).WithMessage("A valid OrganizationId is required.");

        RuleFor(x => x.StatusId)
            .GreaterThan((short)0).WithMessage("A valid StatusId is required.");

        RuleFor(x => x.AuctionRequirements)
            .Must(reqs => reqs == null || reqs.Select(r => r.LineNo).Distinct().Count() == reqs.Count)
            .WithMessage("Duplicate LineNo in auction requirements is not allowed.");

        RuleForEach(x => x.AuctionRequirements)
            .SetValidator(new AuctionRequirementSaveRequestValidator());
    }
}

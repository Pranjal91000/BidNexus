using API.Models.AuctionRel;
using FluentValidation;

namespace API.ModelValidators.AuctionRel;

public class BidTaxDetailSaveRequestValidator : AbstractValidator<BidTaxDetailSaveRequest>
{
    public BidTaxDetailSaveRequestValidator()
    {
        RuleFor(x => x.TaxNatureId)
            .GreaterThan((short)0)
            .When(x => !x.TaxId.HasValue)
            .WithMessage("TaxNatureId must be greater than 0.");

        RuleFor(x => x.ChargeTypeId)
            .GreaterThan((short)0)
            .When(x => !x.TaxId.HasValue)
            .WithMessage("ChargeTypeId must be greater than 0.");

        RuleFor(x => x.TaxValue)
            .GreaterThanOrEqualTo(0)
            .WithMessage("TaxValue cannot be negative.");
    }
}

public class BidDetailSaveRequestValidator : AbstractValidator<BidDetailSaveRequest>
{
    public BidDetailSaveRequestValidator()
    {
        RuleFor(x => x.AuctionRequirementId)
            .GreaterThan(0)
            .WithMessage("A valid AuctionRequirementId is required.");

        RuleFor(x => x.Rate)
            .GreaterThan(0)
            .WithMessage("Rate must be greater than 0.");

        RuleForEach(x => x.Taxes)
            .SetValidator(new BidTaxDetailSaveRequestValidator());
    }
}

public class BidCreateRequestValidator : AbstractValidator<BidCreateRequest>
{
    public BidCreateRequestValidator()
    {
        RuleFor(x => x.AuctionId)
            .GreaterThan(0)
            .WithMessage("A valid AuctionId is required.");

        RuleFor(x => x.BidDetails)
            .NotEmpty()
            .WithMessage("At least one bid detail is required.");

        RuleForEach(x => x.BidDetails)
            .SetValidator(new BidDetailSaveRequestValidator());

        RuleFor(x => x.DiscountAmount)
            .Equal(0)
            .WithMessage("Discounts are not supported by the auction bidding engine.");
    }
}

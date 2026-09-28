using API.Models.AuctionRel;
using Core.Abstraction.AuctionRelated;
using Core.Models.AuctionRelated;
using FluentValidation;

namespace API.ModelValidators.AuctionRel;

public class BidTaxDetailSaveRequestValidator : AbstractValidator<BidTaxDetailSaveRequest>
{
    public BidTaxDetailSaveRequestValidator()
    {
        RuleFor(x => x.TaxNatureId)
            .GreaterThan((short)0).WithMessage("TaxNatureId must be greater than 0.");

        RuleFor(x => x.ChargeTypeId)
            .GreaterThan((short)0).WithMessage("ChargeTypeId must be greater than 0.");

        RuleFor(x => x.TaxValue)
            .GreaterThanOrEqualTo(0).WithMessage("TaxValue cannot be negative.");

        RuleFor(x => x.TaxAmount)
            .GreaterThanOrEqualTo(0).WithMessage("TaxAmount cannot be negative.");
    }
}

public class BidDetailSaveRequestValidator : AbstractValidator<BidDetailSaveRequest>
{
    public BidDetailSaveRequestValidator()
    {
        RuleFor(x => x.AuctionRequirementId)
            .GreaterThan(0).WithMessage("A valid AuctionRequirementId is required.");

        RuleFor(x => x.Rate)
            .GreaterThan(0).WithMessage("Rate must be greater than 0.");

        RuleFor(x => x.BaseAmount)
            .GreaterThan(0).WithMessage("BaseAmount must be greater than 0.");

        RuleFor(x => x.NetAmount)
            .GreaterThan(0).WithMessage("NetAmount must be greater than 0.");

        RuleForEach(x => x.Taxes)
            .SetValidator(new BidTaxDetailSaveRequestValidator());
    }
}

public class BidCreateRequestValidator : AbstractValidator<BidCreateRequest>
{
    public BidCreateRequestValidator()
    {
        RuleFor(x => x.AuctionId)
            .GreaterThan(0).WithMessage("A valid AuctionId is required.");

        RuleFor(x => x.VendorId)
            .GreaterThan(0).WithMessage("A valid VendorId is required.");

        RuleFor(x => x.BasicAmount)
            .GreaterThan(0).WithMessage("BasicAmount must be greater than 0.");

        RuleFor(x => x.NetAmount)
            .GreaterThan(0).WithMessage("NetAmount must be greater than 0.");

        RuleFor(x => x.BidDetails)
            .NotEmpty().WithMessage("At least one bid detail is required.");

        RuleForEach(x => x.BidDetails)
            .SetValidator(new BidDetailSaveRequestValidator());
    }

    public BidCreateRequestValidator(IAuctionRepository auctionRepository, IBidRepositoy bidRepository) : this()
    {
        RuleFor(x => x)
            .CustomAsync(async (request, context, cancellationToken) =>
            {
                AuctionDataModel? auction = null;
                try
                {
                    auction = await auctionRepository.GetById(request.AuctionId);
                }
                catch (KeyNotFoundException)
                {
                    context.AddFailure(nameof(request.AuctionId), "Auction not found.");
                    return;
                }

                if (auction == null)
                {
                    context.AddFailure(nameof(request.AuctionId), "Auction not found.");
                    return;
                }

                var prevBid = await bidRepository.GetVendorsCurrentBidAsync(request.VendorId, request.AuctionId);
                if (prevBid != null)
                {
                    if (auction.IsForwardAuction && request.NetAmount <= prevBid.NetAmount)
                    {
                        context.AddFailure(nameof(request.NetAmount), "NetAmount must be greater than previous bid amount.");
                    }
                    else if (!auction.IsForwardAuction && request.NetAmount >= prevBid.NetAmount)
                    {
                        context.AddFailure(nameof(request.NetAmount), "NetAmount must be less than previous bid amount.");
                    }
                }
            });
    }
}

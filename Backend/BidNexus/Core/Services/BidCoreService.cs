using System;
using System.Collections.Generic;
using System.Linq;
using Core.Abstraction.Services;
using Core.Entities.Auction;
using Core.Exceptions;
using Core.Models.AuctionRelated;

namespace Core.Services
{
    public class BidCoreService : IBidCoreService
    {
        public List<ValidationException> CustomValidation(Bid bid, BidProcessingContext context, AuctionDataModel? auction = null)
        {
            var validationErrors = new List<ValidationException>();

            if (!context.AuctionExistsForTenant)
            {
                validationErrors.Add(new ValidationException("Auction was not found for the authenticated tenant."));
                return validationErrors;
            }

            if (!context.VendorExistsForTenant)
            {
                validationErrors.Add(new ValidationException("The authenticated user is not a valid vendor for this tenant."));
                return validationErrors;
            }

            if (!context.OpenToAll)
            {
                if (context.VendorIntent == null ||
                    !context.VendorIntent.IsInterested ||
                    !context.VendorIntent.IsQualified)
                {
                    validationErrors.Add(new ValidationException("Vendor is not qualified for this auction."));
                    return validationErrors;
                }
            }
            else if (context.VendorIntent != null &&
                     (!context.VendorIntent.IsInterested || !context.VendorIntent.IsQualified))
            {
                validationErrors.Add(new ValidationException("Vendor is not qualified to bid in this auction."));
                return validationErrors;
            }

            if (auction != null)
            {
                var now = DateTimeOffset.UtcNow;
                if (now < auction.AuctionStartTime)
                {
                    validationErrors.Add(new ValidationException("Auction has not started yet."));
                }

                if (now >= auction.AuctionEndTime)
                {
                    validationErrors.Add(new ValidationException("Auction has already ended."));
                }
            }

            var requirementMap = context.Requirements.ToDictionary(x => x.Id);
            var bidRequirementIds = (bid.BidDetails ?? []).Select(x => x.AuctionRequirementId).ToList();

            if (bidRequirementIds.Count != bidRequirementIds.Distinct().Count())
            {
                validationErrors.Add(new ValidationException("Each auction requirement may appear only once in a bid."));
            }

            if (bidRequirementIds.Any(id => !requirementMap.ContainsKey(id)))
            {
                validationErrors.Add(new ValidationException("One or more auction requirements do not belong to this auction."));
            }

            foreach (var detail in bid.BidDetails ?? [])
            {
                if (detail.NetAmount <= 0)
                {
                    validationErrors.Add(new ValidationException($"Bid detail for requirement {detail.AuctionRequirementId} results in a non-positive net amount."));
                }
            }

            if (bid.NetAmount <= 0)
            {
                validationErrors.Add(new ValidationException("Calculated bid NetAmount must be greater than zero."));
            }

            if (context.CurrentBestNetAmount.HasValue)
            {
                if (context.IsForwardAuction &&
                    bid.NetAmount <= context.CurrentBestNetAmount.Value)
                {
                    validationErrors.Add(new ValidationException(
                        $"Bid must be greater than the current leading amount of {context.CurrentBestNetAmount.Value:0.00}."));
                }

                if (!context.IsForwardAuction &&
                    bid.NetAmount >= context.CurrentBestNetAmount.Value)
                {
                    validationErrors.Add(new ValidationException(
                        $"Bid must be less than the current leading amount of {context.CurrentBestNetAmount.Value:0.00}."));
                }
            }

            return validationErrors;
        }
    }
}

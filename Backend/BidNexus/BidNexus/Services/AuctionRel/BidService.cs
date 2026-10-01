using API.Abstraction.AuctionRel;
using API.Models.AuctionRel;
using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Services;
using Core.Entities.Auction;
using Core.Entities.Master;
using Core.Models.AuctionRelated;

namespace API.Services.AuctionRel;

public class BidService(
    IBidRepositoy bidRepository,
    IAuctionRepository auctionRepository,
    IJwtHelperService jwtHelperService) : IBidService
{
    private readonly IBidRepositoy _bidRepository = bidRepository;
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IJwtHelperService _jwtHelper = jwtHelperService;

    public async Task<BidResponseDataModel> ProcessBidAsync(
        BidCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var vendorId = _jwtHelper.GetUserId();
        var tenantId = _jwtHelper.GetTenantId();

        if (vendorId <= 0 || tenantId <= 0)
            throw new UnauthorizedAccessException("The authenticated vendor context is missing.");

        if (request.DiscountAmount != 0)
            throw new InvalidOperationException(
                "Discounts are not supported by the auction bidding engine.");

        var requirementIds = (request.BidDetails ?? [])
            .Select(x => x.AuctionRequirementId)
            .ToArray();

        if (requirementIds.Length != requirementIds.Distinct().Count())
            throw new InvalidOperationException(
                "Each auction requirement may appear only once in a bid.");

        var taxIds = (request.BidDetails ?? [])
            .SelectMany(x => x.Taxes ?? [])
            .Where(x => x.TaxId.HasValue)
            .Select(x => x.TaxId!.Value)
            .Distinct()
            .ToArray();

        var taxNatureIds = (request.BidDetails ?? [])
            .SelectMany(x => x.Taxes ?? [])
            .Select(x => x.TaxNatureId)
            .Where(x => x > 0)
            .Distinct()
            .ToArray();

        var chargeTypeIds = (request.BidDetails ?? [])
            .SelectMany(x => x.Taxes ?? [])
            .Select(x => x.ChargeTypeId)
            .Where(x => x > 0)
            .Distinct()
            .ToArray();

        var auction = await GetAuctionAsync(request.AuctionId, cancellationToken);

        var context = await _bidRepository.GetBidProcessingContextAsync(
            request.AuctionId,
            vendorId,
            requirementIds,
            taxIds,
            taxNatureIds,
            chargeTypeIds,
            tenantId,
            auction.IsForwardAuction,
            cancellationToken);

        if (!context.AuctionExistsForTenant)
            throw new InvalidOperationException("Auction was not found for the authenticated tenant.");

        if (!context.VendorExistsForTenant)
            throw new UnauthorizedAccessException("The authenticated user is not a valid vendor for this tenant.");

        if (!context.OpenToAll)
        {
            if (context.VendorIntent == null ||
                !context.VendorIntent.IsInterested ||
                !context.VendorIntent.IsQualified)
            {
                throw new UnauthorizedAccessException(
                    "Vendor is not interested and qualified for this auction.");
            }
        }
        else if (context.VendorIntent != null &&
                 (!context.VendorIntent.IsInterested || !context.VendorIntent.IsQualified))
        {
            throw new UnauthorizedAccessException(
                "Vendor is not qualified to bid in this auction.");
        }

        var requirementMap = context.Requirements.ToDictionary(x => x.Id);
        if (requirementMap.Count != requirementIds.Length)
            throw new InvalidOperationException(
                "One or more auction requirements do not belong to this auction.");

        var taxMasterMap = context.TaxMasters.ToDictionary(x => x.Id);
        var taxNatureMap = context.TaxNatures.ToDictionary(x => x.Id);
        var chargeTypeMap = context.ChargeTypes.ToDictionary(x => x.Id);

        var bidDetails = new List<BidDetail>();
        decimal basicAmount = 0m;
        decimal signedTaxAmount = 0m;

        foreach (var detailRequest in request.BidDetails ?? [])
        {
            var requirement = requirementMap[detailRequest.AuctionRequirementId];

            var baseAmount = RoundMoney(
                detailRequest.Rate * requirement.Quantity);

            var bidDetail = new BidDetail
            {
                AuctionRequirementId = requirement.Id,
                Rate = detailRequest.Rate,
                BaseAmount = baseAmount
            };

            decimal additiveTaxes = 0m;
            decimal deductiveTaxes = 0m;

            foreach (var taxRequest in detailRequest.Taxes ?? [])
            {
                var tax = ResolveTax(
                    taxRequest,
                    taxMasterMap,
                    taxNatureMap,
                    chargeTypeMap);

                var taxAmount = CalculateTaxAmount(
                    tax.ChargeType.Code,
                    tax.ChargeType.Name,
                    tax.Value,
                    baseAmount,
                    requirement.Quantity);

                var isDeductive = IsDeductiveTax(
                    tax.TaxNature.Code,
                    tax.TaxNature.Name);

                if (isDeductive)
                    deductiveTaxes += taxAmount;
                else
                    additiveTaxes += taxAmount;

                bidDetail.Taxes.Add(new BidTaxDetail
                {
                    TaxId = tax.TaxId,
                    TaxName = tax.Name,
                    TaxCode = tax.Code,
                    TaxNatureId = tax.TaxNature.Id,
                    ChargeTypeId = tax.ChargeType.Id,
                    TaxValue = tax.Value,
                    TaxAmount = taxAmount
                });
            }

            var detailTaxAmount = RoundMoney(additiveTaxes - deductiveTaxes);
            var detailNetAmount = RoundMoney(baseAmount + detailTaxAmount);

            if (detailNetAmount <= 0)
                throw new InvalidOperationException(
                    $"Bid detail for requirement {requirement.Id} results in a non-positive net amount.");

            bidDetail.NetAmount = detailNetAmount;
            basicAmount += baseAmount;
            signedTaxAmount += detailTaxAmount;
            bidDetails.Add(bidDetail);
        }

        basicAmount = RoundMoney(basicAmount);
        signedTaxAmount = RoundMoney(signedTaxAmount);
        var netAmount = RoundMoney(basicAmount + signedTaxAmount);

        if (netAmount <= 0)
            throw new InvalidOperationException("Calculated bid NetAmount must be greater than zero.");

        if (context.CurrentBestNetAmount.HasValue)
        {
            if (context.IsForwardAuction &&
                netAmount <= context.CurrentBestNetAmount.Value)
            {
                throw new InvalidOperationException(
                    $"Bid must be greater than the current leading amount of {context.CurrentBestNetAmount.Value:0.00}.");
            }

            if (!context.IsForwardAuction &&
                netAmount >= context.CurrentBestNetAmount.Value)
            {
                throw new InvalidOperationException(
                    $"Bid must be less than the current leading amount of {context.CurrentBestNetAmount.Value:0.00}.");
            }
        }

        var bid = new Bid
        {
            AuctionId = request.AuctionId,
            VendorId = vendorId,
            BasicAmount = basicAmount,
            TaxAmount = signedTaxAmount,
            DiscountAmount = 0m,
            NetAmount = netAmount,
            CreatedAt = DateTimeOffset.UtcNow,
            IsCurrent = true,
            BidDetails = bidDetails
        };

        return await _bidRepository.ProcessBidAsync(
            bid,
            tenantId,
            cancellationToken);
    }

    private async Task<Core.Models.AuctionRelated.AuctionDataModel> GetAuctionAsync(
        int auctionId,
        CancellationToken cancellationToken)
    {
        return await _auctionRepository.GetById(auctionId);
    }

    private static ResolvedTax ResolveTax(
        BidTaxDetailSaveRequest request,
        IReadOnlyDictionary<int, TaxMaster> taxMasters,
        IReadOnlyDictionary<short, Core.Entities.GlobalData.TaxNature> taxNatures,
        IReadOnlyDictionary<short, Core.Entities.GlobalData.ChargeType> chargeTypes)
    {
        if (request.TaxId.HasValue)
        {
            if (!taxMasters.TryGetValue(request.TaxId.Value, out var master))
                throw new InvalidOperationException(
                    $"Tax master {request.TaxId.Value} was not found for the current tenant.");

            if (!taxNatures.TryGetValue(master.TaxNatureId, out var masterNature) ||
                !masterNature.IsActive)
                throw new InvalidOperationException(
                    $"Tax nature {master.TaxNatureId} is invalid or inactive.");

            if (!chargeTypes.TryGetValue(master.ChargeTypeId, out var masterChargeType) ||
                !masterChargeType.IsActive)
                throw new InvalidOperationException(
                    $"Charge type {master.ChargeTypeId} is invalid or inactive.");

            return new ResolvedTax(
                master.Id,
                master.Name.Trim(),
                master.Code.Trim(),
                masterNature,
                masterChargeType,
                master.TaxValue);
        }

        if (!taxNatures.TryGetValue(request.TaxNatureId, out var nature) ||
            !nature.IsActive)
            throw new InvalidOperationException(
                $"Tax nature {request.TaxNatureId} is invalid or inactive.");

        if (!chargeTypes.TryGetValue(request.ChargeTypeId, out var chargeType) ||
            !chargeType.IsActive)
            throw new InvalidOperationException(
                $"Charge type {request.ChargeTypeId} is invalid or inactive.");

        if (string.IsNullOrWhiteSpace(request.TaxName) ||
            string.IsNullOrWhiteSpace(request.TaxCode))
            throw new InvalidOperationException(
                "TaxName and TaxCode are required when a tax master is not selected.");

        return new ResolvedTax(
            null,
            request.TaxName.Trim(),
            request.TaxCode.Trim(),
            nature,
            chargeType,
            request.TaxValue);
    }

    private static decimal CalculateTaxAmount(
        string chargeTypeCode,
        string chargeTypeName,
        decimal taxValue,
        decimal baseAmount,
        decimal quantity)
    {
        if (taxValue < 0)
            throw new InvalidOperationException("TaxValue cannot be negative.");

        var code = NormalizeCode(chargeTypeCode, chargeTypeName);

        var amount = code switch
        {
            "PERCENTAGE" or "PERCENT" or "PCT" => baseAmount * taxValue / 100m,
            "PER_UNIT" or "PERUNIT" or "UNIT" => quantity * taxValue,
            "WHOLE" or "FIXED" or "FLAT" => taxValue,
            _ => throw new InvalidOperationException(
                $"Unsupported charge type '{chargeTypeName}'.")
        };

        return RoundMoney(amount);
    }

    private static bool IsDeductiveTax(string code, string name)
        => NormalizeCode(code, name) switch
        {
            "DEDUCTIVE" or "DEDUCT" => true,
            "ADDITIVE" or "ADD" => false,
            _ => throw new InvalidOperationException($"Unsupported tax nature '{name}'.")
        };

    private static string NormalizeCode(string code, string name)
        => (string.IsNullOrWhiteSpace(code) ? name : code)
            .Trim()
            .ToUpperInvariant()
            .Replace("-", "_")
            .Replace(" ", "_");

    private static decimal RoundMoney(decimal value)
        => Math.Round(value, 2, MidpointRounding.AwayFromZero);

    private sealed record ResolvedTax(
        int? TaxId,
        string Name,
        string Code,
        Core.Entities.GlobalData.TaxNature TaxNature,
        Core.Entities.GlobalData.ChargeType ChargeType,
        decimal Value);

    public Task<List<BidDataModel>> GetAuctionBidsAsync(
        int auctionId,
        CancellationToken cancellationToken = default)
        => _bidRepository.GetAuctionBidsAsync(auctionId);

    public Task<List<BidDataModel>> GetBidHistoryAsync(
        int vendorId,
        int auctionId,
        CancellationToken cancellationToken = default)
        => _bidRepository.GetBidHistory(vendorId, auctionId);

    public Task<List<BidDataModel>> GetLeaderBoardAsync(
        int auctionId,
        CancellationToken cancellationToken = default)
        => _bidRepository.GetLeaderBoard(auctionId);

    public Task<BidDataModel> GetByIdAsync(
        long bidId,
        CancellationToken cancellationToken = default)
        => _bidRepository.GetById(bidId);
}

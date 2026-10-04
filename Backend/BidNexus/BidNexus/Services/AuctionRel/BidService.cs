using API.Abstraction.AuctionRel;
using API.Models.AuctionRel;
using Core.Abstraction.AuctionRelated;
using Core.Abstraction.Services;
using Core.Entities.Auction;
using Core.Entities.Master;
using Core.Enumeration;
using Core.Models.AuctionRelated;

namespace API.Services.AuctionRel;

public class BidService(
    IBidRepository bidRepository,
    IAuctionRepository auctionRepository,
    IJwtHelperService jwtHelperService,
    IBidCoreService bidCoreService,
    IAuctionEngine auctionEngine) : IBidService
{
    private readonly IBidRepository _bidRepository = bidRepository;
    private readonly IAuctionRepository _auctionRepository = auctionRepository;
    private readonly IJwtHelperService _jwtHelper = jwtHelperService;
    private readonly IBidCoreService _bidCoreService = bidCoreService;
    private readonly IAuctionEngine _auctionEngine = auctionEngine;

    public async Task<BidResponseDataModel> ProcessBidAsync(
        BidCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.DiscountAmount != 0)
            throw new Core.Exceptions.ValidationException("Discounts are not supported by the auction bidding engine.");

        var vendorId = _jwtHelper.GetUserId();
        var tenantId = _jwtHelper.GetTenantId();

        var requirementIds = (request.BidDetails ?? [])
            .Select(x => x.AuctionRequirementId)
            .ToArray();

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

        var requirementMap = context.Requirements.ToDictionary(x => x.Id);
        var taxMasterMap = context.TaxMasters.ToDictionary(x => x.Id);
        var taxNatureMap = context.TaxNatures.ToDictionary(x => x.Id);
        var chargeTypeMap = context.ChargeTypes.ToDictionary(x => x.Id);

        var bidDetails = new List<BidDetail>();
        decimal basicAmount = 0m;
        decimal signedTaxAmount = 0m;

        foreach (var detailRequest in request.BidDetails ?? [])
        {
            requirementMap.TryGetValue(detailRequest.AuctionRequirementId, out var requirement);
            var quantity = requirement?.Quantity ?? 1m;

            var baseAmount = RoundMoney(
                detailRequest.Rate * quantity);

            var bidDetail = new BidDetail
            {
                AuctionRequirementId = detailRequest.AuctionRequirementId,
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
                    (ChargeTypeEnum)tax.ChargeType.Id,
                    tax.Value,
                    baseAmount,
                    quantity);

                var isDeductive = IsDeductiveTax(
                    (TaxNatureEnum)tax.TaxNature.Id);

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

            bidDetail.NetAmount = detailNetAmount;
            basicAmount += baseAmount;
            signedTaxAmount += detailTaxAmount;
            bidDetails.Add(bidDetail);
        }

        basicAmount = RoundMoney(basicAmount);
        signedTaxAmount = RoundMoney(signedTaxAmount);
        var netAmount = RoundMoney(basicAmount + signedTaxAmount);

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

        // Delegate all bid validation rules to Core Service CustomValidation
        var validationErrors = _bidCoreService.CustomValidation(bid, context, auction);
        if (validationErrors.Count > 0)
        {
            throw validationErrors[0];
        }

        return await _auctionEngine.ProcessBidAsync(bid, cancellationToken);
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
        ChargeTypeEnum chargeType,
        decimal taxValue,
        decimal baseAmount,
        decimal quantity)
    {
        if (taxValue < 0)
            throw new InvalidOperationException("TaxValue cannot be negative.");

        var amount = chargeType switch
        {
            ChargeTypeEnum.Percentage => baseAmount * taxValue / 100m,
            ChargeTypeEnum.PerUnit => quantity * taxValue,
            ChargeTypeEnum.Fixed => taxValue,
            _ => throw new InvalidOperationException($"Unsupported charge type '{(int)chargeType}'.")
        };

        return RoundMoney(amount);
    }

    private static bool IsDeductiveTax(TaxNatureEnum taxNature)
        => taxNature switch
        {
            TaxNatureEnum.Deductive => true,
            TaxNatureEnum.Additive => false,
            _ => throw new InvalidOperationException($"Unsupported tax nature '{(int)taxNature}'.")
        };

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

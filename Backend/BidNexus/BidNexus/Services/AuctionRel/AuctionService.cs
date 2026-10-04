using API.Abstraction.AuctionRel;
using API.Models.AuctionRel;
using Core.Abstraction.AuctionRelated;
using Core.Entities.Auction;
using Core.Enumeration;
using Core.Models.AuctionRelated;

namespace API.Services.AuctionRel;

public class AuctionService(IAuctionRepository auctionRepository) : IAuctionService
{
    private readonly IAuctionRepository _auctionRepository = auctionRepository;

    public async Task<AuctionResponseModel> CreateAsync(AuctionCreateRequest request, CancellationToken cancellationToken = default)
    {
        var auction = new Auction(
            request.AuctionName.Trim(),
            request.About?.Trim() ?? string.Empty,
            request.DocNoYearly.Trim(),
            request.DocDate,
            request.IsForwardAuction,
            request.AuctionStartTime,
            request.AuctionEndTime,
            request.DocAttachmentId,
            request.OpenToAll,
            request.IsBidPriceHidden,
            request.OrganizationId,
            request.StatusId == (short)StatusEnum.Authorized && request.OpenToAll ? (short)StatusEnum.Scheduled : request.StatusId
        )
        {
            AuctionRequirements = (request.AuctionRequirements ?? []).Select(r => new AuctionRequirement
            {
                LineNo = r.LineNo,
                ItemId = r.ItemId,
                TechnicalSpecification = r.TechnicalSpecification,
                Quantity = r.Quantity,
                UnitId = r.UnitId,
                DocumentAttachmentId = r.DocumentAttachmentId
            }).ToList()
        };

        return await _auctionRepository.AddAsync(auction);
    }

    public async Task<AuctionResponseModel> UpdateAsync(int id, AuctionUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var auction = new Auction(
            id,
            request.AuctionName.Trim(),
            request.About?.Trim() ?? string.Empty,
            request.DocNoYearly.Trim(),
            request.DocDate,
            request.IsForwardAuction,
            request.AuctionStartTime,
            request.AuctionEndTime,
            request.DocAttachmentId,
            request.OpenToAll,
            request.IsBidPriceHidden,
            request.OrganizationId,
            request.StatusId
        )
        {
            AuctionRequirements = (request.AuctionRequirements ?? []).Select(r => new AuctionRequirement
            {
                LineNo = r.LineNo,
                ItemId = r.ItemId,
                TechnicalSpecification = r.TechnicalSpecification,
                Quantity = r.Quantity,
                UnitId = r.UnitId,
                DocumentAttachmentId = r.DocumentAttachmentId
            }).ToList()
        };

        return await _auctionRepository.UpdateAsync(auction);
    }

    public async Task<AuctionDataModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _auctionRepository.GetById(id);
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    public async Task<List<AuctionGetDataModel>> GetAsync(short categoryId, short pageNo, short pageSize, CancellationToken cancellationToken = default)
    {
        return await _auctionRepository.Get(categoryId, pageNo, pageSize);
    }

    public async Task<List<AuctionGetDataModel>> GetPendingAsync(short categoryId, short pageNo, short pageSize, CancellationToken cancellationToken = default)
    {
        return await _auctionRepository.GetPendingAuctionsAsync(categoryId, pageNo, pageSize, cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _auctionRepository.DeleteAsync(id);
    }
}

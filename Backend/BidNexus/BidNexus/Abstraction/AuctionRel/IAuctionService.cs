using API.Models.AuctionRel;
using Core.Models.AuctionRelated;

namespace API.Abstraction.AuctionRel;

public interface IAuctionService
{
    Task<AuctionResponseModel> CreateAsync(AuctionCreateRequest request, CancellationToken cancellationToken = default);
    Task<AuctionResponseModel> UpdateAsync(int id, AuctionUpdateRequest request, CancellationToken cancellationToken = default);
    Task<AuctionDataModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<AuctionGetDataModel>> GetAsync(short categoryId, short pageNo, short pageSize, CancellationToken cancellationToken = default);
    Task<List<AuctionGetDataModel>> GetPendingAsync(short categoryId, short pageNo, short pageSize, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

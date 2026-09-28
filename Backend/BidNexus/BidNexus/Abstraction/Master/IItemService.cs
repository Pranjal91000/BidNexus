using API.Models.Master;

namespace API.Abstraction.Master;

public interface IItemService
{
    Task<IReadOnlyList<ItemResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ItemResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ItemResponse> CreateAsync(ItemCreateRequest request, CancellationToken cancellationToken = default);
    Task<ItemResponse> UpdateAsync(int id, ItemUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

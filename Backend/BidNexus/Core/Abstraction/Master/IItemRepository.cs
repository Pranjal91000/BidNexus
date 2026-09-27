using Core.Entities.Master;

namespace Core.Abstraction.Master;

public interface IItemRepository
{
    Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Item?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<Item> AddAsync(Item item, CancellationToken cancellationToken = default);
    Task UpdateAsync(Item item, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> UnitsExistAsync(IEnumerable<int> unitIds, CancellationToken cancellationToken = default);
    Task<bool> CategoryExistsAsync(short categoryId, CancellationToken cancellationToken = default);
    Task<bool> StatusExistsAsync(short statusId, CancellationToken cancellationToken = default);
}

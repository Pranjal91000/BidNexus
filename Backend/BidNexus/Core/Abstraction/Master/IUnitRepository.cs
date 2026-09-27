using Core.Entities.Master;

namespace Core.Abstraction.Master;

public interface IUnitRepository
{
    Task<IReadOnlyList<Unit>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Unit?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<Unit> AddAsync(Unit unit, CancellationToken cancellationToken = default);
    Task UpdateAsync(Unit unit, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> IsReferencedAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> StatusExistsAsync(short statusId, CancellationToken cancellationToken = default);
}

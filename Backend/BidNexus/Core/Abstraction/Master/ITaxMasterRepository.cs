using Core.Entities.Master;

namespace Core.Abstraction.Master;

public interface ITaxMasterRepository
{
    Task<IReadOnlyList<TaxMaster>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TaxMaster?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<TaxMaster> AddAsync(TaxMaster taxMaster, CancellationToken cancellationToken = default);
    Task UpdateAsync(TaxMaster taxMaster, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> TaxNatureExistsAsync(short taxNatureId, CancellationToken cancellationToken = default);
    Task<bool> ChargeTypeExistsAsync(short chargeTypeId, CancellationToken cancellationToken = default);
    Task<bool> StatusExistsAsync(short statusId, CancellationToken cancellationToken = default);
}

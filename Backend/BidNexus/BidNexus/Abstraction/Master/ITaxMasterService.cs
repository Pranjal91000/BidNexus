using API.Models.Master;

namespace API.Abstraction.Master;

public interface ITaxMasterService
{
    Task<IReadOnlyList<TaxMasterResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TaxMasterResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaxMasterResponse> CreateAsync(TaxMasterCreateRequest request, CancellationToken cancellationToken = default);
    Task<TaxMasterResponse> UpdateAsync(int id, TaxMasterUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

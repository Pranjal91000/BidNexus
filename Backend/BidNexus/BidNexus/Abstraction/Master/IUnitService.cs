using API.Models.Master;

namespace API.Abstraction.Master;

public interface IUnitService
{
    Task<IReadOnlyList<UnitResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UnitResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UnitResponse> CreateAsync(UnitCreateRequest request, CancellationToken cancellationToken = default);
    Task<UnitResponse> UpdateAsync(int id, UnitUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

using API.Abstraction.Master;
using API.Models.Master;
using Core.Abstraction.Master;
using Core.Abstraction.Services;
using Core.Entities.Master;
using Core.Exceptions;

namespace API.Services.Master;

public class UnitService(
    IUnitRepository unitRepository,
    IJwtHelperService jwtHelper) : IUnitService
{
    private readonly IUnitRepository _unitRepository = unitRepository;
    private readonly IJwtHelperService _jwtHelper = jwtHelper;

    public async Task<IReadOnlyList<UnitResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var units = await _unitRepository.GetAllAsync(cancellationToken);
        return units.Select(MapToResponse).ToList();
    }

    public async Task<UnitResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var unit = await _unitRepository.GetByIdAsync(id, cancellationToken);
        return unit is null ? null : MapToResponse(unit);
    }

    public async Task<UnitResponse> CreateAsync(UnitCreateRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = _jwtHelper.GetTenantId();

        var codeExists = await _unitRepository.ExistsByCodeAsync(request.Code, null, cancellationToken);
        if (codeExists)
            throw new ConflictException($"Unit with code '{request.Code}' already exists for this tenant.");

        var unit = new Unit
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            StatusId = request.StatusId,
            StatusRemarks = request.StatusRemarks ?? string.Empty
        };

        var created = await _unitRepository.AddAsync(unit, cancellationToken);
        return MapToResponse(created);
    }

    public async Task<UnitResponse> UpdateAsync(int id, UnitUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var unit = await _unitRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Unit with ID {id} not found.");

        var codeExists = await _unitRepository.ExistsByCodeAsync(request.Code, id, cancellationToken);
        if (codeExists)
            throw new ConflictException($"Unit with code '{request.Code}' already exists for this tenant.");

        unit.Name = request.Name.Trim();
        unit.Code = request.Code.Trim().ToUpperInvariant();
        unit.StatusId = request.StatusId;
        unit.StatusRemarks = request.StatusRemarks ?? string.Empty;

        await _unitRepository.UpdateAsync(unit, cancellationToken);
        return MapToResponse(unit);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var unit = await _unitRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Unit with ID {id} not found.");

        var isReferenced = await _unitRepository.IsReferencedAsync(id, cancellationToken);
        if (isReferenced)
            throw new ConflictException($"Unit with ID {id} is referenced by items and cannot be deleted.");

        return await _unitRepository.DeleteAsync(id, cancellationToken);
    }

    private static UnitResponse MapToResponse(Unit unit) => new()
    {
        Id = unit.Id,
        Name = unit.Name,
        Code = unit.Code,
        StatusId = unit.StatusId,
        StatusRemarks = unit.StatusRemarks,
        CreatedDateTime = unit.CreatedDateTime,
        LastModifiedDateTime = unit.LastModifiedDateTime
    };
}

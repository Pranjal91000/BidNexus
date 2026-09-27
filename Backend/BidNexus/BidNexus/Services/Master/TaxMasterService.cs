using API.Abstraction.Master;
using API.Models.Master;
using Core.Abstraction.Master;
using Core.Abstraction.Services;
using Core.Entities.Master;
using Core.Exceptions;

namespace API.Services.Master;

public class TaxMasterService(
    ITaxMasterRepository taxMasterRepository,
    IJwtHelperService jwtHelper) : ITaxMasterService
{
    private readonly ITaxMasterRepository _taxMasterRepository = taxMasterRepository;
    private readonly IJwtHelperService _jwtHelper = jwtHelper;

    public async Task<IReadOnlyList<TaxMasterResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var taxMasters = await _taxMasterRepository.GetAllAsync(cancellationToken);
        return taxMasters.Select(MapToResponse).ToList();
    }

    public async Task<TaxMasterResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var taxMaster = await _taxMasterRepository.GetByIdAsync(id, cancellationToken);
        return taxMaster is null ? null : MapToResponse(taxMaster);
    }

    public async Task<TaxMasterResponse> CreateAsync(TaxMasterCreateRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = _jwtHelper.GetTenantId();

        var taxNatureExists = await _taxMasterRepository.TaxNatureExistsAsync(request.TaxNatureId, cancellationToken);
        if (!taxNatureExists)
            throw new ValidationException($"TaxNature with ID {request.TaxNatureId} does not exist.");

        var chargeTypeExists = await _taxMasterRepository.ChargeTypeExistsAsync(request.ChargeTypeId, cancellationToken);
        if (!chargeTypeExists)
            throw new ValidationException($"ChargeType with ID {request.ChargeTypeId} does not exist.");

        var statusExists = await _taxMasterRepository.StatusExistsAsync(request.StatusId, cancellationToken);
        if (!statusExists)
            throw new ValidationException($"Status with ID {request.StatusId} does not exist.");

        var codeExists = await _taxMasterRepository.ExistsByCodeAsync(request.Code, null, cancellationToken);
        if (codeExists)
            throw new ConflictException($"TaxMaster with code '{request.Code}' already exists for this tenant.");

        var nameExists = await _taxMasterRepository.ExistsByNameAsync(request.Name, null, cancellationToken);
        if (nameExists)
            throw new ConflictException($"TaxMaster with name '{request.Name}' already exists for this tenant.");

        var taxMaster = new TaxMaster
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            TaxNatureId = request.TaxNatureId,
            ChargeTypeId = request.ChargeTypeId,
            TaxValue = request.TaxValue,
            StatusId = request.StatusId,
            StatusRemarks = request.StatusRemarks ?? string.Empty
        };

        var created = await _taxMasterRepository.AddAsync(taxMaster, cancellationToken);
        return MapToResponse(created);
    }

    public async Task<TaxMasterResponse> UpdateAsync(int id, TaxMasterUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var taxMaster = await _taxMasterRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"TaxMaster with ID {id} not found.");

        var taxNatureExists = await _taxMasterRepository.TaxNatureExistsAsync(request.TaxNatureId, cancellationToken);
        if (!taxNatureExists)
            throw new ValidationException($"TaxNature with ID {request.TaxNatureId} does not exist.");

        var chargeTypeExists = await _taxMasterRepository.ChargeTypeExistsAsync(request.ChargeTypeId, cancellationToken);
        if (!chargeTypeExists)
            throw new ValidationException($"ChargeType with ID {request.ChargeTypeId} does not exist.");

        var statusExists = await _taxMasterRepository.StatusExistsAsync(request.StatusId, cancellationToken);
        if (!statusExists)
            throw new ValidationException($"Status with ID {request.StatusId} does not exist.");

        var codeExists = await _taxMasterRepository.ExistsByCodeAsync(request.Code, id, cancellationToken);
        if (codeExists)
            throw new ConflictException($"TaxMaster with code '{request.Code}' already exists for this tenant.");

        var nameExists = await _taxMasterRepository.ExistsByNameAsync(request.Name, id, cancellationToken);
        if (nameExists)
            throw new ConflictException($"TaxMaster with name '{request.Name}' already exists for this tenant.");

        taxMaster.Name = request.Name.Trim();
        taxMaster.Code = request.Code.Trim().ToUpperInvariant();
        taxMaster.TaxNatureId = request.TaxNatureId;
        taxMaster.ChargeTypeId = request.ChargeTypeId;
        taxMaster.TaxValue = request.TaxValue;
        taxMaster.StatusId = request.StatusId;
        taxMaster.StatusRemarks = request.StatusRemarks ?? string.Empty;

        await _taxMasterRepository.UpdateAsync(taxMaster, cancellationToken);
        return MapToResponse(taxMaster);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var taxMaster = await _taxMasterRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"TaxMaster with ID {id} not found.");

        return await _taxMasterRepository.DeleteAsync(id, cancellationToken);
    }

    private static TaxMasterResponse MapToResponse(TaxMaster taxMaster) => new()
    {
        Id = taxMaster.Id,
        Name = taxMaster.Name,
        Code = taxMaster.Code,
        TaxNatureId = taxMaster.TaxNatureId,
        ChargeTypeId = taxMaster.ChargeTypeId,
        TaxValue = taxMaster.TaxValue,
        StatusId = taxMaster.StatusId,
        StatusRemarks = taxMaster.StatusRemarks,
        CreatedDateTime = taxMaster.CreatedDateTime,
        LastModifiedDateTime = taxMaster.LastModifiedDateTime
    };
}

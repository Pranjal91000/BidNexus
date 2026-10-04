using API.Abstraction.Master;
using API.Abstraction.Utilities;
using API.Models.Master;
using Core.Abstraction.Master;
using Core.Abstraction.Services;
using Core.Entities.Master;
using Core.Exceptions;

namespace API.Services.Master;

public class ItemService(
    IItemRepository itemRepository,
    IJwtHelperService jwtHelper,
    IAttachmentService attachmentService) : IItemService
{
    private readonly IItemRepository _itemRepository = itemRepository;
    private readonly IJwtHelperService _jwtHelper = jwtHelper;
    private readonly IAttachmentService _attachmentService = attachmentService;

    public async Task<IReadOnlyList<ItemResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _itemRepository.GetAllAsync(cancellationToken);
        return items.Select(MapToResponse).ToList();
    }

    public async Task<ItemResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByIdAsync(id, cancellationToken);
        return item is null ? null : MapToResponse(item);
    }

    public async Task<ItemResponse> CreateAsync(ItemCreateRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = _jwtHelper.GetTenantId();

        var categoryExists = await _itemRepository.CategoryExistsAsync(request.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new ValidationException($"Category with ID {request.CategoryId} does not exist.");

        var statusExists = await _itemRepository.StatusExistsAsync(request.StatusId, cancellationToken);
        if (!statusExists)
            throw new ValidationException($"Status with ID {request.StatusId} does not exist.");

        var unitIds = request.UnitIds ?? [];
        if (unitIds.Count > 0)
        {
            var unitsExist = await _itemRepository.UnitsExistAsync(unitIds, cancellationToken);
            if (!unitsExist)
                throw new ValidationException("One or more specified Unit IDs do not exist for this tenant.");
        }

        var codeExists = await _itemRepository.ExistsByCodeAsync(request.Code, null, cancellationToken);
        if (codeExists)
            throw new ConflictException($"Item with code '{request.Code}' already exists for this tenant.");

        var item = new Item
        {
            TenantId = tenantId,
            Name = request.Name.Trim(),
            Code = request.Code.Trim().ToUpperInvariant(),
            CategoryId = request.CategoryId,
            ItemDescription = request.ItemDescription ?? string.Empty,
            DocAttachmentId = request.DocAttachmentId,
            StatusId = request.StatusId,
            StatusRemarks = request.StatusRemarks ?? string.Empty,
            ApplicableUnits = unitIds.Distinct().Select(unitId => new ItemUnitMapping
            {
                UnitId = unitId,
                TenantId = tenantId
            }).ToList()
        };

        var created = await _itemRepository.AddAsync(item, cancellationToken);

        var response = MapToResponse(created);
        response.UnitIds = unitIds.Distinct().ToList();
        return response;
    }

    public async Task<ItemResponse> UpdateAsync(int id, ItemUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Item with ID {id} not found.");

        var categoryExists = await _itemRepository.CategoryExistsAsync(request.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new ValidationException($"Category with ID {request.CategoryId} does not exist.");

        var statusExists = await _itemRepository.StatusExistsAsync(request.StatusId, cancellationToken);
        if (!statusExists)
            throw new ValidationException($"Status with ID {request.StatusId} does not exist.");

        var unitIds = request.UnitIds ?? [];
        if (unitIds.Count > 0)
        {
            var unitsExist = await _itemRepository.UnitsExistAsync(unitIds, cancellationToken);
            if (!unitsExist)
                throw new ValidationException("One or more specified Unit IDs do not exist for this tenant.");
        }

        var codeExists = await _itemRepository.ExistsByCodeAsync(request.Code, id, cancellationToken);
        if (codeExists)
            throw new ConflictException($"Item with code '{request.Code}' already exists for this tenant.");

        if (item.DocAttachmentId.HasValue && item.DocAttachmentId != request.DocAttachmentId)
        {
            await _attachmentService.DeleteAttachmentAsync(item.DocAttachmentId.Value, cancellationToken);
        }

        item.Name = request.Name.Trim();
        item.Code = request.Code.Trim().ToUpperInvariant();
        item.CategoryId = request.CategoryId;
        item.ItemDescription = request.ItemDescription ?? string.Empty;
        item.DocAttachmentId = request.DocAttachmentId;
        item.StatusId = request.StatusId;
        item.StatusRemarks = request.StatusRemarks ?? string.Empty;

        item.ApplicableUnits ??= [];
        item.ApplicableUnits.Clear();
        foreach (var unitId in unitIds.Distinct())
        {
            item.ApplicableUnits.Add(new ItemUnitMapping
            {
                ItemId = item.Id,
                UnitId = unitId,
                TenantId = item.TenantId
            });
        }

        await _itemRepository.UpdateAsync(item, cancellationToken);

        var response = MapToResponse(item);
        response.UnitIds = unitIds.Distinct().ToList();
        return response;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _itemRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Item with ID {id} not found.");

        if (item.DocAttachmentId.HasValue)
        {
            await _attachmentService.DeleteAttachmentAsync(item.DocAttachmentId.Value, cancellationToken);
        }

        return await _itemRepository.DeleteAsync(id, cancellationToken);
    }

    private static ItemResponse MapToResponse(Item item) => new()
    {
        Id = item.Id,
        Name = item.Name,
        Code = item.Code,
        CategoryId = item.CategoryId,
        ItemDescription = item.ItemDescription,
        DocAttachmentId = item.DocAttachmentId,
        StatusId = item.StatusId,
        StatusRemarks = item.StatusRemarks,
        UnitIds = item.ApplicableUnits?.Select(m => m.UnitId).ToList() ?? [],
        CreatedDateTime = item.CreatedDateTime,
        LastModifiedDateTime = item.LastModifiedDateTime
    };
}

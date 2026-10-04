using Core.Abstraction.Master;
using Core.Abstraction.Services;
using Core.Entities.Master;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Master;

public class ItemRepository(AppDbContext dbContext, IJwtHelperService jwtHelperService) : IItemRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IJwtHelperService _jwtHelper = jwtHelperService;

    private int CurrentTenantId => _jwtHelper.GetTenantId();

    public async Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        IQueryable<Item> query = _dbContext.Items
            .AsNoTracking()
            .Include(x => x.ApplicableUnits);

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Item?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var query = _dbContext.Items
            .Include(x => x.ApplicableUnits)
            .AsQueryable();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        return await query
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var normalizedCode = code.Trim();
        var query = _dbContext.Items.AsNoTracking();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        return await query
            .AnyAsync(x => x.Code.ToLower() == normalizedCode.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    }

    public async Task<Item> AddAsync(Item item, CancellationToken cancellationToken = default)
    {
        if (item.TenantId == 0 && CurrentTenantId > 0)
        {
            item.TenantId = CurrentTenantId;
        }

        await _dbContext.Items.AddAsync(item, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task UpdateAsync(Item item, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        if (tenantId > 0 && item.TenantId != 0 && item.TenantId != tenantId)
            throw new UnauthorizedAccessException("Cannot update an Item belonging to another tenant.");

        _dbContext.Items.Update(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var query = _dbContext.Items.AsQueryable();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        var item = await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return false;

        _dbContext.Items.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UnitsExistAsync(IEnumerable<int> unitIds, CancellationToken cancellationToken = default)
    {
        var distinctIds = unitIds.Distinct().ToList();
        if (distinctIds.Count == 0) return true;

        var tenantId = CurrentTenantId;
        var query = _dbContext.Units.Where(u => distinctIds.Contains(u.Id));

        if (tenantId > 0)
            query = query.Where(u => u.TenantId == tenantId);

        var count = await query.CountAsync(cancellationToken);
        return count == distinctIds.Count;
    }

    public async Task<bool> CategoryExistsAsync(short categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
    }

    public async Task<bool> StatusExistsAsync(short statusId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Statuses.AnyAsync(s => s.Id == statusId, cancellationToken);
    }
}

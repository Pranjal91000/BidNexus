using Core.Abstraction.Master;
using Core.Abstraction.Services;
using Core.Entities.Master;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Master;

public class UnitRepository(AppDbContext dbContext, IJwtHelperService jwtHelperService) : IUnitRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IJwtHelperService _jwtHelper = jwtHelperService;

    private int CurrentTenantId => _jwtHelper.GetTenantId();

    public async Task<IReadOnlyList<Unit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var query = _dbContext.Units.AsNoTracking();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Unit?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var query = _dbContext.Units.AsQueryable();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        return await query
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var normalizedCode = code.Trim();
        var query = _dbContext.Units.AsNoTracking();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        return await query
            .AnyAsync(x => x.Code.ToLower() == normalizedCode.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    }

    public async Task<Unit> AddAsync(Unit unit, CancellationToken cancellationToken = default)
    {
        if (unit.TenantId == 0 && CurrentTenantId > 0)
        {
            unit.TenantId = CurrentTenantId;
        }

        await _dbContext.Units.AddAsync(unit, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return unit;
    }

    public async Task UpdateAsync(Unit unit, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        if (tenantId > 0 && unit.TenantId != 0 && unit.TenantId != tenantId)
            throw new UnauthorizedAccessException("Cannot update a Unit belonging to another tenant.");

        _dbContext.Units.Update(unit);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var query = _dbContext.Units.AsQueryable();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        var unit = await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (unit is null) return false;

        _dbContext.Units.Remove(unit);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> IsReferencedAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ItemUnitMappings.AnyAsync(m => m.UnitId == id, cancellationToken);
    }
}

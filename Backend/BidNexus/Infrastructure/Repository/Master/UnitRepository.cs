using Core.Abstraction.Master;
using Core.Entities.Master;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Master;

public class UnitRepository(AppDbContext dbContext) : IUnitRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<Unit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Units
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Unit?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Units
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim();
        return await _dbContext.Units
            .AnyAsync(x => x.Code.ToLower() == normalizedCode.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    }

    public async Task<Unit> AddAsync(Unit unit, CancellationToken cancellationToken = default)
    {
        await _dbContext.Units.AddAsync(unit, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return unit;
    }

    public async Task UpdateAsync(Unit unit, CancellationToken cancellationToken = default)
    {
        _dbContext.Units.Update(unit);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var unit = await _dbContext.Units.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (unit is null) return false;

        _dbContext.Units.Remove(unit);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> IsReferencedAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ItemUnitMappings.AnyAsync(m => m.UnitId == id, cancellationToken);
    }

    public async Task<bool> StatusExistsAsync(short statusId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Statuses.AnyAsync(s => s.Id == statusId, cancellationToken);
    }
}

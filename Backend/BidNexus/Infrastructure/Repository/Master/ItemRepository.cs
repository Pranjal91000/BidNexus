using Core.Abstraction.Master;
using Core.Entities.Master;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Master;

public class ItemRepository(AppDbContext dbContext) : IItemRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<Item>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Items
            .AsNoTracking()
            .Include(x => x.ApplicableUnits)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Item?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Items
            .Include(x => x.ApplicableUnits)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim();
        return await _dbContext.Items
            .AnyAsync(x => x.Code.ToLower() == normalizedCode.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    }

    public async Task<Item> AddAsync(Item item, CancellationToken cancellationToken = default)
    {
        await _dbContext.Items.AddAsync(item, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task UpdateAsync(Item item, CancellationToken cancellationToken = default)
    {
        _dbContext.Items.Update(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _dbContext.Items.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (item is null) return false;

        _dbContext.Items.Remove(item);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> UnitsExistAsync(IEnumerable<int> unitIds, CancellationToken cancellationToken = default)
    {
        var distinctIds = unitIds.Distinct().ToList();
        if (distinctIds.Count == 0) return true;

        var count = await _dbContext.Units
            .Where(u => distinctIds.Contains(u.Id))
            .CountAsync(cancellationToken);

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

using Core.Abstraction.Master;
using Core.Entities.Master;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Master;

public class TaxMasterRepository(AppDbContext dbContext) : ITaxMasterRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<TaxMaster>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaxMasters
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaxMaster?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaxMasters
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedCode = code.Trim();
        return await _dbContext.TaxMasters
            .AnyAsync(x => x.Code.ToLower() == normalizedCode.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim();
        return await _dbContext.TaxMasters
            .AnyAsync(x => x.Name.ToLower() == normalizedName.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    }

    public async Task<TaxMaster> AddAsync(TaxMaster taxMaster, CancellationToken cancellationToken = default)
    {
        await _dbContext.TaxMasters.AddAsync(taxMaster, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return taxMaster;
    }

    public async Task UpdateAsync(TaxMaster taxMaster, CancellationToken cancellationToken = default)
    {
        _dbContext.TaxMasters.Update(taxMaster);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var taxMaster = await _dbContext.TaxMasters.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (taxMaster is null) return false;

        _dbContext.TaxMasters.Remove(taxMaster);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> TaxNatureExistsAsync(short taxNatureId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaxNatures.AnyAsync(t => t.Id == taxNatureId, cancellationToken);
    }

    public async Task<bool> ChargeTypeExistsAsync(short chargeTypeId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChargeTypes.AnyAsync(c => c.Id == chargeTypeId, cancellationToken);
    }

    public async Task<bool> StatusExistsAsync(short statusId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Statuses.AnyAsync(s => s.Id == statusId, cancellationToken);
    }
}

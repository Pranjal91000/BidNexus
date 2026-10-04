using Core.Abstraction.Master;
using Core.Abstraction.Services;
using Core.Entities.Master;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Master;

public class TaxMasterRepository(AppDbContext dbContext, IJwtHelperService jwtHelperService) : ITaxMasterRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    private readonly IJwtHelperService _jwtHelper = jwtHelperService;

    private int CurrentTenantId => _jwtHelper.GetTenantId();

    public async Task<IReadOnlyList<TaxMaster>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var query = _dbContext.TaxMasters.AsNoTracking();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        return await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaxMaster?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var query = _dbContext.TaxMasters.AsQueryable();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        return await query
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var normalizedCode = code.Trim();
        var query = _dbContext.TaxMasters.AsNoTracking();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        return await query
            .AnyAsync(x => x.Code.ToLower() == normalizedCode.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var normalizedName = name.Trim();
        var query = _dbContext.TaxMasters.AsNoTracking();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        return await query
            .AnyAsync(x => x.Name.ToLower() == normalizedName.ToLower() && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
    }

    public async Task<TaxMaster> AddAsync(TaxMaster taxMaster, CancellationToken cancellationToken = default)
    {
        if (taxMaster.TenantId == 0 && CurrentTenantId > 0)
        {
            taxMaster.TenantId = CurrentTenantId;
        }

        await _dbContext.TaxMasters.AddAsync(taxMaster, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return taxMaster;
    }

    public async Task UpdateAsync(TaxMaster taxMaster, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        if (tenantId > 0 && taxMaster.TenantId != 0 && taxMaster.TenantId != tenantId)
            throw new UnauthorizedAccessException("Cannot update a TaxMaster belonging to another tenant.");

        _dbContext.TaxMasters.Update(taxMaster);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tenantId = CurrentTenantId;
        var query = _dbContext.TaxMasters.AsQueryable();

        if (tenantId > 0)
            query = query.Where(x => x.TenantId == tenantId);

        var taxMaster = await query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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

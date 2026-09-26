using Core.Abstraction.GlobalData;
using Core.Entities.GlobalData;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.GlobalData;

public class GlobalDataRepository(AppDbContext dbContext) : IGlobalDataRepository
{
    public async Task<IReadOnlyList<Category>> GetCategories(CancellationToken cancellationToken) =>
        await dbContext.Categories.AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ChargeType>> GetChargeTypes(CancellationToken cancellationToken) =>
        await dbContext.ChargeTypes.AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RatingFor>> GetRatingFors(CancellationToken cancellationToken) =>
        await dbContext.RatingFors.AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RatingParameter>> GetRatingParameters(CancellationToken cancellationToken) =>
        await dbContext.RatingParameters.AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Status>> GetStatuses(CancellationToken cancellationToken) =>
        await dbContext.Statuses.AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<TaxNature>> GetTaxNatures(CancellationToken cancellationToken) =>
        await dbContext.TaxNatures.AsNoTracking().OrderBy(x => x.Id).ToListAsync(cancellationToken);
}

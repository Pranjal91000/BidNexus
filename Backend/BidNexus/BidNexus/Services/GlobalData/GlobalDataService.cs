using API.Abstraction.GlobalData;
using Core.Abstraction.GlobalData;
using Core.Entities.GlobalData;

namespace API.Services.GlobalData;

public class GlobalDataService(IGlobalDataRepository repository) : IGlobalDataService
{
    public Task<IReadOnlyList<Category>> GetCategories(CancellationToken cancellationToken) =>
        repository.GetCategories(cancellationToken);

    public Task<IReadOnlyList<ChargeType>> GetChargeTypes(CancellationToken cancellationToken) =>
        repository.GetChargeTypes(cancellationToken);

    public Task<IReadOnlyList<RatingFor>> GetRatingFors(CancellationToken cancellationToken) =>
        repository.GetRatingFors(cancellationToken);

    public Task<IReadOnlyList<RatingParameter>> GetRatingParameters(CancellationToken cancellationToken) =>
        repository.GetRatingParameters(cancellationToken);

    public Task<IReadOnlyList<Status>> GetStatuses(CancellationToken cancellationToken) =>
        repository.GetStatuses(cancellationToken);

    public Task<IReadOnlyList<TaxNature>> GetTaxNatures(CancellationToken cancellationToken) =>
        repository.GetTaxNatures(cancellationToken);
}

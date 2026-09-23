using Core.Entities.GlobalData;

namespace API.Abstraction.GlobalData;

public interface IGlobalDataService
{
    Task<IReadOnlyList<Category>> GetCategories(CancellationToken cancellationToken);
    Task<IReadOnlyList<ChargeType>> GetChargeTypes(CancellationToken cancellationToken);
    Task<IReadOnlyList<RatingFor>> GetRatingFors(CancellationToken cancellationToken);
    Task<IReadOnlyList<RatingParameter>> GetRatingParameters(CancellationToken cancellationToken);
    Task<IReadOnlyList<Status>> GetStatuses(CancellationToken cancellationToken);
    Task<IReadOnlyList<TaxNature>> GetTaxNatures(CancellationToken cancellationToken);
}

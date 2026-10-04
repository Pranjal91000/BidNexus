using Core.Models.AuctionRelated;
using Core.Models.Tenant;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Abstraction.Profile
{
    public interface IProfileService
    {
        Task<TenantProfileDataModel> GetCurrentProfileAsync(CancellationToken cancellationToken = default);
        Task<TenantProfileDataModel> UpdateProfileAsync(UpdateProfileRequestModel request, CancellationToken cancellationToken = default);
        Task<OrganizationPublicProfileDataModel?> GetOrganizationPublicProfileAsync(int organizationId, CancellationToken cancellationToken = default);
        Task<List<AuctionGetDataModel>> GetOrganizationAuctionsAsync(int organizationId, CancellationToken cancellationToken = default);
    }
}

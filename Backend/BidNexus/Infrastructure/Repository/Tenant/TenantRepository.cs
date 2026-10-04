using Core.Abstraction.TenantRelated;
using Core.Models.Tenant;
using Microsoft.EntityFrameworkCore;
using TenantEntity = Core.Entities.TenantRelated.Tenant;

namespace Infrastructure.Repository.Tenant
{
    public class TenantRepository(AppDbContext dbContext) : ITenantRepository
    {
        private readonly AppDbContext appDbContext = dbContext;

        public async Task<TenantSaveResponseDataModel> Register(TenantEntity input)
        {
            await appDbContext.Tenants.AddAsync(input);
            var isSuccess = await appDbContext.SaveChangesAsync() > 0;

            if (!isSuccess) throw new InvalidOperationException("Failed to save Tenant.");

            return new TenantSaveResponseDataModel
            {
                Id = input.Id,
                Name = input.Name
            };
        }

        public async Task<bool> LinkTenantToUser(int referenceId, int tenantId)
        {
            var tenant = await appDbContext.Tenants.FirstOrDefaultAsync(x => x.Id == tenantId);
            if (tenant == null) throw new KeyNotFoundException();
            tenant.AddReferenceId(referenceId);

            var success = await appDbContext.SaveChangesAsync() > 0;

            if (!success) throw new InvalidOperationException("Failed to add Reference Id for Tenant.");

            return success;
        }

        public async Task<TenantProfileDataModel?> GetProfileAsync(int tenantId, CancellationToken cancellationToken = default)
        {
            var tenant = await appDbContext.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken);

            if (tenant == null) return null;

            var profile = new TenantProfileDataModel
            {
                TenantId = tenant.Id,
                Name = tenant.Name,
                EmailAddress = tenant.EmailAddress,
                UserName = tenant.UserName,
                ContactNumber = tenant.ContactNumber,
                Role = tenant.IsVendor ? "Vendor" : "Organization",
                ReferenceId = tenant.ReferenceId
            };

            if (tenant.IsVendor)
            {
                var vendor = await appDbContext.Vendors
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.TenantId == tenantId, cancellationToken);

                if (vendor != null)
                {
                    profile.About = vendor.About ?? string.Empty;
                    profile.ForegroundImageId = vendor.ForegroundImageId;
                    profile.ForegroundImageUrl = vendor.ForegroundImageId.HasValue
                        ? $"/api/attachments/{vendor.ForegroundImageId.Value}"
                        : null;
                }
            }
            else
            {
                var org = await appDbContext.Organizations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.TenantId == tenantId, cancellationToken);

                if (org != null)
                {
                    profile.About = org.About ?? string.Empty;
                    profile.OfficialAddress = org.OfficialAddress;
                    profile.ForegroundImageId = org.ForegroundImageId;
                    profile.ForegroundImageUrl = org.ForegroundImageId.HasValue
                        ? $"/api/attachments/{org.ForegroundImageId.Value}"
                        : null;
                }
            }

            return profile;
        }

        public async Task<bool> IsTenantNameTakenAsync(string name, int excludeTenantId, CancellationToken cancellationToken = default)
        {
            var normalized = name.Trim().ToLower();
            return await appDbContext.Tenants
                .AnyAsync(t => t.Id != excludeTenantId && t.Name.Trim().ToLower() == normalized, cancellationToken);
        }

        public async Task<TenantProfileDataModel> UpdateProfileAsync(int tenantId, string name, string about, int? foregroundImageId, CancellationToken cancellationToken = default)
        {
            var trimmedName = name.Trim();
            var trimmedAbout = about?.Trim() ?? string.Empty;

            var tenant = await appDbContext.Tenants
                .FirstOrDefaultAsync(x => x.Id == tenantId, cancellationToken)
                ?? throw new KeyNotFoundException($"Tenant with ID {tenantId} was not found.");

            if (await IsTenantNameTakenAsync(trimmedName, tenantId, cancellationToken))
            {
                throw new InvalidOperationException($"A tenant with the name '{trimmedName}' already exists.");
            }

            tenant.Name = trimmedName;

            if (tenant.IsVendor)
            {
                var vendor = await appDbContext.Vendors
                    .FirstOrDefaultAsync(v => v.TenantId == tenantId, cancellationToken);

                if (vendor != null)
                {
                    vendor.Name = trimmedName;
                    vendor.About = trimmedAbout;
                    vendor.ForegroundImageId = foregroundImageId;
                }
            }
            else
            {
                var org = await appDbContext.Organizations
                    .FirstOrDefaultAsync(o => o.TenantId == tenantId, cancellationToken);

                if (org != null)
                {
                    org.Name = trimmedName;
                    org.About = trimmedAbout;
                    org.ForegroundImageId = foregroundImageId;
                }
            }

            await appDbContext.SaveChangesAsync(cancellationToken);

            return (await GetProfileAsync(tenantId, cancellationToken))!;
        }

        public async Task<OrganizationPublicProfileDataModel?> GetOrganizationPublicProfileAsync(int organizationId, CancellationToken cancellationToken = default)
        {
            var org = await appDbContext.Organizations
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == organizationId, cancellationToken);

            if (org == null) return null;

            var auctions = await appDbContext.Auctions
                .AsNoTracking()
                .Where(a => a.OrganizationId == organizationId)
                .Select(a => new { a.Id, a.StatusId })
                .ToListAsync(cancellationToken);

            return new OrganizationPublicProfileDataModel
            {
                Id = org.Id,
                TenantId = org.TenantId,
                Name = org.Name,
                About = org.About ?? string.Empty,
                OfficialAddress = org.OfficialAddress,
                ForegroundImageId = org.ForegroundImageId,
                ForegroundImageUrl = org.ForegroundImageId.HasValue
                    ? $"/api/attachments/{org.ForegroundImageId.Value}"
                    : null,
                TotalAuctionsCount = auctions.Count,
                LiveAuctionsCount = auctions.Count(a => a.StatusId == 6) // Open status
            };
        }
    }
}

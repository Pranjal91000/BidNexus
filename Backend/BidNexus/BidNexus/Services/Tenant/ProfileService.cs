using API.Abstraction.Profile;
using API.Abstraction.Utilities;
using Core.Abstraction.Services;
using Core.Abstraction.TenantRelated;
using Core.Exceptions;
using Core.Models.AuctionRelated;
using Core.Models.Master;
using Core.Models.Models;
using Core.Models.Tenant;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Services.Profile
{
    public class ProfileService(
        ITenantRepository tenantRepository,
        IAttachmentService attachmentService,
        IJwtHelperService jwtHelper,
        AppDbContext dbContext) : IProfileService
    {
        private readonly ITenantRepository _tenantRepository = tenantRepository;
        private readonly IAttachmentService _attachmentService = attachmentService;
        private readonly IJwtHelperService _jwtHelper = jwtHelper;
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<TenantProfileDataModel> GetCurrentProfileAsync(CancellationToken cancellationToken = default)
        {
            var tenantId = _jwtHelper.GetTenantId();
            if (tenantId <= 0)
                throw new UnauthorizedException("Unauthorized or missing tenant context.");

            var profile = await _tenantRepository.GetProfileAsync(tenantId, cancellationToken);
            if (profile == null)
                throw new NotFoundException($"Profile for tenant {tenantId} was not found.");

            return profile;
        }

        public async Task<TenantProfileDataModel> UpdateProfileAsync(UpdateProfileRequestModel request, CancellationToken cancellationToken = default)
        {
            var tenantId = _jwtHelper.GetTenantId();
            if (tenantId <= 0)
                throw new UnauthorizedException("Unauthorized or missing tenant context.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException("Name cannot be empty.");

            var trimmedName = request.Name.Trim();
            var trimmedAbout = request.About?.Trim() ?? string.Empty;

            var isTaken = await _tenantRepository.IsTenantNameTakenAsync(trimmedName, tenantId, cancellationToken);
            if (isTaken)
            {
                throw new ConflictException($"A tenant with the name '{trimmedName}' already exists.");
            }

            var currentProfile = await _tenantRepository.GetProfileAsync(tenantId, cancellationToken);
            if (currentProfile != null &&
                currentProfile.ForegroundImageId.HasValue &&
                currentProfile.ForegroundImageId != request.ForegroundImageId)
            {
                // Old image replaced or removed; delete old file and free storage
                await _attachmentService.DeleteAttachmentAsync(currentProfile.ForegroundImageId.Value, cancellationToken);
            }

            return await _tenantRepository.UpdateProfileAsync(
                tenantId,
                trimmedName,
                trimmedAbout,
                request.ForegroundImageId,
                cancellationToken);
        }

        public async Task<OrganizationPublicProfileDataModel?> GetOrganizationPublicProfileAsync(int organizationId, CancellationToken cancellationToken = default)
        {
            return await _tenantRepository.GetOrganizationPublicProfileAsync(organizationId, cancellationToken);
        }

        public async Task<List<AuctionGetDataModel>> GetOrganizationAuctionsAsync(int organizationId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Auctions
                .AsNoTracking()
                .Include(a => a.Status)
                .Include(a => a.Organization)
                .Include(a => a.AuctionRequirements)
                    .ThenInclude(r => r.Item)
                .Include(a => a.AuctionRequirements)
                    .ThenInclude(r => r.Unit)
                .Where(a => a.OrganizationId == organizationId)
                .OrderByDescending(a => a.CreatedDateTime)
                .Select(a => new AuctionGetDataModel
                {
                    Id = a.Id,
                    DocNoYearly = a.DocNoYearly,
                    DocDate = a.DocDate,
                    IsForwardAuction = a.IsForwardAuction,
                    StatusName = a.Status.Name,
                    AuctionIntentSubmissionDate = a.AuctionIntentSubmissionDate,
                    Organization = new OrganizationOverviewDataModel
                    {
                        Name = a.Organization.Name,
                        OfficialAddress = a.Organization.OfficialAddress,
                        ForegroundImageId = a.Organization.ForegroundImageId,
                        About = a.Organization.About
                    },
                    AuctionRequirements = a.AuctionRequirements.Select(r => new AuctionRequirementDataModel
                    {
                        Id = r.Id,
                        LineNo = r.LineNo,
                        AuctionId = r.AuctionId,
                        ItemId = r.ItemId,
                        TechnicalSpecification = r.TechnicalSpecification,
                        Quantity = r.Quantity,
                        UnitId = r.UnitId,
                        DocumentAttachmentId = r.DocumentAttachmentId,
                        Item = new ItemDataModel
                        {
                            Id = r.Item.Id,
                            Name = r.Item.Name,
                            Code = r.Item.Code,
                            CategoryId = r.Item.CategoryId,
                            DocAttachmentId = r.Item.DocAttachmentId
                        },
                        Unit = new UnitDataModel
                        {
                            Id = r.Unit.Id,
                            Name = r.Unit.Name,
                            Code = r.Unit.Code
                        }
                    }).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}

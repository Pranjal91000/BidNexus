using API.Abstraction.Profile;
using Core.Exceptions;
using Core.Models.AuctionRelated;
using Core.Models.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers.Profile
{
    [ApiController]
    public class ProfileController(IProfileService profileService) : ControllerBase
    {
        private readonly IProfileService _profileService = profileService;

        [HttpGet("api/profile")]
        [HttpGet("api/tenant/profile")]
        [Authorize]
        [ProducesResponseType(typeof(TenantProfileDataModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TenantProfileDataModel>> GetProfile(CancellationToken cancellationToken)
        {
            var profile = await _profileService.GetCurrentProfileAsync(cancellationToken);
            return Ok(profile);
        }

        [HttpPut("api/profile")]
        [HttpPut("api/tenant/profile")]
        [Authorize]
        [ProducesResponseType(typeof(TenantProfileDataModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TenantProfileDataModel>> UpdateProfile([FromBody] UpdateProfileRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                var updated = await _profileService.UpdateProfileAsync(request, cancellationToken);
                return Ok(updated);
            }
            catch (ConflictException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("api/organizations/{id:int}/public")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(OrganizationPublicProfileDataModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrganizationPublicProfileDataModel>> GetOrganizationPublicProfile(int id, CancellationToken cancellationToken)
        {
            var org = await _profileService.GetOrganizationPublicProfileAsync(id, cancellationToken);
            if (org == null)
            {
                return NotFound(new { message = $"Organization with ID {id} was not found." });
            }

            return Ok(org);
        }

        [HttpGet("api/organizations/{id:int}/auctions")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<AuctionGetDataModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AuctionGetDataModel>>> GetOrganizationAuctions(int id, CancellationToken cancellationToken)
        {
            var auctions = await _profileService.GetOrganizationAuctionsAsync(id, cancellationToken);
            return Ok(auctions);
        }
    }
}

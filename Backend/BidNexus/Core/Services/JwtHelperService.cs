using Core.Abstraction.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Services
{
    public class JwtHelperService : IJwtHelperService
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public JwtHelperService(IHttpContextAccessor? httpContextAccessor = null)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
            => GetIntClaim("userId");

        public int GetTenantId()
            => GetIntClaim("tenantId");

        public string GetEmail()
            => _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)
               ?? string.Empty;

        public string GetSessionId()
            => _httpContextAccessor?.HttpContext?.User?.FindFirstValue("sessionId")
               ?? string.Empty;

        public string GetRole()
            => _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Role)
               ?? string.Empty;

        private int GetIntClaim(string claimType)
        {
            var value = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(claimType);
            return int.TryParse(value, out var result) ? result : 0;
        }
    }
}

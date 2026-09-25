using Core.Abstraction.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Core.Services
{

    public class JwtHelperService : IJwtHelperService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtHelperService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetUserId()
        {
            if (_httpContextAccessor?.HttpContext?.User is null)
                return 0;

            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            _ = int.TryParse(userIdClaim, out int result);
            return result;
        }

        public string GetEmail()
        {
            if (_httpContextAccessor?.HttpContext?.User is null)
                return string.Empty;

            var claims = _httpContextAccessor.HttpContext.User.Claims;
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        }

        public string GetSessionId()
        {
            if (_httpContextAccessor?.HttpContext?.User is null)
                return string.Empty;

            var claims = _httpContextAccessor.HttpContext.User.Claims;
            return claims.FirstOrDefault(c => c.Type == "sessionId")?.Value ?? string.Empty;
        }

        public int GetTenantId()
        {
            if (_httpContextAccessor?.HttpContext?.User is null)
                return 0;

            var claims = _httpContextAccessor.HttpContext.User.Claims;
            var userIdClaim = claims.FirstOrDefault(c => c.Type == "TenantId")?.Value;
            _ = int.TryParse(userIdClaim, out int result);
            return result;
        }
    }

}

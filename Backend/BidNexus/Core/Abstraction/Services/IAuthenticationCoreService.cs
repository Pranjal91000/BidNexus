using System;
using System.Security.Claims;

namespace Core.Abstraction.Services
{
    public interface IAuthenticationCoreService
    {
        string GenerateAuthToken(int userId, int tenantId, string email, string role);
        (string RefreshToken, DateTime ExpiresAt) GenerateRefreshToken(int userId, int tenantId, string email, string role);
        ClaimsPrincipal? ValidateRefreshToken(string refreshToken);
    }
}

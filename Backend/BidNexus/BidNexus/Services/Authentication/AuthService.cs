using API.Abstraction.Authentication;
using API.Models.Authentication;
using Core.Abstraction.Auth;
using Core.Abstraction.Services;

using System.IdentityModel.Tokens.Jwt;

namespace API.Services.Authentication
{
    public class AuthService(IAuthRepository authRepository, IAuthenticationCoreService authenticationCoreService, IConfiguration configuration) : IAuthService
    {
        private readonly IAuthRepository _authRepository = authRepository;
        private readonly IAuthenticationCoreService _authCoreService = authenticationCoreService;
        private readonly IConfiguration _configuration = configuration;

        public async Task<LoginViewModel> Login(LoginInputModel input)
        {
            var response = await _authRepository.ValidateLogin(input.Username, input.Password);

            if (response == null) throw new InvalidOperationException("Invalid Username or Password");

            var (refreshToken, refreshExpiresAt) = _authCoreService.GenerateRefreshToken(response.UserId, response.TenantId, response.Email, response.Role);

            var authExpiryMinutes = double.TryParse(_configuration["Jwt:AuthTokenExpiryMinutes"] ?? _configuration["Jwt:ExpiryMinutes"], out var result) ? result : 30;

            return new LoginViewModel
            {
                AccessToken = _authCoreService.GenerateAuthToken(response.UserId, response.TenantId, response.Email, response.Role),
                ExpiresAt = DateTime.UtcNow.AddMinutes(authExpiryMinutes),
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshExpiresAt
            };
        }

        public async Task<LoginViewModel> Refresh(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedAccessException("Refresh token is required.");

            var principal = _authCoreService.ValidateRefreshToken(refreshToken);
            if (principal == null)
                throw new UnauthorizedAccessException("Invalid or expired refresh token. Please log in again.");

            var tenantIdClaim = principal.FindFirst("tenantId")?.Value;
            if (!int.TryParse(tenantIdClaim, out var tenantId))
                throw new UnauthorizedAccessException("Invalid token claims.");

            var userStatus = await _authRepository.ValidateUserStatus(tenantId);
            if (userStatus == null)
                throw new UnauthorizedAccessException("User account is inactive or blocked. Please log in again.");

            var authExpiryMinutes = double.TryParse(_configuration["Jwt:AuthTokenExpiryMinutes"] ?? _configuration["Jwt:ExpiryMinutes"], out var result) ? result : 30;

            DateTime refreshExpiresAt = DateTime.UtcNow.AddHours(4);
            var expClaim = principal.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
            if (long.TryParse(expClaim, out var expUnix))
            {
                refreshExpiresAt = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            }

            return new LoginViewModel
            {
                AccessToken = _authCoreService.GenerateAuthToken(userStatus.UserId, userStatus.TenantId, userStatus.Email, userStatus.Role),
                ExpiresAt = DateTime.UtcNow.AddMinutes(authExpiryMinutes),
                RefreshToken = refreshToken,
                RefreshTokenExpiresAt = refreshExpiresAt
            };
        }
    }
}

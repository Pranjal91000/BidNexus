using Core.Abstraction.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.Services
{
    public class AuthenticationCoreService(IConfiguration configuration): IAuthenticationCoreService
    {
        private readonly IConfiguration _configuration = configuration;
        public string GenerateAuthToken(int userId, int tenantId, string email, string role)
        {
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is not configured.");

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var expiryMinutes = double.TryParse(_configuration["Jwt:ExpiryMinutes"], out var parsedMinutes) ? parsedMinutes : 30;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new("userId", userId.ToString()),
                new("tenantId", tenantId.ToString()),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Role, role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes((double)expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

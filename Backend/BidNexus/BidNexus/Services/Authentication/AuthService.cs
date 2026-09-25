using API.Abstraction.Authentication;
using API.Models.Authentication;
using Core.Abstraction.Auth;
using Core.Abstraction.Services;

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

            return new LoginViewModel
            {
                AccessToken = _authCoreService.GenerateAuthToken(response.UserId, response.TenantId, response.Email, response.Role),
                ExpiresAt = double.TryParse(_configuration["Jwt:ExpiryMinutes"], out var result) ? DateTime.UtcNow.AddMinutes(result) : DateTime.UtcNow.AddMinutes(30),
            };


        }
    }
}

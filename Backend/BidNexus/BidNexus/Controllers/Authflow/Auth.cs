using API.Abstraction.Authentication;
using API.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Authflow
{
    [ApiController]
    public class Auth(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        [HttpPost("api/login")]
        public async Task<IActionResult> Login([FromBody] LoginInputModel request)
        {
            var result = await _authService.Login(request);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [HttpPost("api/refresh")]
        [HttpPost("api/auth/refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenInputModel request)
        {
            try
            {
                var result = await _authService.Refresh(request.RefreshToken);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}

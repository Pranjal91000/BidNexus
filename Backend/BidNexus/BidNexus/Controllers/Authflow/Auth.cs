using API.Abstraction.Authentication;
using API.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Authflow
{
    public class Auth(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginInputModel request)
        {
            var result = await _authService.Login(request);
            return Ok(result);
        }
    }
}

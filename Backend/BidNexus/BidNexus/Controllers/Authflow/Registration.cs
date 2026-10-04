using API.Abstraction.Authentication;
using API.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Authflow
{
    [ApiController]
    public class Registration(IRegistrationService registration) : ControllerBase
    {
        private readonly IRegistrationService _registeredServices = registration;

        [HttpPost("Registration")]
        [HttpPost("api/registration")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegistrationInputModel input)
        {
            var response = await _registeredServices.Register(input);
            return Ok(response);
        }
    }
}

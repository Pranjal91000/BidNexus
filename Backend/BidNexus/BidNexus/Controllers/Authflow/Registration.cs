using API.Abstraction.Authentication;
using API.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Authflow
{
    public class Registration(IRegistrationService registration) : ControllerBase
    {
        private readonly IRegistrationService _registeredServices = registration;

        [HttpPost("Registration")]
        public async Task<IActionResult> RegisterAsync(RegistrationInputModel input)
        {
            var response = await _registeredServices.Register(input);
            return Ok(input);
        }
    }
}

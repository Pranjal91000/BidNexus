using API.Models.Authentication;

namespace API.Abstraction.Authentication
{
    public interface IRegistrationService
    {
        Task<RegistrationOutputModel> Register(RegistrationInputModel input);
    }
}

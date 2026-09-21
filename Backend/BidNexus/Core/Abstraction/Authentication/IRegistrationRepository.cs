using Core.Entities.Tenant;

namespace Core.Abstraction.Authentication
{
    public interface IRegistrationRepository
    {
        Task<bool> Register(Tenant input);
    }
}

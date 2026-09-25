using API.Models.Authentication;

namespace API.Abstraction.Authentication
{
    public interface IAuthService
    {
        public Task<LoginViewModel> Login(LoginInputModel input);
    }
}

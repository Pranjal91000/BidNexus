
namespace Core.Abstraction.Services
{
    public interface IJwtHelperService
    {
        string GetEmail();

        int GetUserId();

        string GetSessionId();
        public int GetTenantId();
    }

}
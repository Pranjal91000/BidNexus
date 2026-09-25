using Core.Abstraction.Auth;
using Core.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Auth
{
    public class AuthRepository(AppDbContext dbContext): IAuthRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        public async Task<AuthDataModel?> ValidateLogin(string username, string password)
        {
            var user = await _dbContext.Tenants.FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null) return null;
            return new AuthDataModel
            {
                Email = user.EmailAddress,
                Role = user.IsVendor ? "Vendor" : "Organization",
                UserId = user.ReferenceId,
                TenantId = user.Id
            };
        }
    }
}

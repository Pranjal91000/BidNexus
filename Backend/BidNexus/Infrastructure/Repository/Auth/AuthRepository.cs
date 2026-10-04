using Core.Abstraction.Auth;
using Core.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Auth
{
    public class AuthRepository(
        AppDbContext dbContext,
        IPasswordHasher<Core.Entities.TenantRelated.Tenant> passwordHasher) : IAuthRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly IPasswordHasher<Core.Entities.TenantRelated.Tenant> _passwordHasher = passwordHasher;

        public async Task<AuthDataModel?> ValidateLogin(
            string username,
            string password)
        {
            var user = await _dbContext.Tenants
                .FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null || user.IsBlocked)
                return null;

            var verification = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

            if (verification == PasswordVerificationResult.Failed)
                return null;

            if (verification == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, password);
                await _dbContext.SaveChangesAsync();
            }

            return new AuthDataModel
            {
                Email = user.EmailAddress,
                Role = user.IsVendor ? "Vendor" : "Organization",
                UserId = user.ReferenceId,
                TenantId = user.Id
            };
        }

        public async Task<AuthDataModel?> ValidateUserStatus(int tenantId)
        {
            var user = await _dbContext.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == tenantId);

            if (user == null || user.IsBlocked)
                return null;

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

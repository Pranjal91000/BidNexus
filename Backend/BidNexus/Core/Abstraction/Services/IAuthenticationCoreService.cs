using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Abstraction.Services
{
    public interface IAuthenticationCoreService
    {
        public string GenerateAuthToken(int userId, int tenantId, string email, string role);
    }
}

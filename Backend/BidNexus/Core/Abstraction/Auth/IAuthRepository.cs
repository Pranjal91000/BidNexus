using Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstraction.Auth
{
    public interface IAuthRepository
    {
        public Task<AuthDataModel?> ValidateLogin(string username, string password);
    }
}

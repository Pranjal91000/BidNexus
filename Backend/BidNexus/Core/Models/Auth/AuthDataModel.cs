using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Auth
{
    public class AuthDataModel
    {
        public int TenantId { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}

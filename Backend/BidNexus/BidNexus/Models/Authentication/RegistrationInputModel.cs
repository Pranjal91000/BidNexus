using System;
using System.Collections.Generic;
using System.Text;

namespace API.Models.Authentication
{
    public class RegistrationInputModel
    {
        public int Name { get; set; }
        public int ContactNumber { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RegisterAsOrganization { get; set; }
        public string OfficialAddress { get; set; } = string.Empty;
        public int? ForegroundImageId { get; set; }
        public string About { get; set; } = string.Empty;
    }
}

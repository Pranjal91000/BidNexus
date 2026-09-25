using System;

namespace API.Models.Authentication
{
    public class RegistrationInputModel
    {
        public string Name { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RegisterAsOrganization { get; set; }
        public string OfficialAddress { get; set; } = string.Empty;
        public int? ForegroundImageId { get; set; }
        public string About { get; set; } = string.Empty;
    }
}

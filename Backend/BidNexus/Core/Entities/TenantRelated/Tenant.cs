
namespace Core.Entities.TenantRelated
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int ReferenceId { get; set; }
        public bool IsVendor { get; set; }
        public bool IsBlocked { get; set; } = false;

        public Tenant() { }

        public Tenant( string name, string contactNumber, string emailAddress, string username, string password, bool isVendor)
        {
            Name = name;
            ContactNumber = contactNumber;
            EmailAddress = emailAddress;
            UserName = username;
            Password = password;
            IsVendor = isVendor;
        }

        public void AddReferenceId(int refrenceId)
        {
            ReferenceId = refrenceId;
        }

    }
}

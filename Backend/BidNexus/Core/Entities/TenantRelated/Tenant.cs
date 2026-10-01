namespace Core.Entities.TenantRelated
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int ReferenceId { get; set; }
        public bool IsVendor { get; set; }
        public bool IsBlocked { get; set; }

        public Tenant() { }

        public Tenant(
            string name,
            string contactNumber,
            string emailAddress,
            string username,
            string passwordHash,
            bool isVendor)
        {
            Name = name;
            ContactNumber = contactNumber;
            EmailAddress = emailAddress;
            UserName = username;
            PasswordHash = passwordHash;
            IsVendor = isVendor;
        }

        public void AddReferenceId(int referenceId)
        {
            ReferenceId = referenceId;
        }
    }
}

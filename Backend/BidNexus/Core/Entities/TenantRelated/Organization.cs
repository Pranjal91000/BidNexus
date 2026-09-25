using Core.Entities.Shared;

namespace Core.Entities.TenantRelated
{
    public class Organization: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string OfficialAddress { get; set; } = string.Empty;
        public int? ForegroundImageId { get; set; }
        public string About { get; set; } = string.Empty;
        public Tenant Tenant { get; set; } = null!;

        public Organization(int tenantId, string name, string officialAddress, string about)
        {
            TenantId = tenantId;
            Name = name;
            OfficialAddress = officialAddress;
            About = about;
        }

    }
}

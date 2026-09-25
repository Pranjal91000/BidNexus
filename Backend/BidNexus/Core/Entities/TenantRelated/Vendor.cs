using Core.Entities.Shared;

namespace Core.Entities.TenantRelated
{
    public class Vendor: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public int? ForegroundImageId { get; set; }
        public string About { get; set; } = string.Empty;
        public Tenant Tenant { get; set; } = null!;

        public Vendor(int tenantId, string name, string about)
        {
            TenantId = tenantId;
            Name = name;
            About = about;
        }

    }
}

using Core.Entities.Shared;

namespace Core.Entities.Tenant
{
    public class Vendor: BaseEntity
    {
        public int? ForegroundImageId { get; set; }
        public string About { get; set; } = string.Empty;

    }
}

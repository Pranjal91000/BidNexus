namespace Core.Models.Models
{
    public class VendorSaveResponseDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TenantId { get; set; }
    }
}

namespace Core.Models.Tenant
{
    public class VendorDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ForegroundImageId { get; set; }
        public string About { get; set; } = string.Empty;
    }

    public class VendorSaveResponseDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TenantId { get; set; }
    }
}

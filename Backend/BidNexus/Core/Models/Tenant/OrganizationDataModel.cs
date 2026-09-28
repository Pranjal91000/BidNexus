using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Models
{
    public class OrganizationSaveResponseDataModel
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class OrganizationOverviewDataModel
    {
        public string Name { get; set; } = string.Empty;
        public string OfficialAddress { get; set; } = string.Empty;
        public int? ForegroundImageId { get; set; }
        public string About { get; set; } = string.Empty;
    }
}

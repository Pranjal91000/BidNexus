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
}

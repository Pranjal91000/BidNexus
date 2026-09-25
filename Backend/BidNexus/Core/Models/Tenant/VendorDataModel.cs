using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Models
{
    public class VendorSaveResponseDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TenantId { get; set; }
    }
}

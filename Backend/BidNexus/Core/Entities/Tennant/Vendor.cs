using Core.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.User
{
    public class Vendor: BaseEntity
    {
        public int? ForegroundImageId { get; set; }
        public string About { get; set; } = string.Empty;

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Shared
{
    public class MasterBaseEntity: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; 
        public DateTimeOffset CreatedDateTime { get; set; }
        public DateTimeOffset LastModifiedDateTimeDateTime { get; set; }
        public short StatusId { get; set; }
        public string StatusRemarks { get; set; } = string.Empty;

    }
}

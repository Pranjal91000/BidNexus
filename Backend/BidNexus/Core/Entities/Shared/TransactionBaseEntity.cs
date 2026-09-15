using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Shared
{
    public class TransactionBaseEntity: BaseEntity
    {
        public string DocNoYearly { get; set; } = string.Empty;
        public DateOnly DocDate { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public DateTimeOffset LastModifiedDateTimedDateTime { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Shared
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public int TennantId { get; set; }
    }
}

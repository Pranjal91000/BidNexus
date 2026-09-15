using Core.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Master
{
    public class ItemUnitMapping: BaseEntity
    {
        public int ItemId { get; set; }
        public int UnitId { get; set; }
    }
}

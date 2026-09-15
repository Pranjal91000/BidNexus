using Core.Entities.GlobalData;
using Core.Entities.Shared;
using System.Collections;

namespace Core.Entities.Master
{
    public class Item: MasterBaseEntity
    {
        public short CategoryId { get; set; }
        public string ItemDescription { get; set; } = string.Empty;
        public int? DocAttachmentId { get; set; }
        public ICollection<ItemUnitMapping> ApplicableUnits { get; set; } = null!;
        public Categories Category { get; set; } = null!;
    }
}

using Core.Entities.GlobalData;
using Core.Entities.Shared;

namespace Core.Entities.Master
{
    public class TaxMaster : MasterBaseEntity
    {
        public short TaxNatureId { get; set; }
        public short ChargeTypeId { get; set; }
        public decimal TaxValue { get; set; }

        public TaxNature TaxNature { get; set; } = null!;
        public ChargeType ChargeType { get; set; } = null!;
        public Status Status { get; set; } = null!;
    }
}

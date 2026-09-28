using Core.Models.GlobalData;

namespace Core.Models.Master
{
    public class TaxMasterDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public short TaxNatureId { get; set; }
        public short ChargeTypeId { get; set; }
        public decimal TaxValue { get; set; }

        public TaxNatureDataModel? TaxNature { get; set; }
        public ChargeTypeDataModel? ChargeType { get; set; }
    }
}

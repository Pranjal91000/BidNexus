namespace Core.Entities.GlobalData
{
    public class ChargeType
    {
        public short Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

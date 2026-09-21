using System;
namespace Core.Entities.GlobalData
{
    public class TaxNature
    {
        public short Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}

using Core.Entities.Shared;

namespace Core.Entities.Auction
{
    public class VendorIntent: BaseEntity
    {
        public int VendorId { get; set; }
        public bool IsIntrested { get; set; }
        public bool IsQualified { get; set; }
    }
}

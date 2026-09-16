using Core.Entities.Shared;

namespace Core.Entities.Utilities
{
    public class Rating
    {
        public short RatingForId { get; set; }
        public int AuctionId { get; set; }
        public int AgainstTenant { get; set; }
        public int SubmittedByTenant { get; set; }
        public string? Remark { get; set; }
        public ICollection<RatingValue> RatingValues { get; set; } = null!;
    }
}

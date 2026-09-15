using Core.Entities.Shared;

namespace Core.Entities.Utilities
{
    public class Rating
    {
        public short RatingForId { get; set; }
        public int AuctionId { get; set; }
        public int AgainstTennant { get; set; }
        public int SubmittedByTennant { get; set; }
        public string? Remark { get; set; }
        public ICollection<RatingParameter> RatingParameters { get; set; } = null!;
    }
}

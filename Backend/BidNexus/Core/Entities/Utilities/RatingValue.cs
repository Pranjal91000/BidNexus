using Core.Entities.GlobalData;

namespace Core.Entities.Utilities
{
    public class RatingValue
    {
        public int Id { get; set; }
        public short RatingParameterId { get; set; }
        public short RatingScore { get; set; }
        public RatingParameter RatingParameter { get; set; } = new RatingParameter();
    }
}

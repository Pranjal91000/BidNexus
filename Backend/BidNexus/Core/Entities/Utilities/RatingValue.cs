using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Utilities
{
    public class RatingValue
    {
        public int Id { get; set; }
        public int RatingParameterId { get; set; }
        public short RatingScore { get; set; }
        public RatingParameter RatingParameter { get; set; }
    }
}

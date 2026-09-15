using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Utilities
{
    public class RatingValues
    {
        public int Id { get; set; }
        public int ParameterId { get; set; }
        public short RatingScore { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.GlobalData
{
    public class RatingParameter
    {
        public short Id { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public bool RatingFor { get; set; }
    }
}

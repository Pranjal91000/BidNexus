using Core.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Tennant
{
    public class Tennant
    {
        public int Id { get; set; }
        public int Name { get; set; }
        public int ContactNumber { get; set; }
        public string EmailAddress { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }
}

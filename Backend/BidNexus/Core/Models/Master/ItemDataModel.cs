using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models.Master
{
    public class ItemDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public short CategoryId { get; set; }
        public int? DocAttachmentId { get; set; }
        public string? DocAttachmentUrl { get; set; }
    }
}

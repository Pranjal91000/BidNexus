using System;

namespace Core.Entities.Utilities
{
    public class Attachment
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string StoredFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string RelativePath { get; set; } = string.Empty;
        public DateTimeOffset CreatedDateTime { get; set; } = DateTimeOffset.UtcNow;
    }
}

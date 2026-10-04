using System;
using System.Collections.Generic;

namespace Core.Models.Tenant
{
    public class TenantProfileDataModel
    {
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public int? ForegroundImageId { get; set; }
        public string? ForegroundImageUrl { get; set; }
        public string Role { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string? OfficialAddress { get; set; }
        public int ReferenceId { get; set; }
    }

    public class UpdateProfileRequestModel
    {
        public string Name { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public int? ForegroundImageId { get; set; }
    }

    public class OrganizationPublicProfileDataModel
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string About { get; set; } = string.Empty;
        public int? ForegroundImageId { get; set; }
        public string? ForegroundImageUrl { get; set; }
        public string? OfficialAddress { get; set; }
        public int TotalAuctionsCount { get; set; }
        public int LiveAuctionsCount { get; set; }
    }

    public class AttachmentDataModel
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string RelativePath { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}

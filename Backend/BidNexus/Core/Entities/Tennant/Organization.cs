using Core.Entities.Shared;

namespace Core.Entities.User
{
    public class Organization: BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string OfficialAddress { get; set; } = string.Empty;
        public int? ForegroundImageId { get; set; }
        public string About { get; set; } = string.Empty;
    }
}

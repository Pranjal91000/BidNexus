

using Core.Entities.GlobalData;
using Core.Entities.Shared;

namespace Core.Entities.Authentication
{
    public class LoginAttempt: BaseEntity
    {
        public short FailedLoginAttemptCount { get; set; }
        public DateTimeOffset LastAttemptedOn { get; set; }
        public short StatusId { get; set; }
        public Status Status { get; set; } = null!;
    }
}

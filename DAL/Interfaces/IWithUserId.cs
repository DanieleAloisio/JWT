using System;

namespace JWT.Areas.Identity.Interfaces
{
    public interface IWithUserId
    {
        Guid UserId { get; }
    }
}

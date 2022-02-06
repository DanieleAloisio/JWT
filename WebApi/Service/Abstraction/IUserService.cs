using JWT.Areas.Identity.Data;
using System;
using System.Threading.Tasks;

namespace WebApi.Service.Abstraction
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByIdAsync(Guid userId);
    }
}

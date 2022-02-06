using JWT.Areas.Identity.Data;
using JWT.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Service.Abstraction;

namespace WebApi.Service.Implementation
{
    public class UserService: IUserService
    {
        private readonly JWTLocalDbContext _context;

        public UserService(IHttpContextAccessor httpContextAccessor,
            JWTLocalDbContext context)
        {
            _context = context;
        }
        public async Task<ApplicationUser> GetUserByIdAsync(Guid userId) => await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);

    }
}

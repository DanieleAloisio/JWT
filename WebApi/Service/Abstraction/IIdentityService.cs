using JWT.Dto;
using System;
using System.Threading.Tasks;
using WebApi.Dto.Requests;

namespace JWT.Service.Abstraction
{
    public interface IIdentityService
    {
        Task<AuthResponse> LoginAsync(string email, string password);

        Task<BaseResponse> RegisterAsync(RegisterRequest request);
        Task<BaseResponse> RegisterAsync(string email, string password, string firstName, string lastName);
    }
}

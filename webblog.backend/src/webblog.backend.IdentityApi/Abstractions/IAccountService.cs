using System.Security.Claims;
using webblog.backend.IdentityApi.DtoModels;

namespace webblog.backend.IdentityApi.Abstractions
{
    public interface IAccountService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto, HttpContext httpContext);
        Task<UserInfoDto?> GetUserInfoAsync(ClaimsPrincipal user);
    }
}

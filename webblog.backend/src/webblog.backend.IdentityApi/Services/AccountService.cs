using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.RegularExpressions;
using webblog.backend.IdentityApi.Abstractions;
using webblog.backend.IdentityApi.DtoModels;
using webblog.backend.IdentityApi.Models;

namespace webblog.backend.IdentityApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var userName = ExtractUserName(registerDto.Email);
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new RegisterResponseDto
            {
                Success = true,
                UserId = user.Id.ToString()
            };
        }

        private string ExtractUserName(string email)
        {
            var userName = email.Split('@')[0];

            userName = Regex.Replace(userName, @"[^a-zA-Z0-9]", "");

            return userName;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, HttpContext httpContext)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Errors = new List<string> { "Неверное имя пользователя или пароль." }
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Errors = new List<string> { "Неверное имя пользователя или пароль." }
                };
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return new LoginResponseDto
            {
                Success = true,
                UserId = user.Id.ToString()
            };
        }

        public async Task<UserInfoDto?> GetUserInfoAsync(ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);
            var currentUser = await _userManager.FindByIdAsync(userId);

            if (currentUser == null)
                return null;

            return new UserInfoDto
            {
                UserId = currentUser.Id.ToString(),
                Username = currentUser.UserName,
                Email = currentUser.Email
            };
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using webblog.backend.IdentityApi.Abstractions;
using webblog.backend.IdentityApi.DtoModels;

/// <summary>
/// Контроллер по работе с аккаунтами пользователей
/// </summary>
[EnableCors]
[ApiController]
[Route("api/identity/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    /// <summary>
    /// Конструктор контроллера аккаунтов
    /// </summary>
    /// <param name="accountService">Сервис по работе с аккаунтами</param>
    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    /// <param name="registerDto">Данные для регистрации.</param>
    [HttpPost("register")]
    public async Task<RegisterResponseDto> Register([FromBody] RegisterDto registerDto)
    {
        return await _accountService.RegisterAsync(registerDto);
    }

    /// <summary>
    /// Авторизация пользователя.
    /// </summary>
    /// <param name="loginDto">Данные для авторизации.</param>
    [HttpPost("login")]
    public async Task<LoginResponseDto> Login([FromBody] LoginDto loginDto)
    {
        return await _accountService.LoginAsync(loginDto, HttpContext);
    }

    /// <summary>
    /// Проверка, авторизован ли пользователь.
    /// </summary>
    [HttpGet("validate")]
    public ValidateResponseDto Validate()
    {
        return new ValidateResponseDto
        {
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false
        };
    }

    /// <summary>
    /// Получение информации о текущем пользователе.
    /// </summary>
    [Authorize]
    [HttpGet("userinfo")]
    public async Task<UserInfoDto?> UserInfo()
    {
        return await _accountService.GetUserInfoAsync(User);
    }
}
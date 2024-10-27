using Microsoft.AspNetCore.Identity;

namespace webblog.backend.IdentityApi.Models
{
    /// <summary>
    /// Аккаунт пользователя
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
    }
}

using Microsoft.AspNetCore.Identity;

namespace webblog.backend.IdentityApi.Models
{
    /// <summary>
    /// Роль для аккаунтов
    /// </summary>
    public class ApplicationRole : IdentityRole<Guid>
    {
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webblog.backend.IdentityApi.Models;

namespace webblog.backend.IdentityApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>(entity => { entity.ToTable("Users"); });
            builder.Entity<ApplicationRole>(entity => { entity.ToTable("Roles"); });
            builder.Entity<IdentityUserRole<Guid>>(entity => { entity.ToTable("UserRoles"); });
            builder.Entity<IdentityUserClaim<Guid>>(entity => { entity.ToTable("UserClaims"); });
            builder.Entity<IdentityUserLogin<Guid>>(entity => { entity.ToTable("UserLogins"); });
            builder.Entity<IdentityRoleClaim<Guid>>(entity => { entity.ToTable("RoleClaims"); });
            builder.Entity<IdentityUserToken<Guid>>(entity => { entity.ToTable("UserTokens"); });
        }
    }
}

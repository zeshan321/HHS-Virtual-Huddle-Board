using HHSBoard.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HHSBoard.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private IHttpContextAccessor httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Unit> Units { get; set; }

        public DbSet<Board> Boards { get; set; }

        public DbSet<Celebration> Celebrations { get; set; }

        public DbSet<Default> Defaults { get; set; }

        public DbSet<Purpose> Purpose { get; set; }

        public DbSet<WIP> WIPs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            var username = !string.IsNullOrEmpty(httpContextAccessor?.HttpContext?.User?.Identity?.Name)
            ? httpContextAccessor.HttpContext.User.Identity.Name
            : "System-NoUser";

            AddTimestamps(username);
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var username = !string.IsNullOrEmpty(httpContextAccessor?.HttpContext?.User?.Identity?.Name)
            ? httpContextAccessor.HttpContext.User.Identity.Name
            : "System-NoUser";

            AddTimestamps(username);
            return await base.SaveChangesAsync();
        }

        private void AddTimestamps(string currentUsername)
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified) || x.State == EntityState.Deleted);

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).DateCreated = DateTime.UtcNow;
                    ((BaseEntity)entity.Entity).UserCreated = currentUsername;
                }

                if (entity.State == EntityState.Modified)
                {
                    ((BaseEntity)entity.Entity).DateModified = DateTime.UtcNow;
                    ((BaseEntity)entity.Entity).UserModified = currentUsername;
                }
            }
        }
    }
}
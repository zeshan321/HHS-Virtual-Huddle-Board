using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HHSBoard.Models;
using Microsoft.AspNetCore.Http;
using System.Threading;
using HHSBoard.Services;

namespace HHSBoard.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly string currentUsername;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IUserResolverService userService)
            : base(options)
        {
            currentUsername = !string.IsNullOrEmpty(userService?.GetUser())
            ? userService.GetUser()
            : "System-NoUser";
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
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return await base.SaveChangesAsync();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            
            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).DateCreated = DateTime.UtcNow;
                    ((BaseEntity)entity.Entity).UserCreated = currentUsername;
                }

                ((BaseEntity)entity.Entity).DateModified = DateTime.UtcNow;
                ((BaseEntity)entity.Entity).UserModified = currentUsername;
            }
        }
    }
}

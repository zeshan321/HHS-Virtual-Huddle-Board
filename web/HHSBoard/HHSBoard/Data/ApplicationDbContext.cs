﻿using HHSBoard.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        public DbSet<Audit> Audits { get; set; }

        public DbSet<Unit> Units { get; set; }

        public DbSet<Board> Boards { get; set; }

        public DbSet<Celebration> Celebrations { get; set; }

        public DbSet<Default> Defaults { get; set; }

        public DbSet<Purpose> Purpose { get; set; }

        public DbSet<WIP> WIPs { get; set; }

        public DbSet<NewImpOp> NewImpOps { get; set; }

        public DbSet<UnitAccess> UnitAccesses { get; set; }

        public DbSet<ImpIdeasImplemented> ImpIdeasImplemented { get; set; }

        public DbSet<ChangeRequest> ChangeRequests { get; set; }

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

            if (username.Equals("System-NoUser"))
            {
                return base.SaveChanges();
            }

            AddTimestamps(username);
            var auditEntries = OnBeforeSaveChanges(username);
            var result = base.SaveChanges();
            OnAfterSaveChanges(auditEntries);
            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var username = !string.IsNullOrEmpty(httpContextAccessor?.HttpContext?.User?.Identity?.Name)
            ? httpContextAccessor.HttpContext.User.Identity.Name
            : "System-NoUser";

            if (username.Equals("System-NoUser"))
            {
                return await base.SaveChangesAsync();
            }

            AddTimestamps(username);
            var auditEntries = OnBeforeSaveChanges(username);
            var result = await base.SaveChangesAsync();
            await OnAfterSaveChanges(auditEntries);
            return result;
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

        private List<AuditEntry> OnBeforeSaveChanges(string currentUsername)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Metadata.Relational().TableName,
                    Username = currentUsername,
                    State = entry.State.ToString()
                };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        // value will be generated by the database, get the value after saving
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }

            // Save audit entities that have all the modifications
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                Audits.Add(auditEntry.ToAudit());
            }

            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }

        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                // Get the final value of the temporary properties
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                // Save the Audit entry
                Audits.Add(auditEntry.ToAudit());
            }

            return SaveChangesAsync();
        }
    }
}
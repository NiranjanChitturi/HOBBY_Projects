using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MPHMS.Infrastructure.Identity;
using MPHMS.Domain.Common;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MPHMS.Infrastructure.Persistence
{
    /// <summary>
    /// ApplicationDbContext is the CENTRAL database context for MPHMS.
    ///
    /// Responsibilities:
    /// -----------------
    /// 1) Integrates ASP.NET Identity tables
    /// 2) Manages MPHMS business entities
    /// 3) Applies soft delete global filters
    /// 4) Automatically manages audit fields
    ///
    /// This context inherits from IdentityDbContext so that:
    /// - AspNetUsers
    /// - AspNetRoles
    /// - AspNetUserRoles
    /// are created and managed automatically.
    ///
    /// Architecture Layer:
    /// -------------------
    /// Infrastructure.Persistence
    ///
    /// This is the ONLY class that communicates directly with SQL Server.
    /// </summary>
    public class ApplicationDbContext 
        : IdentityDbContext<ApplicationUser, 
                            Microsoft.AspNetCore.Identity.IdentityRole<Guid>, 
                            Guid>
    {
        /// <summary>
        /// Constructor used by Dependency Injection.
        ///
        /// DbContextOptions contains:
        /// - Connection string
        /// - Provider (SQL Server)
        /// - Logging configuration
        /// </summary>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Override for configuring entity mappings and global filters.
        ///
        /// This method executes ONCE at application startup.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ---------------------------------------------------------
            // Apply GLOBAL soft-delete filter
            // ---------------------------------------------------------
            //
            // This ensures:
            // SELECT queries automatically exclude IsDeleted = true
            //
            // Developers NEVER need to remember to add:
            // WHERE IsDeleted = 0
            //
            // This is enterprise best practice.
            //

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // Only apply filter to entities inheriting BaseAuditableEntity
                if (typeof(BaseAuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(ApplicationDbContext)
                        .GetMethod(nameof(SetSoftDeleteFilter),
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Static)
                        ?.MakeGenericMethod(entityType.ClrType);

                    method?.Invoke(null, new object[] { builder });
                }
            }
        }

        /// <summary>
        /// Generic method that applies soft delete filter.
        ///
        /// Expression:
        /// entity => entity.IsDeleted == false
        /// </summary>
        private static void SetSoftDeleteFilter<TEntity>(ModelBuilder builder)
            where TEntity : BaseAuditableEntity
        {
            builder.Entity<TEntity>()
                   .HasQueryFilter(e => !e.IsDeleted);
        }

        // ---------------------------------------------------------
        // AUDIT AUTOMATION
        // ---------------------------------------------------------

        /// <summary>
        /// Override SaveChangesAsync to automatically populate:
        ///
        /// CreatedAt
        /// ModifiedAt
        /// DeletedAt
        ///
        /// This ensures NO developer manually sets audit fields.
        /// </summary>
        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Applies audit values before EF commits changes.
        /// </summary>
        private void ApplyAuditInformation()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseAuditableEntity &&
                       (e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted));

            foreach (var entry in entries)
            {
                var entity = (BaseAuditableEntity)entry.Entity;

                // Creation
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }

                // Modification
                if (entry.State == EntityState.Modified)
                {
                    entity.ModifiedAt = DateTime.UtcNow;
                }

                // Soft Delete Intercept
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;

                    entity.IsDeleted = true;
                    entity.DeletedAt = DateTime.UtcNow;
                }
            }
        }
    }
}

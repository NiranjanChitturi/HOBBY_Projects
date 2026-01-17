using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MPHMS.Infrastructure.Identity;
using MPHMS.Infrastructure.Persistence;
using System;
using MPHMS.Application.Repositories;
using MPHMS.Infrastructure.Persistence.Repositories;

namespace MPHMS.Infrastructure.DependencyInjection
{
    /// <summary>
    /// ServiceRegistration is the SINGLE ENTRY POINT
    /// for registering Infrastructure layer services.
    ///
    /// Purpose:
    /// --------
    /// This keeps Program.cs clean and enforces
    /// Clean Architecture boundaries.
    ///
    /// All database, identity, and infrastructure dependencies
    /// are configured here and injected via extension method.
    /// </summary>
    public static class ServiceRegistration
    {
        /// <summary>
        /// Registers Infrastructure services into the dependency container.
        ///
        /// This includes:
        /// - ApplicationDbContext (EF Core)
        /// - SQL Server provider
        /// - ASP.NET Identity
        /// - Identity configuration rules
        ///
        /// Called from:
        /// MPHMS.Api
        /// MPHMS.Web
        /// Startup/Program.cs
        /// </summary>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ------------------------------------------------------
            // Database Configuration
            // ------------------------------------------------------

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        // Enables retry on transient SQL failures
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
            });
// ------------------------------------------------------
// Repository & Unit Of Work Registration
// ------------------------------------------------------

services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ------------------------------------------------------
            // Identity Configuration
            // ------------------------------------------------------

            // services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            // {
            //     // Password policy
            //     options.Password.RequireDigit = true;
            //     options.Password.RequiredLength = 8;
            //     options.Password.RequireUppercase = true;
            //     options.Password.RequireLowercase = true;
            //     options.Password.RequireNonAlphanumeric = false;

            //     // Account lockout policy
            //     options.Lockout.MaxFailedAccessAttempts = 5;
            //     options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

            //     // User rules
            //     options.User.RequireUniqueEmail = true;
            // })
            // .AddEntityFrameworkStores<ApplicationDbContext>()
            // .AddDefaultTokenProviders();
            services.AddIdentityCore<ApplicationUser>(options =>
            {
                // Password policy (can tighten later)
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;

                // Lockout policy
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()               // Enable Role support
            .AddEntityFrameworkStores<ApplicationDbContext>(); // Persist Identity to SQL
             

            return services;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MPHMS.Infrastructure.Persistence;
using System;
using System.IO;

namespace MPHMS.Infrastructure.Persistence
{
    /// <summary>
    /// Design-time DbContext factory.
    ///
    /// Used ONLY by EF Core CLI tools:
    /// - dotnet ef migrations add
    /// - dotnet ef database update
    ///
    /// Runtime execution NEVER uses this class.
    /// </summary>
    public class ApplicationDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // --------------------------------------------------
            // Load API project's appsettings.json
            // --------------------------------------------------

            // var basePath = Path.Combine(
            //     Directory.GetCurrentDirectory(),
            //     "../MPHMS.Api");
            var basePath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory,
        "../../../../MPHMS.Api"));

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // --------------------------------------------------
            // Read connection string
            // --------------------------------------------------

            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "DefaultConnection string is missing in appsettings.json");
            }

            // --------------------------------------------------
            // Configure DbContext
            // --------------------------------------------------

            var optionsBuilder =
                new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}

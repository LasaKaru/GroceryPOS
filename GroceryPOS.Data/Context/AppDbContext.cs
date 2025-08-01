using GroceryPOS.Core.Models; // For User and EntityBase
using GroceryPOS.Data.Configuration;
using GroceryPOS.Utilities.Logging; // For logging database errors
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryPOS.Data.Context
{
    //public class AppDbContext : DbContext
    //{
    //    private readonly string _databasePath;

    //    // Constructor for runtime with a specific database path
    //    public AppDbContext(string databasePath)
    //    {
    //        _databasePath = databasePath;
    //    }

    //    // This constructor is specifically for Entity Framework Core CLI tools (for migrations).
    //    // It allows the tools to create an instance of your DbContext when running commands like Add-Migration or Update-Database.
    //    // When using the tools, they won't know the _databasePath unless you configure it,
    //    // so we'll set a default for design-time operations.
    //    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    //    {
    //        // This constructor is used by the EF Core tooling.
    //        // It's important that it's present, even if empty, for migrations to work correctly.
    //        // The database path will typically be configured via Dependency Injection in the app's startup.
    //        // For design-time, EF Core often resolves connection strings from appsettings.json or a default location.
    //        // We'll handle runtime database path setting via the other constructor.
    //    }

    //    // Define DbSet properties for your models, corresponding to database tables
    //    public DbSet<User> Users { get; set; }
    //    // public DbSet<Item> Items { get; set; } // Will add later
    //    // public DbSet<Sale> Sales { get; set; } // Will add later
    //    // public DbSet<Customer> Customers { get; set; } // Will add later
    //    // etc.

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    {
    //        // Only configure if _databasePath is set (i.e., not during design-time migrations, unless explicitly set)
    //        if (!string.IsNullOrEmpty(_databasePath))
    //        {
    //            // Ensure the directory for the database exists
    //            var dbDirectory = Path.GetDirectoryName(_databasePath);
    //            if (dbDirectory != null && !Directory.Exists(dbDirectory))
    //            {
    //                try
    //                {
    //                    Directory.CreateDirectory(dbDirectory);
    //                    AppLogger.LogInfo($"Database directory created: {dbDirectory}");
    //                }
    //                catch (Exception ex)
    //                {
    //                    AppLogger.LogError($"Failed to create database directory {dbDirectory}. {ex.Message}", ex);
    //                    // Potentially re-throw or handle more severely if essential
    //                    throw; // Re-throw to indicate a critical setup failure
    //                }
    //            }

    //            try
    //            {
    //                optionsBuilder.UseSqlite($"Data Source={_databasePath}");
    //            }
    //            catch (Exception ex)
    //            {
    //                AppLogger.LogError($"Failed to configure SQLite with path '{_databasePath}'. {ex.Message}", ex);
    //                throw; // Re-throw to indicate a critical setup failure
    //            }
    //        }
    //        base.OnConfiguring(optionsBuilder);
    //    }

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        // Apply configurations for your entities here
    //        modelBuilder.ApplyConfiguration(new UserConfiguration()); // UNCOMMENT THIS LINE
    //        base.OnModelCreating(modelBuilder);
    //    }

    //    public override int SaveChanges()
    //    {
    //        // Intercept changes to update ModifiedDate and CreatedDate
    //        UpdateAuditFields();
    //        return base.SaveChanges();
    //    }

    //    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    //    {
    //        // Intercept changes to update ModifiedDate and CreatedDate
    //        UpdateAuditFields();
    //        return await base.SaveChangesAsync(cancellationToken);
    //    }

    //    private void UpdateAuditFields()
    //    {
    //        foreach (var entry in ChangeTracker.Entries<EntityBase>())
    //        {
    //            if (entry.State == EntityState.Added)
    //            {
    //                entry.Entity.CreatedDate = DateTime.UtcNow;
    //                entry.Entity.ModifiedDate = null; // Ensure ModifiedDate is null on creation
    //            }
    //            else if (entry.State == EntityState.Modified)
    //            {
    //                entry.Entity.ModifiedDate = DateTime.UtcNow;
    //            }
    //            // For deleted entities, IsActive will be set to false elsewhere (soft delete)
    //        }
    //    }
    //}

    // AppDbContext is the main class for interacting with your database.
    public class AppDbContext : DbContext
    {
        private readonly string _databasePath;

        // This constructor is for runtime use by your application.
        // It accepts the database path, which will be provided by your application's startup code.
        public AppDbContext(string databasePath)
        {
            _databasePath = databasePath;
        }

        // This constructor is specifically for Entity Framework Core CLI tools (like Add-Migration).
        // It allows the tools to create an instance of your DbContext.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            // This constructor is used by the factory below.
        }

        // Define DbSet properties for your models, corresponding to database tables
        public DbSet<User> Users { get; set; }
        // public DbSet<Item> Items { get; set; }
        // public DbSet<Sale> Sales { get; set; }
        // public DbSet<Customer> Customers { get; set; }
        // etc.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Only configure with the path if it's set. This prevents the design-time factory
            // from overwriting the options it already provided.
            if (!string.IsNullOrEmpty(_databasePath))
            {
                var dbDirectory = Path.GetDirectoryName(_databasePath);
                if (dbDirectory != null && !Directory.Exists(dbDirectory))
                {
                    Directory.CreateDirectory(dbDirectory);
                    AppLogger.LogInfo($"Database directory created: {dbDirectory}");
                }
                optionsBuilder.UseSqlite($"Data Source={_databasePath}");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply configurations for your entities here
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.ModifiedDate = null;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedDate = DateTime.UtcNow;
                }
            }
        }
    }

    // This factory class explicitly tells the EF Core tools how to create your DbContext.
    // It resolves the ambiguity of having multiple constructors.
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // For migrations, we use a simple, hardcoded database path.
            // This database will only be used for the migration generation process.
            // Your application will use the runtime constructor with a dynamic path.
            optionsBuilder.UseSqlite("Data Source=design_time_migrations.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}

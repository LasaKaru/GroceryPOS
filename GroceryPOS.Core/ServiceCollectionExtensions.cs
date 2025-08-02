//using GroceryPOS.Core.Interfaces.Repositories;
//using GroceryPOS.Core.Interfaces.Services;
//using GroceryPOS.Core.Services;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using GroceryPOS.Data.Context; // For AppDbContext
//using GroceryPOS.Data.Repositories; // For UserRepository
//using GroceryPOS.Data.Repositories.Base; // For RepositoryBase
//using System;
using Microsoft.Extensions.DependencyInjection;
using GroceryPOS.Core.Interfaces.Repositories; // For IUserRepository
using GroceryPOS.Core.Interfaces.Services; // For IAuthService
using GroceryPOS.Core.Services; // For AuthService
using GroceryPOS.Core.Context; // For AppDbContext
using System;
using Microsoft.EntityFrameworkCore;
//using GroceryPOS.Data.Repositories;
//using GroceryPOS.Core.Repositories.Base;

namespace GroceryPOS.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all core services and data repositories with the DI container.
        /// </summary>
        /// <param name="services">The IServiceCollection instance.</param>
        /// <param name="databasePath">The full path to the SQLite database file.</param>
        /// <returns>The modified IServiceCollection instance.</returns>
        public static IServiceCollection AddCoreServices(this IServiceCollection services, string databasePath)
        {
            if (string.IsNullOrWhiteSpace(databasePath))
            {
                throw new ArgumentNullException(nameof(databasePath), "Database path cannot be null or empty for service registration.");
            }

            // Register DbContext with the provided database path
            // DbContexts are typically scoped to a request/operation
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={databasePath}"), ServiceLifetime.Scoped);

            // Register Repositories
            // Using Scoped lifetime ensures a single instance per request/scope
            //services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>)); // Generic repository
            //services.AddScoped<IUserRepository, UserRepository>(); // Specific user repository

            // Register Services
            // Using Scoped lifetime for services as well
            services.AddScoped<IAuthService, AuthService>();

            // Add other services here as they are created (e.g., IInventoryService, ISalesService)

            return services;
        }
    }
}

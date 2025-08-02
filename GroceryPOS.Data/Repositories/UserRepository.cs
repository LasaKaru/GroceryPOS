
using Microsoft.EntityFrameworkCore;
using GroceryPOS.Core.Interfaces.Repositories; // For IUserRepository
using GroceryPOS.Core.Models; // For User model
using GroceryPOS.Core.Context; // For AppDbContext (now in Core!)
using GroceryPOS.Data.Repositories.Base; // For RepositoryBase
using GroceryPOS.Utilities.Logging; // For AppLogger
using System;
using System.Threading.Tasks;
using System.Linq; // For .AsNoTracking() and .FirstOrDefaultAsync()

namespace GroceryPOS.Data.Repositories
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                AppLogger.LogWarning("Attempted to get user with null or empty username.");
                return null; // Or throw ArgumentNullException
            }

            try
            {
                // Use AsNoTracking() for read-only operations for performance,
                // unless you plan to modify the entity after retrieval.
                // Also filter by IsActive for soft-deleted users.
                return await _dbSet.AsNoTracking()
                                   .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Error retrieving user by username '{username}'.", ex);
                throw new ApplicationException($"An error occurred while retrieving user by username.", ex);
            }
        }
        // Add any other user-specific queries here if needed.
    }
}

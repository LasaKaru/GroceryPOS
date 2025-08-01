using GroceryPOS.Core.Interfaces.Repositories;
using GroceryPOS.Core.Models;
using GroceryPOS.Data.Context;
using GroceryPOS.Data.Repositories.Base;
using GroceryPOS.Utilities.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

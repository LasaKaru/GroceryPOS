using GroceryPOS.Core.Enums;
using GroceryPOS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryPOS.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateUserAsync(string username, string password);
        Task<bool> RegisterUserAsync(string username, string password, string firstName, string lastName, UserRole role);
        Task<bool> IsAdminUserCreatedAsync(); // Checks if at least one admin exists for initial setup
        // Add other auth-related methods like password reset, change password etc. later
    }
}

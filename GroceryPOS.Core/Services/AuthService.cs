using GroceryPOS.Core.Enums;
using GroceryPOS.Core.Interfaces.Repositories;
using GroceryPOS.Core.Interfaces.Services;
using GroceryPOS.Core.Models;
using GroceryPOS.Utilities.Encryption;
using GroceryPOS.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryPOS.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        // Constructor will receive IUserRepository via Dependency Injection
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<User?> AuthenticateUserAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                AppLogger.LogWarning("Authentication attempt with empty username or password.");
                return null;
            }

            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);

                if (user == null || !user.IsActive) // User not found or inactive
                {
                    AppLogger.LogInfo($"Authentication failed for username: {username}. User not found or inactive.");
                    return null;
                }

                // Verify password using the PasswordHasher utility
                if (PasswordHasher.VerifyPassword(password, user.PasswordHash))
                {
                    AppLogger.LogInfo($"Authentication successful for user: {username}");
                    return user;
                }
                else
                {
                    AppLogger.LogInfo($"Authentication failed for username: {username}. Invalid password.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"An error occurred during authentication for user '{username}'.", ex);
                // Re-throw as a more general application exception or handle as per policy
                throw new ApplicationException("Authentication service failed.", ex);
            }
        }

        public async Task<bool> RegisterUserAsync(string username, string password, string firstName, string lastName, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(firstName))
            {
                AppLogger.LogWarning("Registration attempt with incomplete user details.");
                return false;
            }

            try
            {
                // Check if username already exists
                var existingUser = await _userRepository.GetByUsernameAsync(username);
                if (existingUser != null)
                {
                    AppLogger.LogWarning($"Registration failed: Username '{username}' already exists.");
                    return false;
                }

                // Hash the password before saving
                string hashedPassword = PasswordHasher.HashPassword(password);

                var newUser = new User(username, hashedPassword, firstName, lastName, role);
                await _userRepository.AddAsync(newUser);
                await _userRepository.SaveChangesAsync(); // Commit the new user to DB

                AppLogger.LogInfo($"Successfully registered new user: {username} with role {role}");
                return true;
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"An error occurred during user registration for username '{username}'.", ex);
                throw new ApplicationException("User registration service failed.", ex);
            }
        }

        public async Task<bool> IsAdminUserCreatedAsync()
        {
            try
            {
                // Check if any user with Admin role exists and is active
                var adminUser = await _userRepository.GetAllAsync(); // This will get all active users
                foreach (var user in adminUser)
                {
                    if (user.Role == UserRole.Admin)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                AppLogger.LogError("An error occurred checking for admin user existence.", ex);
                throw new ApplicationException("Failed to check admin user status.", ex);
            }
        }
    }
}

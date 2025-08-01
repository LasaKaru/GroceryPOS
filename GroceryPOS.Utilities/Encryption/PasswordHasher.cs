using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace GroceryPOS.Utilities.Encryption
{
    public static class PasswordHasher
    {
        /// <summary>
        /// Hashes a plain-text password using BCrypt.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        /// <returns>The hashed password string.</returns>
        public static string HashPassword(string password)
        {
            // BCrypt automatically handles salting. Default work factor (12) is usually good.
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        /// <summary>
        /// Verifies a plain-text password against a hashed password.
        /// </summary>
        /// <param name="password">The plain-text password to check.</param>
        /// <param name="hashedPassword">The stored hashed password.</param>
        /// <returns>True if the password matches the hash, false otherwise.</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (BCrypt.Net.SaltParseException ex)
            {
                // This can happen if the hashedPassword is malformed or not a valid BCrypt hash.
                // Log the exception for debugging.
                // In a real app, you'd use a proper logging framework here.
                System.Diagnostics.Debug.WriteLine($"Error verifying password: {ex.Message}");
                return false;
            }
            catch (System.Exception ex)
            {
                // Catch any other unexpected exceptions during verification.
                System.Diagnostics.Debug.WriteLine($"An unexpected error occurred during password verification: {ex.Message}");
                return false;
            }
        }
    }
}

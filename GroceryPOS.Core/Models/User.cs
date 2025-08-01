using GroceryPOS.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryPOS.Core.Models
{
    public class User : EntityBase // Inherits common properties like Id, CreatedDate, etc.
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Will store hashed password
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UserRole Role { get; set; } // Admin or Employee
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Constructor for easy initialization (optional, but good practice)
        public User() { }

        public User(string username, string passwordHash, string firstName, string lastName, UserRole role)
        {
            Username = username;
            PasswordHash = passwordHash;
            FirstName = firstName;
            LastName = lastName;
            Role = role;
        }
    }
}

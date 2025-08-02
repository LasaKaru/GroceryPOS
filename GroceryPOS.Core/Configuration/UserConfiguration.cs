using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroceryPOS.Core.Models;

namespace GroceryPOS.Core.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users"); // Explicitly sets table name, though EF Core usually pluralizes by default

            builder.HasKey(u => u.Id); // Sets Id as primary key

            builder.Property(u => u.Username)
                   .IsRequired()     // Makes Username a required field
                   .HasMaxLength(50); // Sets maximum length for Username

            builder.HasIndex(u => u.Username) // Creates a unique index on Username
                   .IsUnique();

            builder.Property(u => u.PasswordHash)
                   .IsRequired()
                   .HasMaxLength(255); // A typical length for BCrypt hashes

            builder.Property(u => u.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.LastName)
                   .HasMaxLength(100);

            builder.Property(u => u.Role)
                   .IsRequired(); // Role enum will be stored as integer by default

            builder.Property(u => u.ContactNumber)
                   .HasMaxLength(20);

            builder.Property(u => u.Email)
                   .HasMaxLength(100);

            // Configure audit fields from EntityBase
            builder.Property(u => u.CreatedDate)
                   .IsRequired();

            builder.Property(u => u.ModifiedDate)
                   .IsRequired(false); // Can be null

            builder.Property(u => u.IsActive)
                   .IsRequired();

            // Seed initial data (for default Admin user setup) - OPTIONAL but useful
            // This data will be added to the database when the first migration is applied.
            // Password for 'admin' user will be '123' (hashed using PasswordHasher).
            // It's crucial to ensure PasswordHasher.HashPassword("123") is consistent.
            // For development, we'll hash it once and use the static string.
            // In a real scenario, you'd generate this securely.
            // Let's create a placeholder hash for now, we'll replace it with a real one later.
            // For now, just ensure the property exists.
            // We'll generate a proper hash for "123" when we do the initial setup code.
            /*
            builder.HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "$2a$12$R.S.g.F.f.g.W.X.Y.Z.0.1.2.3.4.5.6.7.8.9.ABcdef", // REPLACE THIS WITH ACTUAL HASH OF "123"
                    FirstName = "System",
                    LastName = "Admin",
                    Role = Core.Enums.UserRole.Admin,
                    ContactNumber = "",
                    Email = "",
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                }
            );
            */
        }
    }
}

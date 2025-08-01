using GroceryPOS.Core.Interfaces.Repositories;
using GroceryPOS.Core.Models;
using GroceryPOS.Data.Context;
using GroceryPOS.Utilities.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryPOS.Data.Repositories.Base
{
    public class RepositoryBase<T> : IRepository<T> where T : EntityBase
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet; // Represents the collection of all entities in the context, or that can be queried from the database.

        public RepositoryBase(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Ensure context is not null
            _dbSet = _context.Set<T>(); // Get the DbSet for the specific entity type T
        }

        public virtual async Task AddAsync(T entity)
        {
            if (entity == null)
            {
                AppLogger.LogWarning($"Attempted to add a null {typeof(T).Name} entity.");
                throw new ArgumentNullException(nameof(entity));
            }
            try
            {
                await _dbSet.AddAsync(entity);
                AppLogger.LogInfo($"Added new {typeof(T).Name} with ID: {entity.Id}");
            }
            catch (DbUpdateException ex)
            {
                AppLogger.LogError($"Database error when adding {typeof(T).Name}.", ex);
                throw new InvalidOperationException($"Could not add {typeof(T).Name}. A database error occurred.", ex);
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"An unexpected error occurred when adding {typeof(T).Name}.", ex);
                throw new ApplicationException($"An unexpected error occurred while adding {typeof(T).Name}.", ex);
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                AppLogger.LogWarning($"Attempted to delete {typeof(T).Name} with invalid ID: {id}.");
                throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero.");
            }

            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity == null)
                {
                    AppLogger.LogWarning($"{typeof(T).Name} with ID {id} not found for deletion.");
                    // Optionally, throw an exception if not found is an error for your business logic
                    return; // Or throw new KeyNotFoundException($"Entity with ID {id} not found.");
                }

                // For soft deletion:
                entity.IsActive = false;
                _dbSet.Update(entity); // Mark as modified
                AppLogger.LogInfo($"Soft-deleted {typeof(T).Name} with ID: {id}");

                // For hard deletion (uncomment if you need permanent removal, use with caution):
                // _dbSet.Remove(entity);
                // AppLogger.LogInfo($"Hard-deleted {typeof(T).Name} with ID: {id}");
            }
            catch (DbUpdateException ex)
            {
                AppLogger.LogError($"Database error when deleting {typeof(T).Name} with ID {id}.", ex);
                throw new InvalidOperationException($"Could not delete {typeof(T).Name} with ID {id}. A database error occurred.", ex);
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"An unexpected error occurred when deleting {typeof(T).Name} with ID {id}.", ex);
                throw new ApplicationException($"An unexpected error occurred while deleting {typeof(T).Name} with ID {id}.", ex);
            }
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet.Where(e => e.IsActive).ToListAsync(); // Only return active entities
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Error retrieving all {typeof(T).Name} entities.", ex);
                throw new ApplicationException($"An error occurred while retrieving all {typeof(T).Name} entities.", ex);
            }
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                AppLogger.LogWarning($"Attempted to get {typeof(T).Name} with invalid ID: {id}.");
                return null; // Or throw ArgumentOutOfRangeException
            }
            try
            {
                // FindAsync efficiently looks in local cache first, then database
                var entity = await _dbSet.FindAsync(id);
                // Only return if active (if you want soft delete behavior to hide inactive)
                return entity?.IsActive == true ? entity : null;
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"Error retrieving {typeof(T).Name} by ID: {id}.", ex);
                throw new ApplicationException($"An error occurred while retrieving {typeof(T).Name} by ID: {id}.", ex);
            }
        }

        public virtual async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                AppLogger.LogWarning($"Attempted to update a null {typeof(T).Name} entity.");
                throw new ArgumentNullException(nameof(entity));
            }
            // Check if the entity is already being tracked by the context
            var trackedEntity = _context.Entry(entity);
            if (trackedEntity.State == EntityState.Detached)
            {
                // If detached, attach it and mark as modified
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                // If already tracked, EF Core will detect changes on SaveChanges
                _dbSet.Update(entity);
            }

            try
            {
                AppLogger.LogInfo($"Updated {typeof(T).Name} with ID: {entity.Id}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                AppLogger.LogError($"Concurrency error when updating {typeof(T).Name} with ID {entity.Id}. It may have been modified or deleted by another user/process.", ex);
                throw new InvalidOperationException($"Concurrency conflict: {typeof(T).Name} with ID {entity.Id} was modified or deleted.", ex);
            }
            catch (DbUpdateException ex)
            {
                AppLogger.LogError($"Database error when updating {typeof(T).Name} with ID {entity.Id}.", ex);
                throw new InvalidOperationException($"Could not update {typeof(T).Name} with ID {entity.Id}. A database error occurred.", ex);
            }
            catch (Exception ex)
            {
                AppLogger.LogError($"An unexpected error occurred when updating {typeof(T).Name} with ID {entity.Id}.", ex);
                throw new ApplicationException($"An unexpected error occurred while updating {typeof(T).Name} with ID {entity.Id}.", ex);
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                AppLogger.LogInfo("Database changes saved successfully.");
            }
            catch (DbUpdateException ex)
            {
                AppLogger.LogError("Database save changes error.", ex);
                throw new InvalidOperationException("Could not save changes to the database. A database error occurred.", ex);
            }
            catch (Exception ex)
            {
                AppLogger.LogError("An unexpected error occurred while saving changes.", ex);
                throw new ApplicationException("An unexpected error occurred while saving changes to the database.", ex);
            }
        }
    }
}

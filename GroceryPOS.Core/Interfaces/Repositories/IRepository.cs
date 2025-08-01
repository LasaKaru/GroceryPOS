using GroceryPOS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryPOS.Core.Interfaces.Repositories
{
    public interface IRepository<T> where T : EntityBase
    {
        Task<T?> GetByIdAsync(int id); // Nullable T? for when an entity isn't found
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync(); // For committing changes to the database
    }
}

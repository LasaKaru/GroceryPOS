using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroceryPOS.Core.Models
{
    public abstract class EntityBase
    {
        public int Id { get; set; } // Primary Key for all entities
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Record creation date in UTC
        public DateTime? ModifiedDate { get; set; } // Record last modification date
        public bool IsActive { get; set; } = true; // For soft deletion or active/inactive status
    }
}

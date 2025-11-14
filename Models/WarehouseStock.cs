using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class WarehouseStock
    {
        
        public int WarehouseStockId { get; set; } // Primary key

        
        public int WarehouseId { get; set; }
        public virtual Warehouse Warehouse { get; set; }

      
        public int FridgeId { get; set; } // Links to Fridge (model)
        public virtual Fridge Fridge { get; set; }

        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; } // Stock of this fridge model in this warehouse

    }
}

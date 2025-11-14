using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class Warehouse
    {
        [Key]

        public int WarehouseId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public int PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }

       // public virtual ICollection<Fridge> Fridge { get; set; } = new List<Fridge>();

        public virtual ICollection<Employee> Employees { get; set; }  = new List<Employee>();
        public virtual ICollection<FaultTechnician> FaultTechnicians { get; set; } = new List<FaultTechnician>();
        public virtual ICollection<WarehouseStock> WarehouseStocks { get; set; } = new List<WarehouseStock>();
        public virtual ICollection<Fridge> Fridges { get; set; } = new List<Fridge>();
        public virtual ICollection<CustomerFridge> CustomerFridges { get; set; } = new List<CustomerFridge>();

    }
}

using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class FridgeStatusHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public DateTime LastUpdate { get; set; }
        [Required]
        [MaxLength(300)]
        public string Note { get; set; }

        public int EventId { get; set; }
        public EventType EventType { get; set; }

        public int InventoryId { get; set; }
        public FridgeInventory FridgeInventory { get; set; }

        public int LocationId { get; set; }
        public CustomerLocation Location { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}

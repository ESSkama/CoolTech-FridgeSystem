using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class ServiceChecklist
    {
        [Key]
        public int ChecklistId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        [Required]
        [MaxLength(100)]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }


        //fk to  who created the checklist 
        public int CreatedById { get; set; }
        public Employee CreatedBy { get; set; }
        //

        public ICollection<ServiceCheckItem> Items { get; set; } = new List<ServiceCheckItem>();


    }
}

using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class ServiceCheckItem
    {
        [Key]
        public int ItemId { get; set; }
        [Required]

        public string StepDescription { get; set; }
        [Required]
        public int SequenceOrder { get; set; }


        //fk to service checklist
        public int ChecklistId { get; set; }
        public ServiceChecklist ServiceChecklist { get; set; }

        public ICollection<ServiceCheckResult> ServiceCheckResults { get; set; } = new List<ServiceCheckResult>();
    }
}

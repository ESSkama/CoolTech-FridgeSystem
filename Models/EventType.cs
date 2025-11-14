using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class EventType
    {
        [Key]
        public int EventId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Event { get; set; }


        public ICollection<ServiceSchedule> ServiceSchedules { get; set; } = new List<ServiceSchedule>();
        public ICollection<FridgeStatusHistory> FridgeStatusHistory { get; set; } = new List<FridgeStatusHistory>();

    }
}

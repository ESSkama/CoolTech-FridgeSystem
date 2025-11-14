using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class ServiceCheckResult
    {
        [Key]
        public int ResultId { get; set; }
        [Required]
        [MaxLength(300)]
        public string Notes { get; set; }
        [Required]
        public DateTime CheckedDate { get; set; }

        //fk to service schedule
        public int ScheduleId { get; set; }
        public ServiceSchedule ServiceSchedule { get; set; }
        //fk to checklist items
        public int ItemId { get; set; }
        public ServiceCheckItem Item { get; set; }
        //fk to satus table
        public RequestStatus RequestStatus { get; set; }
        //fk to employee who did the check
        public int CheckedById { get; set; }
        public Employee CheckedBy { get; set; }


    }
}

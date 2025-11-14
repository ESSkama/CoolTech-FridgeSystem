using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class CustomerNotification
    {
        [Key]
        public int NotificationId { get; set; }
        [Required]
        [MaxLength(150)]
        public string Message { get; set; }
        [Required]
        public DateTime SentDate { get; set; }

        //FK to cust
        public int BusinessId { get; set; }
        public Customer Customer { get; set; }
        //fk to fault report
        public int? FaultId { get; set; }
        public FaultReport? FaultReport { get; set; }
        //fk to service schedule
        public int? ScheduleId { get; set; }
        public ServiceSchedule? Schedule { get; set; }

        public bool IsRead { get; set; } = false;

    }
}

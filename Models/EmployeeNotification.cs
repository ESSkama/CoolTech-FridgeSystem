using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class EmployeeNotification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime SentDate { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}


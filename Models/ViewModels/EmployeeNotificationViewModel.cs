using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models.ViewModels
{
    public class EmployeeNotificationViewModel
    {
        public int NotificationId { get; set; }

        [Display(Name = "Message")]
        public string? Message { get; set; }

        [Display(Name = "Date Sent")]
        public DateTime SentDate { get; set; }

        [Display(Name = "Read")]
        public bool IsRead { get; set; }
    }
}

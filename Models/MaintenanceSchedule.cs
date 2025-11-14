using System;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class MaintenanceSchedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fridge ID")]
        public string FridgeId { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        [Display(Name = "Schedule Date")]
        public DateTime ScheduleDate { get; set; }

        [Required]
        [Display(Name = "Time Window")]
        public string TimeWindow { get; set; }

        [Required]
        [Display(Name = "Maintenance Type")]
        public string MaintenanceType { get; set; }

        [Required]
        [Display(Name = "Assigned Technician")]
        public string Technician { get; set; }
    }
}
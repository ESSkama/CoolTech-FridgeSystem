
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class ServiceAssignment
    {
        [Key]
        public int AssignmentId { get; set; }
        public DateTime AssignedDate { get; set; }


        public int FaultId { get; set; }
        public FaultReport FaultReport { get; set; }
        //fk to service schedule table
        public int ScheduleId { get; set; }
        public ServiceSchedule Schedule { get; set; }
      
        //fk to admin who assigns the service to technicians
        public int AssignedById { get; set; }
        public Employee AssignedBy { get; set; }
        //fk to tech who is assigned the work
        public int EmployeeId { get; set; }
        public Employee Technician { get; set; }


    }
}

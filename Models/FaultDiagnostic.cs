using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models
{
    public class FaultDiagnostic
    {
        [Key]
        public int DiagnosisId { get; set; }
        [Required]
        public DateTime DiagnosisDate { get; set; }
        [Required]
        [MaxLength(300)]
        public string Description { get; set; }

        //fk to fault report
        public int FaultId { get; set; }
        public FaultReport FaultReport { get; set; }

        //fk to tech who performed diagnosis
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        //fk to status table which will be used to show the next step, eg, repairs, scrapping etc.
        public RequestStatus RequestStatus { get; set; }
        //
        public int ScheduleId { get; set; }
        public ServiceSchedule Schedule { get; set; }



    }
}

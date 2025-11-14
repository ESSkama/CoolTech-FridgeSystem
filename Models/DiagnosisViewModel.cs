namespace FridgeSystem.Models
{
    public class DiagnosisViewModel
    {

        public int FaultId { get; set; }
        public int ChecklistId { get; set; }

        public List<DiagnosisItemViewModel> Items { get; set; }
    }
}

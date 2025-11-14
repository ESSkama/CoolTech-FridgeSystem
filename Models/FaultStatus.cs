namespace FridgeSystem.Models
{
    public enum FaultStatus
    {
        Logged,    
        Assigned,// Logged but not yet seen by technician
        UnderDiagnosis,   // Technician has viewed it
        Scheduled,        // Repair has been scheduled
        Completed,
        Pending
    }
}

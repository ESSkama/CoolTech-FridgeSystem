namespace FridgeSystem.Models.ViewModels
{
    public class CustLiaisonDashboardViewModel
    {
        public string EmployeeName { get; set; } = string.Empty;

        // Example summary counts
        public int TotalCustomers { get; set; }
        public int ActiveAllocations { get; set; }
        public int PendingRequests { get; set; }
        public int UnreadNotifications { get; set; }

        // Optional: list of recent allocations
        public List<string> RecentAllocations { get; set; } = new List<string>();
        // Recent notifications for EmployeeNotification Section
        public List<EmployeeNotificationViewModel> RecentNotifications { get; set; } = new List<EmployeeNotificationViewModel>();
    }
}

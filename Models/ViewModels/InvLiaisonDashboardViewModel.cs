namespace FridgeSystem.Models.ViewModels
{
    public class InvLiaisonDashboardViewModel
    {
        public string EmployeeName { get; set; } = string.Empty;

        public int TotalFridgesInInventory { get; set; }
        public int LowStockAlerts { get; set; }
        public int PendingPurchaseRequests { get; set; }
        public int UnreadNotifications { get; set; }

        public List<string> RecentFridgeAdditions { get; set; } = new List<string>();
        public List<EmployeeNotificationViewModel> RecentNotifications { get; set; } = new List<EmployeeNotificationViewModel>();
        public List<StockAlertViewModel> LowStockAlertsList { get; set; } = new List<StockAlertViewModel>();

    }
}

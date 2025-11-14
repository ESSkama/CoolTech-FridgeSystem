namespace FridgeSystem.Models.ViewModels
{
    
    
        public class FridgeHistoryViewModel
        {
            // Basic fridge info
            public int FridgeId { get; set; }

            // Fridge name or model reference
            public string? FridgeName { get; set; }

            public string Model { get; set; }
            public string? SerialNumber { get; set; }

            // Category info (from FridgeCategory)
            public string? Category { get; set; }

            public string? FridgeColor { get; set; }

            // Current status (computed: Available, Unavailable, Scrapped)
            public string Status { get; set; }

            // Which warehouse this fridge is stored in
            public string? WarehouseName { get; set; }

            // Optional: inventory model info
            public string? InventoryModel { get; set; }

            // Status change history
            public List<FridgeStatusHistoryViewModel> StatusHistory { get; set; } = new();

            // Fault reports (if applicable)
            public List<FaultReportViewModel> FaultReports { get; set; } = new();

            // Maintenance / service history
            public List<ServiceScheduleViewModel> MaintenanceHistory { get; set; } = new();
        }

        public class FridgeStatusHistoryViewModel
        {
            public DateTime LastUpdate { get; set; }

            public string Note { get; set; }

            // e.g., Allocated, Returned, Scrapped, etc.
            public string EventType { get; set; }

            // Branch or customer location name
            public string? Location { get; set; }

            // Which employee performed this action
            public string? EmployeeName { get; set; }
        }

        public class FaultReportViewModel
        {
            public DateTime ReportedDate { get; set; }

            public string Title { get; set; }

            public string Description { get; set; }

            public string PriorityLevel { get; set; }

            public string Status { get; set; } // e.g., Open, InProgress, Closed

            public string ReportedBy { get; set; } // Employee name
        }

        public class ServiceScheduleViewModel
        {
            public DateTime ScheduledDate { get; set; }
            public DateTime ScheduledTime { get; set; }

            public string ServiceType { get; set; }

            public string TechnicianName { get; set; }

            public string Status { get; set; } // e.g., Pending, Completed

            public string? Notes { get; set; } // Optional notes by technician
        }
    

}

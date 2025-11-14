namespace FridgeSystem.Models
{
    public class ManageCustomersPageViewModel
    {
        // This is the collection of individual customers to display
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();

        // Properties for search and filter (these are inputs/state for the page)
        public string? SearchText { get; set; }
        public string? StatusFilter { get; set; }

        // Properties for pagination (even if not fully wired up yet, they belong here)
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 1;
        public int TotalCustomers { get; set; } = 0;
    }
}

using Microsoft.AspNetCore.Identity;

namespace FridgeSystem.Models
{
    public class UserViewModel
    {
        public IEnumerable<EmployeeViewModel>? Employees { get; set; }
        public IEnumerable<CustomerViewModel>? Customers { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; } = null!;
        public IEnumerable<IdentityRole> Roles { get; set; } = null!;
    }
}

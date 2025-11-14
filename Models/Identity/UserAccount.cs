using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models.Identity
{
    public class UserAccount : IdentityUser
    {
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsActive { get; set; } = true;

        // Either internal employee OR business account
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int? BusinessId { get; set; }
        public Customer Business { get; set; }

        // Link to domain role
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace FridgeSystem.Models.Identity
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; }

        // Navigation collections
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
        public ICollection<ServiceAssignment> ServiceAssignments { get; set; } = new List<ServiceAssignment>();
    }
}

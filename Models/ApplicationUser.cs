using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Track when the employee was added/hired
        public DateTime? HiredDate { get; set; } = DateTime.UtcNow;

        // Soft delete or activation flag
        public bool IsActive { get; set; } = true;

       // Link to Technician if this user is a technician
        public virtual FaultTechnician? Technician { get; set; }
       public ICollection<FridgeRequest> FridgeRequests { get; set; } = new List<FridgeRequest>();

        [NotMapped]
        public IList<string> RoleNames { get; set; } = new List<string>();
        public string? BusinessName { get; internal set; }
       
    }
}

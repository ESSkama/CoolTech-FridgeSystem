using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FridgeSystem.Models
{
    public class Customer
    {
        [Key]
        public int BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string BillingAddress { get; set; }

        public string ContactPersonName { get; set; } // Person at the business who made the account

        public int BusinessTypeId { get; set; }

        [ForeignKey(nameof(BusinessTypeId))]
        public virtual BusinessType? BusinessType { get; set; }
        public Boolean ProfileActive { get; set; }
       
        public DateTime DateCreated { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public bool isDeleted { get; set; } = false;

        public ICollection<FaultReport> FaultReports { get; set; } = new List<FaultReport>();
        public ICollection<CustomerNotification> Notifications { get; set; } = new List<CustomerNotification>();
        public ICollection<CustomerFridge> CustomerFridges { get; set; } = new List<CustomerFridge>();


        //Customers are tracked via customer locations
        public virtual ICollection<CustomerLocation> CustomerLocations { get; set; } 
        //public virtual ICollection<CustomerFridge> CustomerFridges { get; set; } // To track fridges owned by customer
        public virtual ICollection<FridgeRequest> FridgeRequests { get; set; }
        public string ApplicationUserId { get; set; }  // FK -> AspNetUsers.Id
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

    }
}

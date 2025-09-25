using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Stripe_Integration.Models
{
    public class ServiceDetails
    {
        [Key]
        public int ServiceDetailID { get; set; }

        [Required]
        public int ServiceMainID { get; set; }

        [ForeignKey(nameof(ServiceMainID))]
        public virtual ServiceMain ServicesMain { get; set; }

        [Required, MaxLength(250)]
        public string DetailItemDescription { get; set; }

        [Required, MaxLength(10)]
        public string ServiceTypeID { get; set; }

        [ForeignKey(nameof(ServiceTypeID))]
        public virtual ServiceType ServiceType { get; set; }

        public int? MonthlyCount { get; set; }

        public bool UserDisplay { get; set; } = true;

        [MaxLength(250)]
        public string? ServiceCodedDetails { get; set; }

        public bool IsSubscription { get; set; }

        // Navigation
        public virtual ICollection<UserServiceUsage> UserServiceUsages { get; set; }
    }
}

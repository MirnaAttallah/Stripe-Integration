using System.ComponentModel.DataAnnotations;

namespace Stripe_Integration.Models
{
    public class ServiceType
    {
        [Key, MaxLength(10)]
        public string ServiceTypeID { get; set; }

        [Required, MaxLength(100)]
        public string Description { get; set; }

        // Navigation
        public virtual ICollection<ServiceDetails> ServicesDetails { get; set; }
    }
}

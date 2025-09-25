using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Stripe_Integration.Models
{
    public class ServiceMain
    {
        [Key]
        public int ServiceMainID { get; set;}


        [Required, MaxLength(200)]
        public string ShortDescription { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required, MaxLength(50)]
        public string ServiceType { get; set; }  // Main, Add-On, Special, etc.
        [Column(TypeName = "decimal(10,2)")]
        public decimal? AnnualPrice { get; set; }

        // Navigation
        public virtual ICollection<ServiceDetails> ServiceDetails { get; set; }
        public virtual ICollection<UserPlan> UserPlans { get; set; }
    }
}

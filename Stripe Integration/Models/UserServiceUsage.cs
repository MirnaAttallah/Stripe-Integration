using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Stripe_Integration.Models
{
    public class UserServiceUsage
    {
        [Key]
        public int UsageID { get; set; }

        [Required]
        public int UserPlanID { get; set; }

        [ForeignKey(nameof(UserPlanID))]
        public virtual UserPlan UserPlan { get; set; }

        [Required]
        public int ServiceDetailID { get; set; }

        [ForeignKey(nameof(ServiceDetailID))]
        public virtual ServiceDetails ServicesDetail { get; set; }

        public DateTime AddedDateTime { get; set; } = DateTime.UtcNow;

        public DateTime? LastUsageDate { get; set; }

        public int RemainingBalance { get; set; } = 0;
    }
}

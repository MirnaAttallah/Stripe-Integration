using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Stripe_Integration.Models
{
    public class UserPlan
    {
        [Key]
        public int UserPlanID { get; set; }

        [Required, MaxLength(150)]
        public string B2CSubID { get; set; }   // Azure AD B2C user ID
        [ForeignKey(nameof(B2CSubID))]
        public virtual User User {  get; set; }

        [Required]
        public int ServiceMainID { get; set; }

        [ForeignKey(nameof(ServiceMainID))]
        public virtual ServiceMain ServicesMain { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(10,2)")]
        public decimal TransactionAmount { get; set; }

        [Required, MaxLength(50)]
        public string Frequency { get; set; }   // Monthly, Once, Yearly

        public DateTime NextRenewalDate { get; set; }

        public bool Active { get; set; }

        // Navigation
        public virtual ICollection<UserServiceUsage> UserServiceUsages { get; set; }
    }
}

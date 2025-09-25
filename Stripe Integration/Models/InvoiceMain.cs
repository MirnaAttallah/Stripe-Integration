using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stripe_Integration.Models
{
    public class InvoiceMain
    {
        [Key]
        public int InvoiceID { get; set; }
        [StringLength(150)]
        public string B2CSubID { get; set; }
        public DateTime? PurchaseDate { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalTransactionAmount { get; set; }
        public decimal? TaxAmount { get; set; }
        [StringLength(100)]
        public string? StripeSubscriptionID { get; set; }
        [StringLength(100)]
        public string? StripeCustomerEmail { get; set; }
        [StringLength(100)]
        public string? StripeCustomerID { get; set; }
        [StringLength(100)]
        public string? StripePaymentIntentID { get; set; }
        [StringLength(100)]
        public string? StripeInvoiceID { get; set; }
        [StringLength(50)]
        public string? StripeStatus { get; set; }

        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new HashSet<InvoiceDetail>();
    }
}

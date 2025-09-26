using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stripe_Integration.Models
{
    public class InvoiceDetail
    {
        [Key]
        public int InvoiceDetailID { get; set; }
        public int InvoiceID { get; set; }
        public int ServiceMainID { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitAmount { get; set; }
        [MaxLength(50)]
        public string Frequency { get; set; } // monthly, quarterly, yearly, etc.
        //Navigation Properties
        [ForeignKey(nameof(ServiceMainID))]
        public virtual ServiceMain ServiceMain { get; set; }
        [ForeignKey(nameof(InvoiceID))]
        public virtual InvoiceMain InvoiceMain { get; set; }
    }
}

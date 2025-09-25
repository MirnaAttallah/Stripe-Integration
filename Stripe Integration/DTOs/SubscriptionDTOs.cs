namespace Stripe_Integration.DTOs
{
    public class CreateSubscriptionInvoice
    {
        public string B2cSubId { get; set; } = "";
        public int PlanId { get; set; }
        public string Interval { get; set; } = "month";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
    }
    
    public class CreateSubscriptionRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public int PlanId { get; set; }
        public string Interval { get; set; } = "month";
        public int InvoiceId { get; set; }
        public string B2cSubId { get; set; } = "";
    }

}

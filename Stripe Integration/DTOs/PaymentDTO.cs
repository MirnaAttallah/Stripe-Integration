namespace Stripe_Integration.DTOs
{
    public class CreatePaymentInvoice
    {
        public string B2cSubId { get; set; } = "";
        public List<CartItem> Items { get; set; }
        public string Currency { get; set; } = "usd";
    }

    public class CreatePaymentRequest
    {
        //public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public int InvoiceId { get; set; }
        public string B2cSubId { get; set; } = "";
    }
}

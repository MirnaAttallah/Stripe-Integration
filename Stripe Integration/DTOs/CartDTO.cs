namespace Stripe_Integration.DTOs
{
    public class CartDTO
    {
        public int? CartId { get; set; } = null;
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public string Currency { get; set; } = "usd";
        public string B2cSubId { get; set; } = "";
        public DateTime CreatedIn { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount
        {
            get
            {
                return Items.Sum(item => item.UnitAmount * item.Quantity);
            }
        }
    }
    public class CartItem
    {
        public required string ServiceName { get; set; }
        public int ServiceMainId { get; set; }
        public decimal UnitAmount { get; set; }
        public int Quantity { get; set; }
    }
}

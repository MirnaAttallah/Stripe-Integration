namespace Stripe_Integration.DTOs
{
	public class ViewCartDTO
	{
		public List<ViewCartItemDTO> Items { get; set; } = new List<ViewCartItemDTO>();
		public string Currency { get; set; } = "usd";
		public string B2cSubId { get; set; } = "";
		public DateTime CreatedIn { get; set; } = DateTime.UtcNow;

		public decimal TotalAmount
		{
			get => Items.Sum(item => item.UnitAmount * item.Quantity)
		}
	}

	public class ViewCartItemDTO
	{
		public int ServiceName { get; set; }
		public decimal UnitAmount { get; set; }
		public int Quantity { get; set; }
	}
}
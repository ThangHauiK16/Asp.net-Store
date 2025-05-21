namespace Shopping_Tutorial.Models
{
	public class ProductUsageNeed
	{
		public long ProductId { get; set; }
		public int UsageNeedId { get; set; }

		public ProductModel Product { get; set; }
		public UsageNeedModel UsageNeed { get; set; }
	}
}

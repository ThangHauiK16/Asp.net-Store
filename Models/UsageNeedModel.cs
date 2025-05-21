using System.ComponentModel.DataAnnotations;

namespace Shopping_Tutorial.Models
{
	public class UsageNeedModel
	{
		[Key]
		public int Id { get; set; }
		public string Name { get; set; }
		public ICollection<ProductUsageNeed> ProductUsageNeeds { get; set; }
	}
}

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Shopping_Tutorial.Models.ViewModels
{
	public class CompareViewModel
	{
		public long? ProductId1 { get; set; } = 0;
		public long? ProductId2 { get; set; } = 0;
		public string UsageNeed { get; set; } = string.Empty;
		public List<SelectListItem> Products { get; set; } = new();
		public string Result { get; set; }
	}
}

using System.ComponentModel.DataAnnotations;

namespace Shopping_Tutorial.Models
{
	public class ThongSoKyThuatModel
	{
		
			[Key]
			public int Id { get; set; }

			[Required]
			public long ProductId { get; set; }

			// Thông tin kỹ thuật
			public string? Camera { get; set; }
			public string? CPU { get; set; }
			public string? RAM { get; set; }
			public string? Chip { get; set; }
			public string? Pin { get; set; }
			public string? Screen { get; set; }

			// Navigation
			public ProductModel? Product { get; set; }
		}
	
}

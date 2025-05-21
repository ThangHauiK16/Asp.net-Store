using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Models.ViewModels;
using Shopping_Tutorial.Repository;
using System.Diagnostics;
using System.Globalization;

namespace Shopping_Tutorial.Controllers
{
	public class HomeController : Controller
	{
		private readonly DataContext _dataContext;
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<AppUserModel> _userManager;

		public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<AppUserModel> userManager)
		{
			_logger = logger;
			_dataContext = context;
			_userManager = userManager;

		}

		//public IActionResult Index()
		//{

		//	var products = _dataContext.Products.Include("Category").Include("Brand").ToList();

		//	var sliders = _dataContext.Sliders.Where(s => s.Status == 1).ToList();
		//	ViewBag.Sliders = sliders;

		//	return View(products);
		//}
		public async Task<IActionResult> Index(int page = 1)
		{
			const int pageSize = 6; // Số sản phẩm mỗi trang

			var productsQuery = _dataContext.Products
				.Include(p => p.Category)
				.Include(p => p.Brand)
				.OrderByDescending(p => p.Id);

			var totalProducts = await productsQuery.CountAsync();

			var products = await productsQuery
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			var sliders = await _dataContext.Sliders
				.Where(s => s.Status == 1)
				.ToListAsync();

			ViewBag.Sliders = sliders;
			ViewBag.CurrentPage = page;
			ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);

			return View(products);
		}

		//public async Task<IActionResult> Compare()
		//{
		//	var compare_product = await (from c in _dataContext.Compares
		//								 join p in _dataContext.Products on c.ProductId equals p.Id
		//								 join u in _dataContext.Users on c.UserId equals u.Id
		//								 select new { User = u, Product = p, Compares = c })
		//					   .ToListAsync();

		//	return View(compare_product);
		//}
		//public async Task<IActionResult> DeleteCompare(int Id)
		//{
		//	CompareModel compare = await _dataContext.Compares.FindAsync(Id);

		//	_dataContext.Compares.Remove(compare);

		//	await _dataContext.SaveChangesAsync();
		//	TempData["success"] = "So sánh đã được xóa thành công";
		//	return RedirectToAction("Compare", "Home");
		//}
		//public async Task<IActionResult> AddCompare(long Id)
		//{
		//	var user = await _userManager.GetUserAsync(User);

		//	var compareProduct = new CompareModel
		//	{
		//		ProductId = Id,
		//		UserId = user.Id
		//	};

		//	_dataContext.Compares.Add(compareProduct);
		//	try
		//	{
		//		await _dataContext.SaveChangesAsync();
		//		return Ok(new { success = true, message = "Add to compare Successfully" });
		//	}
		//	catch (Exception)
		//	{
		//		return StatusCode(500, "An error occurred while adding to compare table.");
		//	}

		//}
		public async Task<IActionResult> DeleteWishlist(int Id)
		{
			WishlistModel wishlist = await _dataContext.Wishlists.FindAsync(Id);

			_dataContext.Wishlists.Remove(wishlist);

			await _dataContext.SaveChangesAsync();
			TempData["success"] = "Yêu thích đã được xóa thành công";
			return RedirectToAction("Wishlist", "Home");
		}
		public async Task<IActionResult> Wishlist()
		{
			var wishlist_product = await (from w in _dataContext.Wishlists
										  join p in _dataContext.Products on w.ProductId equals p.Id
										  select new { Product = p, Wishlists = w })
							   .ToListAsync();

			return View(wishlist_product);
		}

		[HttpPost]
		public async Task<IActionResult> AddWishlist(long Id, WishlistModel wishlistmodel)
		{
			var user = await _userManager.GetUserAsync(User);

			var wishlistProduct = new WishlistModel
			{
				ProductId = Id,
				UserId = user.Id
			};

			_dataContext.Wishlists.Add(wishlistProduct);
			try
			{
				await _dataContext.SaveChangesAsync();
				return Ok(new { success = true, message = "Add to wishlisht Successfully" });
			}
			catch (Exception)
			{
				return StatusCode(500, "An error occurred while adding to wishlist table.");
			}

		}
		[HttpPost]
		
		public IActionResult Privacy()
		{
			return View();
		}
		public async Task<IActionResult> Contact()
		{
			var contact = await _dataContext.Contact.FirstAsync();
			return View(contact);
		}
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error(int statuscode)
		{
			if (statuscode == 404)
			{
				return View("NotFound");
			}
			else
			{

			}
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
        [HttpGet]
        public IActionResult Compare(long? productId1, string productName1, long? productId2, string productName2)
        {
            var model = new CompareViewModel();
            model.Products = _dataContext.Products
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name })
                .ToList();
            model.Products.Insert(0, new SelectListItem { Text = "-- Chọn sản phẩm --", Value = "" });

            if (productId1.HasValue)
            {
                model.ProductId1 = productId1.Value;
                ViewBag.ProductName1 = productName1;
            }

            if (productId2.HasValue)
            {
                model.ProductId2 = productId2.Value;
                ViewBag.ProductName2 = productName2;

                if (productId1.HasValue)
                {
                    var product1 = _dataContext.Products
                        .Include(p => p.ThongSoKyThuat)
                        .Include(p => p.ProductUsageNeeds)
                            .ThenInclude(pun => pun.UsageNeed)
                        .FirstOrDefault(p => p.Id == model.ProductId1);

                    var product2 = _dataContext.Products
                        .Include(p => p.ThongSoKyThuat)
                        .Include(p => p.ProductUsageNeeds)
                            .ThenInclude(pun => pun.UsageNeed)
                        .FirstOrDefault(p => p.Id == model.ProductId2);

                    if (product1 == null || product2 == null)
                    {
                        model.Result = "Không tìm thấy sản phẩm.";
                    }
                    else if (product1.CategoryId != product2.CategoryId)
                    {
                        model.Result = "Hai sản phẩm không cùng loại (khác Category), không thể so sánh!";
                    }
                    else
                    {
                        model.Result = "Bạn đã chọn 2 sản phẩm để so sánh";
                        ViewBag.Product1 = product1;
                        ViewBag.Product2 = product2;
                        string csvFilePath = GenerateCsvAndSendToPython(product1, product2);
                        ReadCompareResultsJson();
                    }
                }
            }

            // Set selected values for the dropdowns
            foreach (var item in model.Products)
            {
                if (productId1.HasValue && item.Value == productId1.ToString())
                {
                    item.Selected = true;
                }
                if (productId2.HasValue && item.Value == productId2.ToString())
                {
                    item.Selected = true;
                }
            }

            return View(model);
        }
        [HttpPost]
		public IActionResult Compare(CompareViewModel model)
		{
			var product1 = _dataContext.Products
				.Include(p => p.ThongSoKyThuat)
				.Include(p => p.ProductUsageNeeds)
					.ThenInclude(pun => pun.UsageNeed)
				.FirstOrDefault(p => p.Id == model.ProductId1);

			var product2 = _dataContext.Products
				.Include(p => p.ThongSoKyThuat)
				.Include(p => p.ProductUsageNeeds)
					.ThenInclude(pun => pun.UsageNeed)
				.FirstOrDefault(p => p.Id == model.ProductId2);

			if (product1 == null || product2 == null)
			{
				model.Result = "Không tìm thấy sản phẩm.";
			}
			else if (product1.CategoryId != product2.CategoryId)
			{
				model.Result = "Hai sản phẩm không cùng loại (khác Category), không thể so sánh!";
			}
			else
			{
				model.Result = "Bạn đã thêm 2 sản phẩm thành công";
				ViewBag.Product1 = product1;
				ViewBag.Product2 = product2;
				string csvFilePath = GenerateCsvAndSendToPython(product1, product2);
				ReadCompareResultsJson();
			}

			model.Products = _dataContext.Products.Select(p => new SelectListItem
			{
				Value = p.Id.ToString(),
				Text = p.Name,
				Selected = (p.Id == model.ProductId1 || p.Id == model.ProductId2)
			}).ToList();
			model.Products.Insert(0, new SelectListItem { Text = "-- Chọn sản phẩm --", Value = "" });

			return View(model);
		}

		private string GenerateCsvAndSendToPython(ProductModel product1, ProductModel product2)
		{
			string timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
			Random rand = new Random();
			string csvFilePath = $"C:/Users/ACER/PycharmProjects/ProjectPython/product_data_{timeStamp}_{rand.Next(1000, 9999)}.csv";

			using (var writer = new StreamWriter(csvFilePath))
			using (var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteField("ProductName");
				csv.WriteField("RAM");
				csv.WriteField("CPU");
				csv.WriteField("Camera");
				csv.WriteField("Price");
				csv.WriteField("Battery");
				csv.WriteField("Usage");
				csv.NextRecord();

				foreach (var usage in product1.ProductUsageNeeds)
				{
					csv.WriteField(product1.Name);
					csv.WriteField(product1.ThongSoKyThuat?.RAM);
					csv.WriteField(product1.ThongSoKyThuat?.CPU);
					csv.WriteField(product1.ThongSoKyThuat?.Camera);
					csv.WriteField(product1.Price);
					csv.WriteField(product1.ThongSoKyThuat?.Pin);
					csv.WriteField(usage.UsageNeed?.Name);
					csv.NextRecord();
				}

				foreach (var usage in product2.ProductUsageNeeds)
				{
					csv.WriteField(product2.Name);
					csv.WriteField(product2.ThongSoKyThuat?.RAM);
					csv.WriteField(product2.ThongSoKyThuat?.CPU);
					csv.WriteField(product2.ThongSoKyThuat?.Camera);
					csv.WriteField(product2.Price);
					csv.WriteField(product2.ThongSoKyThuat?.Pin);
					csv.WriteField(usage.UsageNeed?.Name);
					csv.NextRecord();
				}
			}
			SendFileToPython(csvFilePath);
			return csvFilePath;
		}

		private void ReadCompareResultsJson()
		{
			string jsonPath = @"C:\Users\ACER\PycharmProjects\ProjectPython\compare_results.json";
			if (System.IO.File.Exists(jsonPath))
			{
				string jsonData = System.IO.File.ReadAllText(jsonPath);
				var result = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonData);
				ViewBag.CompareResults = result;
			}
			else
			{
				ViewBag.JsonResult = "⚠️ Không tìm thấy file kết quả.";
			}
		}

		private void SendFileToPython(string filePath)
		{
			while (true)
			{
				try
				{
					using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
					{
						break;
					}
				}
				catch (IOException)
				{
					Thread.Sleep(1000);
				}
			}

			var client = new HttpClient();
			var fileContent = new MultipartFormDataContent();
			fileContent.Add(new StreamContent(new FileStream(filePath, FileMode.Open, FileAccess.Read)), "file", Path.GetFileName(filePath));

			var response = client.PostAsync("http://localhost:5000/upload-csv", fileContent).Result;

			if (!response.IsSuccessStatusCode)
			{
				Console.WriteLine("Có lỗi khi gửi file.");
			}
			else
			{
				Console.WriteLine("File đã được gửi thành công.");
			}
		}

		[HttpGet]
		public JsonResult GetCategoryId(long productId)
		{
			var categoryId = _dataContext.Products
				.Where(p => p.Id == productId)
				.Select(p => p.CategoryId)
				.FirstOrDefault();

			return Json(categoryId);
		}
	}
}


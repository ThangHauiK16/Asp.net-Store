using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;
using Shopping_Tutorial.Repository;

namespace Shopping_Tutorial.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Admin/ThongSoKyThuat")]
	public class ThongSoKyThuatController : Controller
	{
		public readonly DataContext _datacontext;

		public ThongSoKyThuatController(DataContext context)
		{
			_datacontext = context;

		}
		[Route("Index")]
		public async Task<IActionResult> Index()
		{
			var thongSoList = await _datacontext.ThongSoKyThuats
				.Include(t => t.Product)
				.ToListAsync();

			return View(thongSoList);
		}
		[Route("Create")]
		public IActionResult Create()
		{
			ViewBag.ProductList = new SelectList(_datacontext.Products, "Id", "Name");
			return View();
		}
		[HttpPost]
		[Route("Create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ThongSoKyThuatModel model)
		{
			if (ModelState.IsValid)
			{
				_datacontext.ThongSoKyThuats.Add(model);
				await _datacontext.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			ViewBag.ProductList = new SelectList(_datacontext.Products, "Id", "Name", model.ProductId);
			return View(model);
		}
		// GET: Edit
		[Route("Edit")]
		public async Task<IActionResult> Edit(int id)
		{
			var thongso = await _datacontext.ThongSoKyThuats.FindAsync(id);
			if (thongso == null)
			{
				return NotFound();
			}

			ViewBag.ProductList = new SelectList(_datacontext.Products, "Id", "Name", thongso.ProductId);
			return View(thongso);
		}

		// POST: Edit
		[HttpPost]
		[Route("Edit")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, ThongSoKyThuatModel model)
		{
			if (id != model.Id)
				return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					_datacontext.Update(model);
					await _datacontext.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!_datacontext.ThongSoKyThuats.Any(e => e.Id == id))
						return NotFound();
					else
						throw;
				}
			}

			ViewBag.ProductList = new SelectList(_datacontext.Products, "Id", "Name", model.ProductId);
			return View(model);
		}
		[HttpGet]
		[Route("Delete")]
		public async Task<IActionResult> Delete(int id)
		{
			var ts = await _datacontext.ThongSoKyThuats.FindAsync(id);
			if (ts == null)
			{
				TempData["error"] = "Không tìm thấy thông số.";
				return RedirectToAction("Index");
			}

			_datacontext.ThongSoKyThuats.Remove(ts);
			await _datacontext.SaveChangesAsync();

			TempData["success"] = "Đã xóa thông số thành công.";
			return RedirectToAction("Index");
		}



	}
}

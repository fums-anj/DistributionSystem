using BarcodeLib;
using BarcodeStandard;
using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.InventoryManagement;
using BH.Models.OrganizationManagement;
using BH.Models.ProductManagement;
using BH.Models.ViewModels;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;
using System.Drawing.Imaging;

namespace BHWeb.Areas.ProductManagement.Controllers
{
	[Area("ProductManagement")]
	[Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin)]
	public class VariantController : BaseController
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _hostEnvironment;
		private ApplicationUser applicationUser;
		public VariantController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_hostEnvironment = hostEnvironment;
			applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId, includeProperties: "Shop");
		}

		public IActionResult Index()
		{
			if (applicationUser == null)
			{
				applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
			}
			var objList = _unitOfWork.Variant.GetAll(d => d.IsDisable != true && d.IsDeleted != true && d.ShopId == applicationUser.ShopId, includeProperties: "ShopProduct.Catalog.Location,UnitOfMeasure,ShopProduct");

			return View(objList);
		}

		//GET
		public IActionResult Upsert(int? id)
		{
			VariantVM variantVM = new()
			{
				Variant = new(),
				LocationList = _unitOfWork.Location.GetAll().Where(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
				CatalogList = _unitOfWork.Catalog.GetAll().Where(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
				ShopProductList = _unitOfWork.ShopProduct.GetAll().Where(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
				UnitOfMeasureList = _unitOfWork.UnitOfMeasure.GetAll().Where(x => x.IsDisable != true).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
				SupplierList = _unitOfWork.Supplier.GetAll().Where(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
			};

			if (id == null || id == 0)
			{
				return View(variantVM);
			}
			else
			{
				variantVM.Variant = _unitOfWork.Variant.GetFirstOrDefault(u => u.Id == id);
				return View(variantVM);
			}


		}

		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Upsert(VariantVM obj, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
				string wwwRootPath = _hostEnvironment.WebRootPath;
				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString();
					var uploads = Path.Combine(wwwRootPath, @"images\Variants");
					var extension = Path.GetExtension(file.FileName);

					if (obj.Variant.ImageUrl != null)
					{
						var oldImagePath = Path.Combine(wwwRootPath, obj.Variant.ImageUrl.TrimStart('\\'));
						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
					{
						file.CopyTo(fileStreams);
					}
					obj.Variant.ImageUrl = @"\images\Variants\" + fileName + extension;

				}
				if (obj.Variant.Id == 0)
				{
					obj.Variant.CreatedBy = _userId;
					obj.Variant.CreatedDate = DateTime.Now;
					obj.Variant.ShopId = applicationUser.ShopId;
					obj.Variant.ShopProduct = null;
					obj.Variant.IsDisable = false;
					obj.Variant.VariantCode = _unitOfWork.Variant.GenerateCode();
					if (string.IsNullOrEmpty(obj.Variant.SKU))
					{
						obj.Variant.SKU = _unitOfWork.Variant.GenerateSKU(applicationUser.Shop.Name);
					}
					if (string.IsNullOrEmpty(obj.Variant.PackingSKU))
					{
						obj.Variant.PackingSKU = _unitOfWork.Variant.GenerateSKU(applicationUser.Shop.Name);
					}
					var objFromDb = _unitOfWork.Variant.GetFirstOrDefault(u => u.Name == obj.Variant.Name || u.SKU == obj.Variant.SKU);
					if (objFromDb == null)
					{
						_unitOfWork.Variant.Add(obj.Variant);
						TempData["success"] = "Variant created successfully";
					}
					else
					{
						TempData["error"] = "Variant already exists...";
					}
				}
				else
				{
					obj.Variant.ModifiedBy = _userId;
					obj.Variant.ModifiedDate = DateTime.Now;
					_unitOfWork.Variant.Update(obj.Variant);
					TempData["success"] = "Variant updated successfully";
				}
				_unitOfWork.Save();
				return RedirectToAction("Index");
			}
			return View(obj);
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			var obj = _unitOfWork.Variant.GetFirstOrDefault(u => u.Id == id);

			if (obj == null)
			{
				return NotFound();
			}

			return View(obj);
		}

		//POST
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeletePOST(int? id)
		{
			var obj = _unitOfWork.Variant.GetFirstOrDefault(u => u.Id == id);
			if (obj == null)
			{
				TempData["success"] = "Error while deleting";
				return RedirectToAction("Index");
			}
			if (obj.ImageUrl != null)
			{
				var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
				if (System.IO.File.Exists(oldImagePath))
				{
					System.IO.File.Delete(oldImagePath);
				}
			}
			StockTransfer stockTransfer = _unitOfWork.StockTransfer.GetFirstOrDefault(u => u.VariantId == id);
			if (stockTransfer != null)
			{
				TempData["success"] = "You are not allowed to delete this Variant...";
				return RedirectToAction("Index");
			}
			_unitOfWork.Variant.Remove(obj);
			_unitOfWork.Save();
			TempData["success"] = "Variant deleted successfully";
			return RedirectToAction("Index");
		}

		//GenerateBarcode
		public IActionResult GenerateBarcode(int? id, string? SKU)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			var obj = _unitOfWork.Variant.GetFirstOrDefault(u => u.Id == id && (u.SKU == SKU || u.PackingSKU == SKU), includeProperties: "ApplicationUser,ApplicationUser.Shop");

			if (obj == null)
			{
				return NotFound();
			}
			Barcode barcode = new Barcode();
			Image image = barcode.Encode(TYPE.CODE128, SKU, Color.Black, Color.White, 140, 50);
			using (MemoryStream ms = new MemoryStream())
			{
				image.Save(ms, ImageFormat.Png);
				ViewBag.Barcode = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
			}
			return View(obj);
		}

		//POST
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult GenerateBarcode(Variant variant)
		{
			TempData["success"] = "Variant printed successfully";
			return RedirectToAction("Index");
		}
	}
}

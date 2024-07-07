using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.OrganizationManagement;
using BH.Models.ViewModels;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BHWeb.Areas.ProductManagement.Controllers
{
    [Area("ProductManagement")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin)]
    public class ShopProductController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private ApplicationUser applicationUser;
        public ShopProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
        }

        public IActionResult Index()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            var objList = _unitOfWork.ShopProduct.GetAll(includeProperties: "Catalog.Location,Catalog").Where(d => d.IsDeleted != true && d.ShopId == applicationUser.ShopId);
            return View(objList);
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            ShopProductVM obj = new()
            {
                ShopProduct = new(),
                LocationList = _unitOfWork.Location.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }),
                CatalogList = _unitOfWork.Catalog.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() })
            };

            if (id == null || id == 0)
            {
                return View(obj);
            }
            else
            {
                obj.ShopProduct = _unitOfWork.ShopProduct.GetFirstOrDefault(u => u.Id == id);
                return View(obj);

                //update product
            }


        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ShopProductVM obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.ShopProduct.Id == 0)
                {
                    obj.ShopProduct.CreatedBy = _userId;
                    obj.ShopProduct.CreatedDate = DateTime.Now;
                    obj.ShopProduct.ShopId = applicationUser.ShopId;
                    obj.ShopProduct.IsDisable = false;
                    _unitOfWork.ShopProduct.Add(obj.ShopProduct);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    obj.ShopProduct.ModifiedBy = _userId;
                    obj.ShopProduct.ModifiedDate = DateTime.Now;
                    _unitOfWork.ShopProduct.Update(obj.ShopProduct);
                    TempData["success"] = "Product updated successfully";
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
            var obj = _unitOfWork.ShopProduct.GetFirstOrDefault(u => u.Id == id);

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
            var obj = _unitOfWork.ShopProduct.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                TempData["success"] = "Error while deleting";
                return RedirectToAction("Index");
            }


            _unitOfWork.ShopProduct.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction("Index");

        }
    }
}

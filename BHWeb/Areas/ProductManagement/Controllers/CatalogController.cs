using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.OrganizationManagement;
using BH.Models.ProductManagement;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BHWeb.Areas.ProductManagement.Controllers
{
    [Area("ProductManagement")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin)]
    public class CatalogController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private ApplicationUser applicationUser;
        public CatalogController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
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
            var objList = _unitOfWork.Catalog.GetAll(includeProperties: "Location").Where(d => d.IsDeleted == false && d.ShopId == applicationUser.ShopId);
            return View(objList);
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            Catalog catalog = new();
            ViewData["LoctionList"] = _unitOfWork.Location.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });

            if (id == null || id == 0)
            {
                return View(catalog);
            }
            else
            {
                catalog = _unitOfWork.Catalog.GetFirstOrDefault(u => u.Id == id);
                return View(catalog);

                //update product
            }


        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Catalog obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    obj.CreatedBy = _userId;
                    obj.CreatedDate = DateTime.Now;
                    obj.ShopId = applicationUser.ShopId;
                    obj.IsDisable = false;
                    _unitOfWork.Catalog.Add(obj);
                    TempData["success"] = "Catalog created successfully";
                }
                else
                {
                    obj.ModifiedBy = _userId;
                    obj.ModifiedDate = DateTime.UtcNow;
                    _unitOfWork.Catalog.Update(obj);
                    TempData["success"] = "Catalog updated successfully";
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
            var obj = _unitOfWork.Catalog.GetFirstOrDefault(u => u.Id == id);

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
            var obj = _unitOfWork.Catalog.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                TempData["success"] = "Error while deleting";
                return RedirectToAction("Index");
            }

            _unitOfWork.Catalog.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Location deleted successfully";
            return RedirectToAction("Index");

        }
    }
}

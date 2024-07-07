using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.OrganizationManagement;
using BH.Models.ViewModels;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BHWeb.Areas.Organization.Controllers
{
    [Area("Organization")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin)]
    public class LocationController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationUser applicationUser;
        public LocationController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
        }

        public IActionResult Index()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            IEnumerable<Location> objList = _unitOfWork.Location.GetAll().Where(x => x.ShopId == applicationUser.ShopId);
            return View(objList);
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            LocationVM obj = new()
            {
                Location = new(),
                LoctionTypeList = _unitOfWork.LocationType.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() })
            };

            if (id == null || id == 0)
            {
                //create product
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                return View(obj);
            }
            else
            {
                obj.Location = _unitOfWork.Location.GetFirstOrDefault(u => u.Id == id);
                return View(obj);

                //update product
            }


        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(LocationVM obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Location.Id == 0)
                {
                    obj.Location.CreatedBy = _userId;
                    obj.Location.CreatedDate = DateTime.UtcNow;
                    obj.Location.ShopId = applicationUser.ShopId;
                    obj.Location.IsDisable = false;
                    obj.Location.CostCenter = _unitOfWork.Location.GenerateCostCenter();
                    obj.Location.ProfitCenter = _unitOfWork.Location.GenerateProfitCenter();
                    _unitOfWork.Location.Add(obj.Location);
                    TempData["success"] = "Location created successfully";
                }
                else
                {
                    obj.Location.ModifiedBy = _userId;
                    obj.Location.ModifiedDate = DateTime.UtcNow;
                    _unitOfWork.Location.Update(obj.Location);
                    TempData["success"] = "Location updated successfully";
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
            var FromDbFirst = _unitOfWork.Location.GetFirstOrDefault(u => u.Id == id);

            if (FromDbFirst == null)
            {
                return NotFound();
            }

            return View(FromDbFirst);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Location.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Location.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Location deleted successfully";
            return RedirectToAction("Index");

        }
    }

}

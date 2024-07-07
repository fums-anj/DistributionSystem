using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.OrganizationManagement;
using BH.Models.ProductManagement;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BHWeb.Areas.ProductManagement.Controllers
{
    [Area("ProductManagement")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin)]
    public class UnitOfMeasuresController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationUser applicationUser;
        public UnitOfMeasuresController(IUnitOfWork unitOfWork)
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
            IEnumerable<UnitOfMeasure> objList = _unitOfWork.UnitOfMeasure.GetAll().Where(x => x.IsDisable != true && x.IsDisable != true && x.IsDeleted != true);
            return View(objList);
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            UnitOfMeasure obj = new();

            if (id == null || id == 0)
            {
                return View(obj);
            }
            else
            {
                obj = _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Id == id);
                return View(obj);
            }


        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(UnitOfMeasure obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    obj.CreatedBy = _userId;
                    obj.CreatedDate = DateTime.Now;
                    obj.ShopId = applicationUser.ShopId;
                    obj.IsDisable = false;
                    _unitOfWork.UnitOfMeasure.Add(obj);
                    TempData["success"] = "Unit of measure created successfully";
                }
                else
                {
                    obj.ModifiedBy = _userId;
                    obj.ModifiedDate = DateTime.Now;
                    _unitOfWork.UnitOfMeasure.Update(obj);
                    TempData["success"] = "Unit of measure updated successfully";
                }
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var FromDbFirst = _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Id == id);

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
            var obj = _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.UnitOfMeasure.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Unit of measure deleted successfully";
            return RedirectToAction(nameof(Index));

        }
    }
}

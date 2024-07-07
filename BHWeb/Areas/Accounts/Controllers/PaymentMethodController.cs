using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.AccountManagement;
using BH.Models.OrganizationManagement;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BHWeb.Areas.AccountManagement.Controllers
{
    [Area("Accounts")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin)]
    public class PaymentMethodController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationUser applicationUser;
        public PaymentMethodController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
        }

        public IActionResult Index()
        {
            IEnumerable<PaymentMethod> objList = _unitOfWork.PaymentMethod.GetAll().Where(x =>  x.IsDisable != true && x.IsDeleted != true);
            return View(objList);
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            PaymentMethod obj = new();

            if (id == null || id == 0)
            {
                return View(obj);
            }
            else
            {
                obj = _unitOfWork.PaymentMethod.GetFirstOrDefault(u => u.Id == id);
                return View(obj);
            }
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(PaymentMethod obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    obj.CreatedBy = _userId;
                    obj.CreatedDate = DateTime.UtcNow;
                    obj.ShopId = applicationUser.ShopId;
                    obj.IsDisable = false;
                    _unitOfWork.PaymentMethod.Add(obj);
                    TempData["success"] = "Payment Method created successfully";
                }
                else
                {
                    obj.ModifiedBy = _userId;
                    obj.ModifiedDate = DateTime.UtcNow;
                    _unitOfWork.PaymentMethod.Update(obj);
                    TempData["success"] = "Payment Method updated successfully";
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
            var FromDbFirst = _unitOfWork.PaymentMethod.GetFirstOrDefault(u => u.Id == id);
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
            var obj = _unitOfWork.PaymentMethod.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.PaymentMethod.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Payment Method deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}

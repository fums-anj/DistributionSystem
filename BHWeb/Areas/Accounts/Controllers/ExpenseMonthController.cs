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
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin + "," + SDRoles.Role_ShopUser)]
    public class ExpenseMonthController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationUser applicationUser;
        public ExpenseMonthController(IUnitOfWork unitOfWork)
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
            IEnumerable<ExpenseMonth> objList = _unitOfWork.ExpenseMonth.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true);
            return View(objList);
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            ExpenseMonth obj = new();
            if (id == null || id == 0)
            {
                return View(obj);
            }
            else
            {
                obj = _unitOfWork.ExpenseMonth.GetFirstOrDefault(u => u.Id == id);
                return View(obj);
            }
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ExpenseMonth obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    obj.CreatedBy = _userId;
                    obj.CreatedDate = DateTime.UtcNow;
                    obj.ShopId = applicationUser.ShopId;
                    obj.IsDisable = false;
                    _unitOfWork.ExpenseMonth.Add(obj);
                    TempData["success"] = "Expense month added successfully";
                }
                else
                {
                    obj.ModifiedBy = _userId;
                    obj.ModifiedDate = DateTime.UtcNow;
                    _unitOfWork.ExpenseMonth.Update(obj);
                    TempData["success"] = "Expense month updated successfully";
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
            var FromDbFirst = _unitOfWork.ExpenseMonth.GetFirstOrDefault(u => u.Id == id);

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
            var obj = _unitOfWork.ExpenseMonth.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.ExpenseMonth.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Expense month deleted successfully";
            return RedirectToAction("Index");
        }
    }
}

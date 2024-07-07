using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.AccountManagement;
using BH.Models.OrganizationManagement;
using BH.Models.ViewModels;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BHWeb.Areas.AccountManagement.Controllers
{
    [Area("Accounts")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin + "," + SDRoles.Role_ShopUser)]
    public class ExpenseController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationUser applicationUser;
        public ExpenseController(IUnitOfWork unitOfWork)
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
            ExpenseVM expenseVM = new ExpenseVM();
            expenseVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            expenseVM.ExpenseTypeList = new SelectList(_unitOfWork.ExpenseType.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            expenseVM.ExpenseList = _unitOfWork.Expense.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true, includeProperties: "ExpenseType");
            return View(expenseVM);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Index(ExpenseVM expenseVM)
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            expenseVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            expenseVM.ExpenseTypeList = new SelectList(_unitOfWork.ExpenseType.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            expenseVM.ExpenseList = _unitOfWork.Expense.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true, includeProperties: "ExpenseType");
            expenseVM.ExpenseList = _unitOfWork.Expense.FilterExpense(expenseVM);
            return View(expenseVM);
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            ViewBag.ExpenseType = _unitOfWork.ExpenseType.GetAll(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            Expense obj = new();
            obj.ApprovedDate = DateTime.Now;
            if (id == null || id == 0)
            {
                return View(obj);
            }
            else
            {
                obj = _unitOfWork.Expense.GetFirstOrDefault(u => u.Id == id);
                return View(obj);
            }
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Upsert(Expense obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    if (obj.Amount <= 0)
                    {
                        TempData["error"] = "Amount must be greater than zero...";
                        return RedirectToAction(nameof(Upsert));
                    }
                    obj.CreatedBy = _userId;
                    obj.CreatedDate = DateTime.Now;
                    obj.ShopId = applicationUser.ShopId;
                    obj.IsDisable = false;
                    _unitOfWork.Expense.Add(obj);
                    TempData["success"] = "Expense managed successfully";
                }
                else
                {
                    obj.ModifiedBy = _userId;
                    obj.ModifiedDate = DateTime.UtcNow;
                    _unitOfWork.Expense.Update(obj);
                    TempData["success"] = "Expense updated successfully";
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
            var FromDbFirst = _unitOfWork.Expense.GetFirstOrDefault(u => u.Id == id, includeProperties: "ExpenseType");

            if (FromDbFirst == null)
            {
                return NotFound();
            }

            return View(FromDbFirst);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Expense.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Expense.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Deleted successfully";
            return RedirectToAction("Index");
        }
    }
}

using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.InventoryManagement;
using BH.Models.OrganizationManagement;
using BH.Models.SaleManagement;
using BH.Models.ViewModels;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BHWeb.Areas.Accounts.Controllers
{
    [Area("Accounts")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin + "," + SDRoles.Role_ShopUser)]
    public class DailyCashController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationUser applicationUser;
        public DailyCashController(IUnitOfWork unitOfWork)
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
            IEnumerable<ManageCash> objList = _unitOfWork.ManageCash.GetAll().Where(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true && x.SupplierId == null);
            return View(objList);
        }
        public IActionResult IndexSupplierCash()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            ManageCashVM obj = new ManageCashVM();
            obj.ManageCash = new ManageCash();
            obj.SupplierList = _unitOfWork.Supplier.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.manageCashList = _unitOfWork.ManageCash.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true && x.SupplierId != null, includeProperties: "Supplier");
            return View(obj);
        }
        public IActionResult IndexSingleSupplierCash(int SuppId)
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            ManageCashVM obj = new ManageCashVM();
            obj.SupplierList = _unitOfWork.Supplier.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.ManageCash = new ManageCash();
            obj.ManageCash.SupplierId = SuppId;
            obj.manageCashList = _unitOfWork.ManageCash.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true && x.SupplierId == SuppId, includeProperties: "Supplier");
            return View(obj);
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            ManageCash obj = new();
            if (id == null || id == 0)
            {
                return View(obj);
            }
            else
            {
                obj = _unitOfWork.ManageCash.GetFirstOrDefault(u => u.Id == id);
                return View(obj);
            }
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ManageCash obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    if (obj.SaleAmount <= 0)
                    {
                        TempData["error"] = "Cash must be greater than zero...";
                        return RedirectToAction(nameof(Upsert));
                    }
;
                    obj.CreatedBy = _userId;
                    obj.CreatedDate = DateTime.Now;
                    obj.ShopId = applicationUser.ShopId;
                    obj.Credit = 0;
                    obj.Debit = 0;
                    obj.Balance = 0;
                    obj.IsDisable = false;
                    _unitOfWork.ManageCash.Add(obj);
                    TempData["success"] = "Cash managed successfully";
                }
                else
                {
                    obj.ModifiedBy = _userId;
                    obj.ModifiedDate = DateTime.Now;
                    _unitOfWork.ManageCash.Update(obj);
                    TempData["success"] = "Cash updated successfully";
                }
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            return View(obj);
        }
        public IActionResult UpsertPaySupplier(int? id)
        {
            ViewBag.SupplierList = _unitOfWork.Supplier.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            ManageCash obj = new();
            if (id == null || id == 0)
            {
                return View(obj);
            }
            else
            {
                obj = _unitOfWork.ManageCash.GetFirstOrDefault(u => u.Id == id);
                return View(obj);
            }
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertPaySupplier(ManageCashVM obj)
        {
            if (obj.ManageCash.Id == 0)
            {
                if (obj.ManageCash.Credit <= 0 && obj.ManageCash.Debit <= 0)
                {
                    TempData["error"] = "Provide credit or debit...";
                    return RedirectToAction(nameof(UpsertPaySupplier));
                }
;
                obj.ManageCash.CreatedBy = _userId;
                obj.ManageCash.CreatedDate = DateTime.UtcNow;
                obj.ManageCash.ShopId = applicationUser.ShopId;
                obj.ManageCash.SaleAmount = 0;
                double dbBalance = 0;

                IEnumerable<ManageCash> manageCashes = _unitOfWork.ManageCash.GetAll(g => g.SupplierId == obj.ManageCash.SupplierId);
                if (manageCashes.Count() > 0)
                {
                    dbBalance = manageCashes.OrderByDescending(o => o.Id).FirstOrDefault().Balance;
                }
                obj.ManageCash.Balance = obj.ManageCash.Credit - obj.ManageCash.Debit + dbBalance;
                obj.ManageCash.IsDisable = false;
                _unitOfWork.ManageCash.Add(obj.ManageCash);
                TempData["success"] = "Cash managed successfully";
            }
            else
            {
                obj.ManageCash.ModifiedBy = _userId;
                obj.ManageCash.ModifiedDate = DateTime.UtcNow;
                _unitOfWork.ManageCash.Update(obj.ManageCash);
                TempData["success"] = "Cash updated successfully";
            }
            _unitOfWork.Save();

            return RedirectToAction(nameof(IndexSupplierCash));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var FromDbFirst = _unitOfWork.ManageCash.GetFirstOrDefault(u => u.Id == id);

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
            var obj = _unitOfWork.ManageCash.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.ManageCash.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Cash deleted successfully";
            return RedirectToAction("Index");
        }
        public IActionResult EndOfDay(int? id)
        {
            if (id != null)
            {
                var obj = _unitOfWork.CreditNote.GetFirstOrDefault(u => u.Id == id);
                if (obj == null)
                {
                    return NotFound();
                }
                obj.CashOut = true;
                obj.ModifiedBy = applicationUser.Id;
                obj.ModifiedDate = DateTime.UtcNow;
                _unitOfWork.CreditNote.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Cash out successful...";
            }
            IEnumerable<CreditNote> CreditNoteList = _unitOfWork.CreditNote.GetAll(u => u.ShopId == applicationUser.ShopId && u.CashOut == false, includeProperties: "ApplicationUser");

            if (CreditNoteList == null)
            {
                return NotFound();
            }

            return View(CreditNoteList);
        }

        //POST
        [HttpPost, ActionName("EndOfDay")]
        [ValidateAntiForgeryToken]
        public IActionResult EndOfDayPOST(int? id)
        {
            var obj = _unitOfWork.CreditNote.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.CreditNote.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Cash deleted successfully";
            return RedirectToAction("Index");
        }
    }
}

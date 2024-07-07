using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.OrganizationManagement;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BHWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin)]
    public class CustomerRouteController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationUser applicationUser;
        private readonly UserManager<ApplicationUser> _userManager;
        public CustomerRouteController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
        }
        public IActionResult Index()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            IEnumerable<CustomerRoute> objList = _unitOfWork.CustomerRoute.GetAll(includeProperties: "Saleman").Where(x => x.ShopId == applicationUser.ShopId);
            return View(objList);
        }

        //GET
        public IActionResult Upsert(int? id)
        {
            //ViewBag.Saleman = _unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text=i.FullName, Value=i.Id});
            ViewBag.Saleman =  _userManager.GetUsersInRoleAsync(SDRoles.Role_Saleman).Result.Select(i => new SelectListItem { Text = i.FullName, Value = i.Id });
            CustomerRoute customerRoute = new();

            if (id == null || id == 0)
            {
                return View(customerRoute);
            }
            else
            {
                customerRoute = _unitOfWork.CustomerRoute.GetFirstOrDefault(u => u.Id == id);
                return View(customerRoute);
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CustomerRoute obj)
        {
            if (ModelState.IsValid)
            {
               if (obj.Id == 0)
                {
                    obj.CreatedBy = _userId;
                    obj.CreatedDate = DateTime.Now;
                    obj.ShopId = applicationUser.ShopId;
                    _unitOfWork.CustomerRoute.Add(obj);
                    TempData["success"] = "Route created successfully";
                }
                else
                {
                    obj.ModifiedBy = _userId;
                    obj.ModifiedDate = DateTime.Now;
                    _unitOfWork.CustomerRoute.Update(obj);
                    TempData["success"] = "Route updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var FromDbFirst = _unitOfWork.CustomerRoute.GetFirstOrDefault(u => u.Id == id);

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
            var obj = _unitOfWork.CustomerRoute.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.CustomerRoute.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Route deleted successfully";
            return RedirectToAction("Index");

        }
    }
}

using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.CustomerManagement;
using BH.Models.OrganizationManagement;
using BH.Models.ViewModels;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace BHWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin)]
    public class ShopCustomerController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationUser applicationUser;
        public ShopCustomerController(IUnitOfWork unitOfWork)
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
            IEnumerable<ShopCustomer> objList = _unitOfWork.ShopCustomer.GetAll(includeProperties: "CustomerRoute").Where(x => x.ShopId == applicationUser.ShopId);
            return View(objList);
        }

		
		public IActionResult CustomerOfRouteList(int routeId)
		{
			try
			{
				if (applicationUser == null)
				{
					applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
				}

				var customers = _unitOfWork.ShopCustomer.GetAll(includeProperties: "CustomerRoute")
					.Where(x => x.ShopId == applicationUser.ShopId && x.CustomerRouteId == routeId);
                var AccountReceivables = _unitOfWork.AccountReceivable.GetAll(x => x.ShopId == applicationUser.ShopId &&
																				   x.ShopCustomer != null && // Ensure ShopCustomer is not null
																				   x.ShopCustomer.CustomerRouteId == routeId).OrderByDescending(ar => ar.ReceivedDate);
                var StockTransfers = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && 
																		   x.ShopCustomer != null &&
                                                                           x.ShopCustomer.CustomerRouteId == routeId).OrderByDescending(st => st.CreatedDate);
                
				var viewModel = customers.Select(c => new ShopCustomerVM
				{
					Customer = c,
					LastReceivedDate = AccountReceivables.FirstOrDefault(st => st.ShopCustomerId == c.Id)?.ReceivedDate,
					LastRecovery = AccountReceivables.FirstOrDefault(st => st.ShopCustomerId == c.Id)?.ReceivedAmount,
                    LastStockTransferDate = StockTransfers.FirstOrDefault(st => st.ShopCustomerId == c.Id)?.CreatedDate,

					largerDate = (AccountReceivables.FirstOrDefault(st => st.ShopCustomerId == c.Id)?.ReceivedDate == null) ? StockTransfers.FirstOrDefault(st => st.ShopCustomerId == c.Id)?.CreatedDate :
									(StockTransfers.FirstOrDefault(st => st.ShopCustomerId == c.Id)?.CreatedDate == null) ? AccountReceivables.FirstOrDefault(st => st.ShopCustomerId == c.Id)?.ReceivedDate :
                                    (AccountReceivables.FirstOrDefault(st => st.ShopCustomerId == c.Id)?.ReceivedDate
								> StockTransfers.FirstOrDefault(st => st.ShopCustomerId == c.Id)?.CreatedDate) ?
                                AccountReceivables.FirstOrDefault(st => st.ShopCustomerId == c.Id)?.ReceivedDate : 
                                StockTransfers.FirstOrDefault(st => st.ShopCustomerId == c.Id)?.CreatedDate

				}).ToList();

				return View(viewModel);
			}
			catch (Exception ex)
			{
				// Log error
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}



		//GET
		public IActionResult Upsert(int? id, string? addPOS)
        {
            ViewBag.CustomerRoute = _unitOfWork.CustomerRoute.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.RouteName, Value = i.Id.ToString() });
            ShopCustomer shopCoustomer = new();
            if (addPOS != null)
            {
                shopCoustomer.CreateFromPOS = addPOS;
            }

            if (id == null || id == 0)
            {
                return View(shopCoustomer);
            }
            else
            {
                shopCoustomer = _unitOfWork.ShopCustomer.GetFirstOrDefault(u => u.Id == id);
                return View(shopCoustomer);
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ShopCustomer obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    obj.CreatedBy = _userId;
                    obj.CreatedDate = DateTime.Now;
                    obj.CustomerCode = _unitOfWork.ShopCustomer.GenerateCustomerCode();
                    obj.ShopId = applicationUser.ShopId;
                    _unitOfWork.ShopCustomer.Add(obj);
                    TempData["success"] = "Customer Shop created successfully";
                }
                else
                {
                    obj.ModifiedBy = _userId;
                    obj.ModifiedDate = DateTime.Now;
                    _unitOfWork.ShopCustomer.Update(obj);
                    TempData["success"] = "Customer Shop updated successfully";
                }
                _unitOfWork.Save();
                if (obj.CreateFromPOS != null)
                {
                    return RedirectToAction(nameof(Index), "POS", new { area = "Customer" });
                }
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
            var FromDbFirst = _unitOfWork.ShopCustomer.GetFirstOrDefault(u => u.Id == id);

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
            var obj = _unitOfWork.ShopCustomer.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.ShopCustomer.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Customer Shop deleted successfully";
            return RedirectToAction("Index");

        }
    }
}

﻿using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.SaleManagement;
using BH.Models.ViewModels;
using BH.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Security.Claims;

namespace BHWeb.Areas.SaleManagement.Controllers
{
    [Area("SaleManagement")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product"),
            };
            return View(OrderVM);
        }

        //[ActionName("Details")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Details_PAY_NOW()
        //{
        //    OrderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
        //    OrderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

        //    //stripe settings 
        //    var domain = "https://localhost:44300/";
        //    var options = new SessionCreateOptions
        //    {
        //        PaymentMethodTypes = new List<string>
        //        {
        //          "card",
        //        },
        //        LineItems = new List<SessionLineItemOptions>(),
        //        Mode = "payment",
        //        SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderid={OrderVM.OrderHeader.Id}",
        //        CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",
        //    };

        //    foreach (var item in OrderVM.OrderDetail)
        //    {

        //        var sessionLineItem = new SessionLineItemOptions
        //        {
        //            PriceData = new SessionLineItemPriceDataOptions
        //            {
        //                UnitAmount = (long)(item.Price * 100),//20.00 -> 2000
        //                Currency = "usd",
        //                ProductData = new SessionLineItemPriceDataProductDataOptions
        //                {
        //                    Name = item.Product.Title
        //                },

        //            },
        //            Quantity = item.Count,
        //        };
        //        options.LineItems.Add(sessionLineItem);

        //    }

        //    var service = new SessionService();
        //    Session session = service.Create(options);
        //    _unitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
        //    _unitOfWork.Save();
        //    Response.Headers.Add("Location", session.Url);
        //    return new StatusCodeResult(303);
        //}

        //public IActionResult PaymentConfirmation(int orderHeaderid)
        //{
        //    OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderHeaderid);
        //    if (orderHeader.PaymentStatus == SDPaymentStatus.PaymentStatusDelayedPayment)
        //    {
        //        var service = new SessionService();
        //        Session session = service.Get(orderHeader.SessionId);
        //        //check the stripe status
        //        if (session.PaymentStatus.ToLower() == "paid")
        //        {
        //            _unitOfWork.OrderHeader.UpdateStatus(orderHeaderid, orderHeader.OrderStatus, SDPaymentStatus.PaymentStatusApproved);
        //            _unitOfWork.Save();
        //        }
        //    }
        //    return View(orderHeaderid);
        //}

        [HttpPost]
        [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetail()
        {
            var orderHEaderFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked: false);
            orderHEaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHEaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHEaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHEaderFromDb.City = OrderVM.OrderHeader.City;
            orderHEaderFromDb.State = OrderVM.OrderHeader.State;
            orderHEaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            if (OrderVM.OrderHeader.Carrier != null)
            {
                orderHEaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if (OrderVM.OrderHeader.TrackingNumber != null)
            {
                orderHEaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHEaderFromDb);
            _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction("Details", "Order", new { orderId = orderHEaderFromDb.Id });
        }

        [HttpPost]
        [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult StartProcessing()
        {
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SDStatus.StatusInProcess);
            _unitOfWork.Save();
            TempData["Success"] = "Order Status Updated Successfully.";
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_Employee)]
        [ValidateAntiForgeryToken]
        public IActionResult ShipOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked: false);
            orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeader.OrderStatus = SDStatus.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            if (orderHeader.PaymentStatus == SDPaymentStatus.PaymentStatusDelayedPayment)
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully.";
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_Employee)]
        [ValidateAntiForgeryToken]
        //public IActionResult CancelOrder()
        //{
        //    var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id, tracked: false);
        //    if (orderHeader.PaymentStatus == SDPaymentStatus.PaymentStatusApproved)
        //    {
        //        var options = new RefundCreateOptions
        //        {
        //            Reason = RefundReasons.RequestedByCustomer,
        //            PaymentIntent = orderHeader.PaymentIntentId
        //        };

        //        var service = new RefundService();
        //        Refund refund = service.Create(options);

        //        _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SDStatus.StatusCancelled, SDStatus.StatusRefunded);
        //    }
        //    else
        //    {
        //        _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SDStatus.StatusCancelled, SDStatus.StatusCancelled);
        //    }
        //    _unitOfWork.Save();

        //    TempData["Success"] = "Order Cancelled Successfully.";
        //    return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(SDRoles.Role_Admin) || User.IsInRole(SDRoles.Role_Employee))
            {
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SDPaymentStatus.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SDStatus.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SDStatus.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SDStatus.StatusApproved);
                    break;
                default:
                    break;
            }


            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}

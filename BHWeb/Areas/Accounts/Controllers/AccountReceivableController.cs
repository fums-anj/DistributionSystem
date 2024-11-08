﻿using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.AccountManagement;
using BH.Models.CustomerManagement;
using BH.Models.OrganizationManagement;
using BH.Models.ViewModels;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BHWeb.Areas.Accounts.Controllers
{

    [Area("Accounts")]
    [Authorize(Roles = $"{SDRoles.Role_Admin},{SDRoles.Role_ShopAdmin},{SDRoles.Role_ShopUser}")]
    public class AccountReceivableController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationUser applicationUser;

        public AccountReceivableController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
        }

        private void EnsureApplicationUser() =>
            applicationUser ??= _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);

        public IActionResult CustomerCashList()
        {
            EnsureApplicationUser();
            var shopId = applicationUser.ShopId;
            var today = DateTime.Now.Date;

            var accountReceivableVM = new AccountReceivableVM
            {
                accountReceivable = new(),
                //ShopCustomerList = GetSelectList(_unitOfWork.ShopCustomer.GetAll(x => x.ShopId == shopId), c => c.CustomerName, c => c.Id.ToString()),
                ShopCustomerList = _unitOfWork.ShopCustomer.GetSelectList(c => c.CustomerName, c => c.Id.ToString(), x => x.ShopId == shopId),
                RouteList = _unitOfWork.CustomerRoute.GetSelectList(r => r.RouteName, r => r.Id.ToString(), x => x.ShopId == shopId),
                AccountReceivableList = _unitOfWork.AccountReceivable.GetAll(
                    x => x.ShopId == shopId && !x.IsDisable && !x.IsDeleted && x.ShopCustomerId != null && x.CreatedDate >= today,
                    includeProperties: "ShopCustomer,ShopCustomer.CustomerRoute"
                ),
                TotalExpance = _unitOfWork.Expense.GetAll(x => x.ShopId == shopId && !x.IsDisable && !x.IsDeleted && x.ApprovedDate == today)
                              .Sum(x => x.Amount),
                CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == shopId && !x.IsDeleted && !x.IsDisable),
            };

            accountReceivableVM.AccountReceivableVMList = accountReceivableVM.AccountReceivableList
                .OrderByDescending(u => u.Id)
                .GroupBy(g => g.ShopCustomer.CustomerName)
                .Select(s => new AccountReceivableVM
                {
                    ShopCustomerName = s.Key,
                    ShopCustomerCode = s.First().ShopCustomer.CustomerCode,
                    ShopCustomerAddress = s.First().ShopCustomer.CustomerCity,
                    Payable = s.Sum(x => x.TotalReceivable),
                    Paid = s.Where(x => x.ReceivedAmount >= 0).Sum(x => x.ReceivedAmount),
                    OldBalance = s.Where(x => x.ReceivedAmount < 0).Sum(x => x.ReceivedAmount),
                    Balance = s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount),
                    TotalBalance = s.First().ShopCustomer.Balance,
                    PreviousBalance = s.First().ShopCustomer.Balance - (s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount)),
                    ShopCustomerId = s.First().ShopCustomerId,
                    Route = s.First().ShopCustomer.CustomerRoute.RouteName,
                    TermDay = s.First().ShopCustomer.PaymentTermsDays
                }).ToList();

            return View(accountReceivableVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult CustomerCashList(AccountReceivableVM obj)
        {
            EnsureApplicationUser();
            var shopId = applicationUser.ShopId;

            obj.accountReceivable = new();
            obj.User = applicationUser;
            obj.ShopCustomerList = _unitOfWork.ShopCustomer.GetSelectList( c => c.CustomerName, c => c.Id.ToString(), x => x.ShopId == shopId);
            obj.RouteList = _unitOfWork.CustomerRoute.GetSelectList(r => r.RouteName, r => r.Id.ToString(), x => x.ShopId == shopId);
            obj.TotalExpance = _unitOfWork.Expense.GetAll(x => x.ShopId == shopId && !x.IsDisable && !x.IsDeleted && x.ApprovedDate >= obj.RegDateFrom && x.ApprovedDate <= obj.RegDateTo).Sum(x => x.Amount);
            obj.AccountReceivableList = _unitOfWork.AccountReceivable.FilterAccountReceivable(obj);
            obj.CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == shopId && !x.IsDeleted && !x.IsDisable);

            if (obj.AccountReceivableList.Any())
            {
                obj.AccountReceivableVMList = obj.AccountReceivableList
                    .OrderByDescending(u => u.Id)
                    .GroupBy(g => g.ShopCustomer.CustomerName)
                    .Select(s => new AccountReceivableVM
                    {
                        ShopCustomerName = s.Key,
                        ShopCustomerCode = s.First().ShopCustomer.CustomerCode,
                        ShopCustomerAddress = s.First().ShopCustomer.CustomerCity,
                        Payable = s.Sum(x => x.TotalReceivable),
                        Paid = s.Where(x => x.ReceivedAmount >= 0).Sum(x => x.ReceivedAmount),
                        OldBalance = s.Where(x => x.ReceivedAmount < 0).Sum(x => x.ReceivedAmount),
                        Balance = s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount),
                        TotalBalance = s.First().ShopCustomer.Balance,
                        PreviousBalance = s.First().ShopCustomer.Balance - (s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount)),
                        ShopCustomerId = s.First().ShopCustomerId,
                        Route = s.First().ShopCustomer.CustomerRoute.RouteName,
                        TermDay = s.First().ShopCustomer.PaymentTermsDays
                    }).ToList();
            }

            return View(obj);
        }

        public IActionResult CustomerSalesDetail(int CustomerId) => GetSingleCustomerCashList(CustomerId);

        public IActionResult SingleCustomerCashList(int CustomerId) => GetSingleCustomerCashList(CustomerId);

        private IActionResult GetSingleCustomerCashList(int customerId)
        {
            EnsureApplicationUser();
            var shopId = applicationUser.ShopId;

            var obj = new AccountReceivableVM
            {
                accountReceivable = new AccountReceivable { ShopCustomerId = customerId },
                ShopCustomerId = customerId,
                ShopCustomerList = _unitOfWork.ShopCustomer.GetSelectList(c => c.CustomerName, c => c.Id.ToString(), x => x.ShopId == shopId),
                AccountReceivableList = _unitOfWork.AccountReceivable.GetAll(
                    x => x.ShopId == shopId && !x.IsDisable && !x.IsDeleted && x.ShopCustomerId == customerId,
                    includeProperties: "SaleOrder,SaleOrder.Customer"
                ).ToList(),
                CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == shopId && !x.IsDeleted && !x.IsDisable)
            };

            return obj.AccountReceivableList.Any() ? View(obj) : RedirectToAction("CustomerCashList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SingleCustomerCashList(AccountReceivableVM obj)
        {
            EnsureApplicationUser();
            var shopId = applicationUser.ShopId;

            obj.ShopCustomerList = _unitOfWork.ShopCustomer.GetSelectList(c => c.CustomerName, c => c.Id.ToString(), x => x.ShopId == shopId);
            obj.accountReceivable = new();

            if (obj.ReceivedDateFrom != null)
            {
                obj.ReceivedDateTo ??= DateTime.Now;
                var endDate = obj.ReceivedDateTo.Value.AddDays(1);

                obj.AccountReceivableList = _unitOfWork.AccountReceivable.GetAll(
                    x => x.ShopId == shopId && !x.IsDisable && !x.IsDeleted && x.ReceivedDate >= obj.ReceivedDateFrom && x.ReceivedDate < endDate && x.ShopCustomerId == obj.ShopCustomerId,
                    includeProperties: "SaleOrder,SaleOrder.Customer"
                ).ToList();
            }

            obj.CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == shopId && !x.IsDeleted && !x.IsDisable);
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertPayCustomer(AccountReceivableVM obj)
        {
            if (obj.accountReceivable.Id == 0)
            {
                if (string.IsNullOrEmpty(obj.ShopCustomerCodeAndName) || obj.accountReceivable.ReceivedAmount == 0)
                {
                    TempData["error"] = obj.ShopCustomerCodeAndName == null ? "Provide Customer..." : "Provide received amount...";
                    return RedirectToAction(nameof(CustomerCashList));
                }

                var customerCode = obj.ShopCustomerCodeAndName.Split(",")[0];
                var shopCustomer = _unitOfWork.ShopCustomer.GetFirstOrDefault(u => u.CustomerCode == customerCode && !u.IsDisable);

                if (shopCustomer == null)
                {
                    TempData["error"] = "Provide existing customer...";
                    return RedirectToAction(nameof(CustomerCashList));
                }

                obj.accountReceivable.ShopCustomerId = shopCustomer.Id;
                obj.accountReceivable.CreatedBy = _userId;
                obj.accountReceivable.ReceivedDate ??= DateTime.Now;
                obj.accountReceivable.ShopId = applicationUser.ShopId;

                _unitOfWork.AccountReceivable.Add(obj.accountReceivable);
                _unitOfWork.Save();
                TempData["success"] = "Received amount saved successfully";
            }
            else
            {
                _unitOfWork.AccountReceivable.Update(obj.accountReceivable);
                _unitOfWork.Save();
                TempData["success"] = "Record updated successfully";
            }

            return RedirectToAction(nameof(CustomerCashList));
        }
    }


    //the following is older code and the above is generated by Chatgpt

    //[Area("Accounts")]
    //[Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin + "," + SDRoles.Role_ShopUser)]
    //public class AccountReceivableController : BaseController
    //{
    //    private readonly IUnitOfWork _unitOfWork;
    //    private ApplicationUser applicationUser;
    //    public AccountReceivableController(IUnitOfWork unitOfWork)
    //    {
    //        _unitOfWork = unitOfWork;
    //        applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
    //    }

    //    public IActionResult CustomerCashList()
    //    {
    //        applicationUser ??= _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
    //        AccountReceivableVM obj = new AccountReceivableVM();
    //        obj.accountReceivable = new AccountReceivable();
    //        obj.ShopCustomerList = _unitOfWork.ShopCustomer.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.CustomerName, Value = i.Id.ToString() });
    //        obj.RouteList = _unitOfWork.CustomerRoute.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.RouteName, Value = i.Id.ToString() });
    //        obj.AccountReceivableList = _unitOfWork.AccountReceivable.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true && x.ShopCustomerId != null && x.CreatedDate >= DateTime.Now.Date, includeProperties: "ShopCustomer,ShopCustomer.CustomerRoute");
    //        obj.TotalExpance = _unitOfWork.Expense.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true && x.ApprovedDate >= DateTime.Now.Date && x.ApprovedDate <= DateTime.Now.Date).Sum(x => x.Amount);
    //        obj.CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.IsDisable != true);

    //        obj.AccountReceivableVMList = obj.AccountReceivableList.OrderByDescending(u => u.Id).GroupBy(g => g.ShopCustomer.CustomerName).Select(s => new AccountReceivableVM()
    //        {
    //            ShopCustomerName = s.Key,
    //            ShopCustomerCode = s.FirstOrDefault().ShopCustomer.CustomerCode,
    //            ShopCustomerAddress = s.FirstOrDefault().ShopCustomer.CustomerCity,
    //            Payable = s.Sum(x => x.TotalReceivable),
    //            Paid = s.Where(x => x.ReceivedAmount >= 0).Sum(x => x.ReceivedAmount),
    //            OldBalance = s.Where(x => x.ReceivedAmount < 0).Sum(x => x.ReceivedAmount),
    //            Balance = s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount),
    //            TotalBalance = s.FirstOrDefault().ShopCustomer.Balance,
    //            PreviousBalance = s.FirstOrDefault().ShopCustomer.Balance - (s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount)),
    //            ShopCustomerId = s.FirstOrDefault().ShopCustomerId,
    //            Route = s.FirstOrDefault().ShopCustomer.CustomerRoute.RouteName,
    //            TermDay = s.FirstOrDefault().ShopCustomer.PaymentTermsDays
    //        }).ToList();


    //        return View(obj);
    //    }
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    [Authorize]
    //    public IActionResult CustomerCashList(AccountReceivableVM obj)
    //    {
    //        if (applicationUser == null)
    //        {
    //            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
    //        }
    //        obj.accountReceivable = new AccountReceivable();
    //        obj.User = applicationUser;
    //        obj.ShopCustomerList = _unitOfWork.ShopCustomer.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.CustomerName, Value = i.Id.ToString() });
    //        obj.RouteList = _unitOfWork.CustomerRoute.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.RouteName, Value = i.Id.ToString() });
    //        obj.TotalExpance = _unitOfWork.Expense.GetAll(x => x.ShopId == applicationUser.ShopId &&
    //                                                           x.IsDisable != true &&
    //                                                           x.IsDeleted != true &&
    //                                                           x.ApprovedDate >= obj.RegDateFrom &&
    //                                                           x.ApprovedDate <= obj.RegDateTo).Sum(x => x.Amount);
    //        obj.AccountReceivableList = _unitOfWork.AccountReceivable.FilterAccountReceivable(obj);
    //        obj.CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.IsDisable != true);

    //        if (obj.AccountReceivableList.Count() != 0)
    //        {
    //            obj.AccountReceivableVMList = obj.AccountReceivableList.OrderByDescending(u => u.Id).GroupBy(g => g.ShopCustomer.CustomerName).Select(s => new AccountReceivableVM()
    //            {
    //                ShopCustomerName = s.Key,
    //                ShopCustomerCode = s.FirstOrDefault().ShopCustomer.CustomerCode,
    //                ShopCustomerAddress = s.FirstOrDefault().ShopCustomer.CustomerCity,
    //                Payable = s.Sum(x => x.TotalReceivable),
    //                Paid = s.Where(x => x.ReceivedAmount >= 0).Sum(x => x.ReceivedAmount),
    //                OldBalance = s.Where(x => x.ReceivedAmount < 0).Sum(x => x.ReceivedAmount),
    //                Balance = s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount),
    //                TotalBalance = s.FirstOrDefault().ShopCustomer.Balance,
    //                PreviousBalance = s.FirstOrDefault().ShopCustomer.Balance - (s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount)),
    //                ShopCustomerId = s.FirstOrDefault().ShopCustomerId,
    //                Route = s.FirstOrDefault().ShopCustomer.CustomerRoute.RouteName,
    //                TermDay = s.FirstOrDefault().ShopCustomer.PaymentTermsDays
    //            }).ToList();
    //        }


    //        return View(obj);
    //    }
    //    public IActionResult CustomerSalesDetail(int CustomerId)
    //    {
    //        applicationUser ??= _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
    //        AccountReceivableVM obj = new AccountReceivableVM
    //        {
    //            ShopCustomerList = _unitOfWork.ShopCustomer.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.CustomerName, Value = i.Id.ToString() }),
    //            accountReceivable = new AccountReceivable()
    //        };
    //        obj.accountReceivable.ShopCustomerId = CustomerId;
    //        obj.ShopCustomerId = CustomerId;
    //        obj.AccountReceivableList = _unitOfWork.AccountReceivable.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true && x.ShopCustomerId == CustomerId, includeProperties: "SaleOrder,SaleOrder.Customer");
    //        if (!obj.AccountReceivableList.Any())
    //        {
    //            return RedirectToAction("CustomerCashList");
    //        }
    //        obj.CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.IsDisable != true);
    //        return View(obj);
    //    }
    //    public IActionResult SingleCustomerCashList(int CustomerId)
    //    {
    //        applicationUser ??= _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
    //        AccountReceivableVM obj = new()
    //        {
    //            ShopCustomerList = _unitOfWork.ShopCustomer.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.CustomerName, Value = i.Id.ToString() }),
    //            accountReceivable = new()
    //        };
    //        obj.accountReceivable.ShopCustomerId = CustomerId;
    //        obj.ShopCustomerId = CustomerId;
    //        obj.AccountReceivableList = _unitOfWork.AccountReceivable.GetAll(x =>
    //                                            x.ShopId == applicationUser.ShopId &&
    //                                            x.IsDisable != true &&
    //                                            x.IsDeleted != true &&
    //                                            x.ShopCustomerId == CustomerId, includeProperties: "SaleOrder,SaleOrder.Customer").ToList();
    //        if (obj.AccountReceivableList.Count() == 0)
    //        {
    //            return RedirectToAction("CustomerCashList");
    //        }
    //        obj.CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == applicationUser.ShopId && !x.IsDeleted && !x.IsDisable);
    //        return View(obj);
    //    }

    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public IActionResult SingleCustomerCashList(AccountReceivableVM obj)
    //    {
    //        applicationUser ??= _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);

    //        obj.ShopCustomerList = _unitOfWork.ShopCustomer
    //            .GetAll(x => x.ShopId == applicationUser.ShopId)
    //            .Select(i => new SelectListItem { Text = i.CustomerName, Value = i.Id.ToString() });

    //        obj.accountReceivable = new AccountReceivable();

    //        if (obj.ReceivedDateFrom != null)
    //        {
    //            obj.ReceivedDateTo ??= DateTime.Now;

    //            var endDate = obj.ReceivedDateTo.Value.AddDays(1);
    //            obj.AccountReceivableList = _unitOfWork.AccountReceivable
    //                .GetAll(x => x.ShopId == applicationUser.ShopId &&
    //                             !x.IsDisable &&
    //                             !x.IsDeleted &&
    //                             x.ReceivedDate >= obj.ReceivedDateFrom &&
    //                             x.ReceivedDate < endDate &&
    //                             x.ShopCustomerId == obj.ShopCustomerId,
    //                        includeProperties: "SaleOrder,SaleOrder.Customer")
    //                .ToList();
    //        }

    //        obj.CustomerDataList = _unitOfWork.ShopCustomer
    //            .GetAll(x => x.ShopId == applicationUser.ShopId && !x.IsDeleted && !x.IsDisable);

    //        return View(obj);
    //    }


    //    public IActionResult SingleCustomerSaleList(int CustomerId)
    //    {
    //        applicationUser ??= _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
    //        AccountReceivableVM obj = new AccountReceivableVM();
    //        obj.ShopCustomerList = _unitOfWork.ShopCustomer.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.CustomerName, Value = i.Id.ToString() });
    //        obj.accountReceivable = new AccountReceivable();
    //        obj.accountReceivable.ShopCustomerId = CustomerId;
    //        obj.ShopCustomerId = CustomerId;
    //        obj.AccountReceivableList = _unitOfWork.AccountReceivable.GetAll(x =>
    //                                            x.ShopId == applicationUser.ShopId &&
    //                                            x.IsDisable != true &&
    //                                            x.IsDeleted != true &&
    //                                            x.ShopCustomerId == CustomerId, includeProperties: "SaleOrder,SaleOrder.Customer").ToList();
    //        if (!obj.AccountReceivableList.Any())
    //        {
    //            return RedirectToAction("CustomerCashList");
    //        }
    //        obj.CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.IsDisable != true);
    //        return View(obj);
    //    }
    //    //POST
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public IActionResult UpsertPayCustomer(AccountReceivableVM obj)
    //    {
    //        if (obj.accountReceivable.Id == 0)
    //        {
    //            ShopCustomer shopCustomer = new ShopCustomer();
    //            if (obj.ShopCustomerCodeAndName != null)
    //            {
    //                if (obj.accountReceivable.ReceivedAmount == 0)
    //                {
    //                    TempData["error"] = "Provide received amount...";
    //                    return RedirectToAction(nameof(CustomerCashList));
    //                }
    //                string[] customer = obj.ShopCustomerCodeAndName.Split(",");
    //                shopCustomer = _unitOfWork.ShopCustomer.GetFirstOrDefault(u => u.CustomerCode == customer[0] && u.IsDisable != true);
    //                if (shopCustomer == null)
    //                {
    //                    TempData["error"] = "Provide existing customer...";
    //                    return RedirectToAction(nameof(CustomerCashList));
    //                }
    //                else
    //                {
    //                    obj.accountReceivable.ShopCustomerId = shopCustomer.Id;
    //                }
    //            }
    //            else
    //            {
    //                TempData["error"] = "Provide Customer...";
    //                return RedirectToAction(nameof(CustomerCashList));
    //            }


    //            obj.accountReceivable.CreatedBy = _userId;
    //            if (obj.accountReceivable.ReceivedDate == null)
    //            {
    //                obj.accountReceivable.ReceivedDate = DateTime.Now;
    //            }
    //            obj.accountReceivable.CreatedDate = DateTime.Now;
    //            obj.accountReceivable.ShopId = applicationUser.ShopId;
    //            double? dbBalance = 0;
    //            AccountReceivable accountReceivableDB = _unitOfWork.AccountReceivable.getLastAccountReceivable((int)obj.accountReceivable.ShopCustomerId);
    //            if (shopCustomer.Balance != null)
    //            {
    //                dbBalance = shopCustomer.Balance;
    //            }
    //            shopCustomer.Balance = obj.accountReceivable.TotalReceivable + dbBalance - obj.accountReceivable.ReceivedAmount;
    //            obj.accountReceivable.IsDisable = false;
    //            obj.accountReceivable.TotalReceivable = 0;
    //            obj.accountReceivable.IsDeleted = false;
    //            _unitOfWork.AccountReceivable.Add(obj.accountReceivable);
    //            TempData["success"] = "Cash received successfully";
    //        }
    //        _unitOfWork.Save();

    //        return RedirectToAction(nameof(CustomerCashList));
    //    }

    //    public IActionResult Delete(int? id)
    //    {
    //        if (id == null || id == 0)
    //        {
    //            return NotFound();
    //        }
    //        var FromDbFirst = _unitOfWork.AccountReceivable.GetFirstOrDefault(u => u.Id == id);

    //        if (FromDbFirst == null)
    //        {
    //            return NotFound();
    //        }

    //        return View(FromDbFirst);
    //    }

    //    //POST
    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public IActionResult DeletePOST(int? id)
    //    {
    //        var obj = _unitOfWork.AccountReceivable.GetFirstOrDefault(u => u.Id == id);
    //        if (obj == null)
    //        {
    //            return NotFound();
    //        }
    //        var coustmerDb = _unitOfWork.ShopCustomer.GetFirstOrDefault(u => u.Id == obj.ShopCustomerId);
    //        coustmerDb.Balance = coustmerDb.Balance + obj.ReceivedAmount;
    //        _unitOfWork.AccountReceivable.Remove(obj);
    //        _unitOfWork.Save();
    //        TempData["success"] = "Deleted successfully";
    //        return RedirectToAction("SingleCustomerCashList", new { CustomerId = obj.ShopCustomerId });
    //    }
    //}
}

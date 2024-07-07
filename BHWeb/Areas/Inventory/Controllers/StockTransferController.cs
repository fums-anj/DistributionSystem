using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.OrganizationManagement;
using BH.Models.ProductManagement;
using BH.Models.ViewModels;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BHWeb.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin)]
    public class StockTransferController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationUser applicationUser;
        public StockTransferController(IUnitOfWork unitOfWork)
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
            StockTransferVM stockTransferVM = new StockTransferVM();
            //stockTransferTotalList is stock list for stock balance which is not sold yet by using StockSoldQty
            stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Purchase || x.StockType == SDStockType.StockType_StockReturn), includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.CreatedDate >= DateTime.Now.Date, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.VariantId)
                .Select(s => new StockTransferVM()
                {
                    VariantId = s.Key,
                    Code = s.FirstOrDefault().Variant.VariantCode,
                    ReceivingQuantity = s.Where(x => x.ReceivedQty >= 0).Sum(x => x.ReceivedQty),
                    ReturningToSupplierQuantity = s.Where(x => x.ReceivedQty < 0).Sum(x => x.ReceivedQty),
                    SellingQuantity = s.Sum(x => x.SalesQty),
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    PurchaseStockIn = s.Where(x => x.ReceivedQty >= 0).Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    PurchaseStockReturn = s.Where(x => x.ReceivedQty < 0).Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    Product = s.FirstOrDefault().Variant.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    // Balance = stockTransferVM.stockTransferTotalList.Where(x => x.VariantId == s.Key).Sum(x => x.ReceivedQty - x.StockSoldQty),
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();

            return View(stockTransferVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Index(StockTransferVM? stockTransferVM)
        {
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.UnitOfMeasureList = new SelectList(_unitOfWork.UnitOfMeasure.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            //stockTransferTotalList is stock list for stock balance which is not sold yet by using StockSoldQty
            stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Purchase || x.StockType == SDStockType.StockType_StockReturn), includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true, includeProperties: "Variant,Variant.ShopProduct,UnitOfMeasure,Supplier");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.FilterStock(stockTransferVM);
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.VariantId)
                .Select(s => new StockTransferVM()
                {
                    VariantId = s.Key,
                    Code = s.FirstOrDefault().Variant.VariantCode,
                    ReceivingQuantity = s.Where(x => x.ReceivedQty >= 0).Sum(x => x.ReceivedQty),
                    ReturningToSupplierQuantity = s.Where(x => x.ReceivedQty < 0).Sum(x => x.ReceivedQty),
                    SellingQuantity = s.Sum(x => x.SalesQty),
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    PurchaseStockIn = s.Where(x => x.ReceivedQty >= 0).Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    PurchaseStockReturn = s.Where(x => x.ReceivedQty < 0).Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    Product = s.FirstOrDefault().Variant.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        public IActionResult IndexStockIn()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            StockTransferVM stockTransferVM = new StockTransferVM();
            //this stock list is for stock balance which is not sold yet
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Purchase || x.StockType == SDStockType.StockType_StockReturn) && x.ReceivedQty != x.StockSoldQty, includeProperties: "Variant,Variant.ShopProduct");
            //stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Purchase || x.StockType == SDStockType.StockType_StockReturn) && x.CreatedDate >= DateTime.Now.Date.AddDays(-15), includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase && x.CreatedDate >= DateTime.Now.Date.AddDays(-15), includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.VariantId)
                .Select(s => new StockTransferVM()
                {
                    VariantId = s.Key,
                    Code = s.FirstOrDefault().Variant.VariantCode,
                    ReceivingQuantity = s.Sum(x => x.ReceivedQty),
                    SellingQuantity = s.Sum(x => x.SalesQty),
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    PurchaseStockIn = s.Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    Product = s.FirstOrDefault().Variant.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    //Balance = s.Sum(x => x.ReceivedQty) - s.Sum(x => x.SalesQty),
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        //Temprorary Function created for sold quantity in purchasing stock and purchase price in selling stock
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult IndexStockIn(StockTransferVM? stockTransferVM)
        {
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.UnitOfMeasureList = new SelectList(_unitOfWork.UnitOfMeasure.GetAll(), "Id", "Name");
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            //this stock list is for stock balance which is not sold yet
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Purchase || x.StockType == SDStockType.StockType_StockReturn) && x.ReceivedQty != x.StockSoldQty, includeProperties: "Variant,Variant.ShopProduct");
            //stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Purchase || x.StockType == SDStockType.StockType_StockReturn), includeProperties: "Variant,Variant.ShopProduct,UnitOfMeasure,Supplier");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct,UnitOfMeasure,Supplier");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.FilterStock(stockTransferVM);
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.VariantId)
                .Select(s => new StockTransferVM()
                {
                    VariantId = s.Key,
                    Code = s.FirstOrDefault().Variant.VariantCode,
                    ReceivingQuantity = s.Sum(x => x.ReceivedQty),
                    SellingQuantity = s.Sum(x => x.SalesQty),
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    PurchaseStockIn = s.Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    Product = s.FirstOrDefault().Variant.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        public IActionResult IndexReturnToSupplier()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            StockTransferVM stockTransferVM = new StockTransferVM();
            //this stock list is for stock balance which is not sold yet
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Purchase || x.StockType == SDStockType.StockType_StockReturn) && x.ReceivedQty != x.StockSoldQty, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_StockReturn && x.CreatedDate >= DateTime.Now.Date.AddDays(-15), includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.VariantId)
                .Select(s => new StockTransferVM()
                {
                    VariantId = s.Key,
                    Code = s.FirstOrDefault().Variant.VariantCode,
                    ReturningToSupplierQuantity = s.Sum(x => x.ReceivedQty),
                    SellingQuantity = s.Sum(x => x.SalesQty),
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    PurchaseStockReturn = s.Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    Product = s.FirstOrDefault().Variant.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    //Balance = s.Sum(x => x.ReceivedQty) - s.Sum(x => x.SalesQty),
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult IndexReturnToSupplier(StockTransferVM? stockTransferVM)
        {
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.UnitOfMeasureList = new SelectList(_unitOfWork.UnitOfMeasure.GetAll(), "Id", "Name");
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            //this stock list is for stock balance which is not sold yet
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Purchase || x.StockType == SDStockType.StockType_StockReturn) && x.ReceivedQty != x.StockSoldQty, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Purchase || x.StockType == SDStockType.StockType_StockReturn), includeProperties: "Variant,Variant.ShopProduct,UnitOfMeasure,Supplier");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.FilterStock(stockTransferVM);
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.VariantId)
                .Select(s => new StockTransferVM()
                {
                    VariantId = s.Key,
                    Code = s.FirstOrDefault().Variant.VariantCode,
                    ReceivingQuantity = s.Sum(x => x.ReceivedQty),
                    SellingQuantity = s.Sum(x => x.SalesQty),
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    PurchaseStockIn = s.Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    Product = s.FirstOrDefault().Variant.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        public IActionResult StockTransferBySKU(int VariId)
        {
            StockTransferVM stockTransferVM = new StockTransferVM();
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(), "Id", "Name");
            stockTransferVM.UnitOfMeasureList = new SelectList(_unitOfWork.UnitOfMeasure.GetAll(), "Id", "Name");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.StockType == SDStockType.StockType_Sale && x.VariantId == VariId && x.IsDeleted != true && x.CreatedDate >= DateTime.Now.Date.AddDays(-15), includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.Variant = _unitOfWork.Variant.GetFirstOrDefault(x => x.Id == VariId);
            if (stockTransferVM.stockTransferList.Count() == 0)
            {
                TempData["error"] = "Variant not sold...";
            }
            return View(stockTransferVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult StockTransferBySKU(int? VariId, StockTransferVM? stockTransferVM)
        {
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(), "Id", "Name");
            stockTransferVM.UnitOfMeasureList = new SelectList(_unitOfWork.UnitOfMeasure.GetAll(), "Id", "Name");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.VariantId == VariId && x.IsDeleted != true, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.FilterStock(stockTransferVM);
            stockTransferVM.Variant = _unitOfWork.Variant.GetFirstOrDefault(x => x.Id == VariId);
            return View(stockTransferVM);
        }
        public IActionResult StockInBySKU(int VariId)
        {
            StockTransferVM stockTransferVM = new StockTransferVM();
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(), "Id", "Name");
            stockTransferVM.UnitOfMeasureList = new SelectList(_unitOfWork.UnitOfMeasure.GetAll(), "Id", "Name");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId &&
                                                                                      x.IsDisable != true &&
                                                                                      x.VariantId == VariId &&
                                                                                      x.IsDeleted != true &&
                                                                                      x.CreatedDate >= DateTime.Now.Date.AddDays(-30) &&
                                                                                      x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            if (stockTransferVM.stockTransferList.Count() == 0)
            {
                TempData["error"] = "Data not exists...";
            }
            stockTransferVM.Variant = _unitOfWork.Variant.GetFirstOrDefault(x => x.Id == VariId);
            return View(stockTransferVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult StockInBySKU(int? VariId, StockTransferVM? stockTransferVM)
        {
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(), "Id", "Name");
            stockTransferVM.UnitOfMeasureList = new SelectList(_unitOfWork.UnitOfMeasure.GetAll(), "Id", "Name");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId &&
                                                                                      x.IsDisable != true &&
                                                                                      x.VariantId == VariId &&
                                                                                      x.IsDeleted != true &&
                                                                                      x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.FilterStock(stockTransferVM);
            stockTransferVM.Variant = _unitOfWork.Variant.GetFirstOrDefault(x => x.Id == VariId);
            return View(stockTransferVM);
        }
        public IActionResult StockReturnByVariantId(int VariId)
        {
            StockTransferVM stockTransferVM = new StockTransferVM();
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(), "Id", "Name");
            stockTransferVM.UnitOfMeasureList = new SelectList(_unitOfWork.UnitOfMeasure.GetAll(), "Id", "Name");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId &&
                                                                                      x.IsDisable != true &&
                                                                                      x.VariantId == VariId &&
                                                                                      x.IsDeleted != true &&
                                                                                      x.CreatedDate >= DateTime.Now.Date.AddDays(-30) &&
                                                                                      x.StockType == SDStockType.StockType_StockReturn, includeProperties: "Variant,Variant.ShopProduct");
            if (stockTransferVM.stockTransferList.Count() == 0)
            {
                TempData["error"] = "Variant not sold...";
                return RedirectToAction(nameof(IndexStockIn));
            }
            return View(stockTransferVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult StockReturnByVariantId(int? VariId, StockTransferVM? stockTransferVM)
        {
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(), "Id", "Name");
            stockTransferVM.UnitOfMeasureList = new SelectList(_unitOfWork.UnitOfMeasure.GetAll(), "Id", "Name");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId &&
                                                                                      x.IsDisable != true &&
                                                                                      x.VariantId == VariId &&
                                                                                      x.IsDeleted != true &&
                                                                                      x.StockType == SDStockType.StockType_StockReturn, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.FilterStock(stockTransferVM);
            return View(stockTransferVM);
        }

        //GET
        public IActionResult Upsert(int? id, StockTransferVM? obj, string? Code = null)
        {
            //StockTransferVM obj = new();
            obj.VariantDataList = _unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.IsDisable != true);
            obj.StockTransfer = new();
            if (obj.StockTransfer.UnitPurchasePrice != 0)
            {
                //obj.StockTransfer = objSKU;
                obj.StockTransfer.Variant = _unitOfWork.Variant.GetFirstOrDefault(x => x.Id == obj.StockTransfer.VariantId);
            }
            obj.CatalogList = _unitOfWork.Catalog.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.ShopProductList = _unitOfWork.ShopProduct.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.VariantList = _unitOfWork.Variant.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.LocationList = _unitOfWork.Location.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.UnitOfMeasureList = _unitOfWork.UnitOfMeasure.GetAll().Where(x => x.IsDisable != true ).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.SupplierList = _unitOfWork.Supplier.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            if (id == null || id == 0)
            {
                return View(obj);
            }
            else
            {
                obj.StockTransfer = _unitOfWork.StockTransfer.GetFirstOrDefault(u => u.Id == id);
                return View(obj);
            }
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(StockTransferVM obj, string? StockType)
        {
            obj.VariantDataList = _unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.IsDisable != true);
            //obj.StockTransfer = new();
            if (obj.StockTransfer.UnitPurchasePrice != 0)
            {
                obj.StockTransfer.Variant = _unitOfWork.Variant.GetFirstOrDefault(x => x.Id == obj.StockTransfer.VariantId);
            }
            obj.CatalogList = _unitOfWork.Catalog.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.ShopProductList = _unitOfWork.ShopProduct.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.VariantList = _unitOfWork.Variant.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.LocationList = _unitOfWork.Location.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.UnitOfMeasureList = _unitOfWork.UnitOfMeasure.GetAll().Where(x => x.IsDisable != true).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            obj.SupplierList = _unitOfWork.Supplier.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() });
            if (obj.StockTransfer.Id == 0)
            {
                Variant variantFromDB = new();
                if (obj.SKU != null)
                {
                    string[] sku = obj.SKU.Split(",");
                    variantFromDB = _unitOfWork.Variant.GetFirstOrDefault(u => u.SKU == obj.SKU || u.PackingSKU == obj.SKU || u.VariantCode == sku[0], includeProperties: "ShopProduct");
                    if (variantFromDB == null)
                    {
                        TempData["error"] = "Variant not exists...";
                        return RedirectToAction("Upsert");
                    }
                    obj.SKU = null;
                    obj.StockTransfer.Variant = new();
                    obj.StockTransfer.Variant.ShopProduct = new();
                    obj.StockTransfer.Variant.ShopProductId = variantFromDB.ShopProductId;
                    obj.StockTransfer.Variant.ShopProduct.CatalogId = variantFromDB.ShopProduct.CatalogId;
                    obj.StockTransfer.UnitOfMeasureId = variantFromDB.UnitOfMeasureId;
                    obj.StockTransfer.VariantId = variantFromDB.Id;
                    obj.StockTransfer.ReceivingDate = DateTime.Now;
                    obj.StockTransfer.LocationId = variantFromDB.LocationId;
                    obj.StockTransfer.UnitPurchasePrice = (double)variantFromDB.PurchasePrice;
                    obj.StockTransfer.UnitSellingPrice = (double)variantFromDB.ListPrice;
                    obj.AvailableQty = _unitOfWork.StockTransfer.GetStockQuantity(obj.StockTransfer.VariantId, (int)applicationUser.ShopId);
                    if (obj.SKU == variantFromDB.PackingSKU)
                    {
                        obj.StockTransfer.UnitOfMeasureId = _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Name == SDUnits.Unit_packing).Id;
                    }
                    //return RedirectToAction("Upsert", obj);
                    return View(obj);
                }
                if (obj.StockTransfer.ReceivedQty == 0)
                {
                    TempData["error"] = "Quntity must be greater than zero";
                    return RedirectToAction("Upsert");
                }
                if (obj.SKU == null)
                {
                    variantFromDB = _unitOfWork.Variant.GetFirstOrDefault(u => u.Id == obj.StockTransfer.VariantId, includeProperties: "ShopProduct");
                }
                if (obj.StockTransfer.UnitOfMeasureId == _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Name == SDUnits.Unit_packing).Id)
                {
                    obj.StockTransfer.ReceivedQty = obj.StockTransfer.ReceivedQty * variantFromDB.Packing;
                }
                else
                {
                    UnitOfMeasure unitOfMeasure = _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Id == obj.StockTransfer.UnitOfMeasureId);
                    obj.StockTransfer.ReceivedQty = obj.StockTransfer.ReceivedQty * unitOfMeasure.ValueInBaseUnit;
                }
                obj.StockTransfer.CreatedBy = _userId;
                obj.StockTransfer.CreatedDate = DateTime.UtcNow;
                obj.StockTransfer.ShopId = applicationUser.ShopId;
                obj.StockTransfer.IsDisable = false;
                obj.StockTransfer.Variant = null;
                obj.StockTransfer.SalesQty = 0;
                if (obj.StockTransfer.UnitPurchasePrice <= 0)
                {
                    obj.StockTransfer.UnitPurchasePrice = (double)variantFromDB.PurchasePrice;
                }
                if (StockType == SDStockType.StockType_StockReturn)
                {
                    if (obj.StockTransfer.ReceivedQty > _unitOfWork.StockTransfer.GetStockQuantity(obj.StockTransfer.VariantId, (int)applicationUser.ShopId))
                    {
                        TempData["success"] = "You have not this quantity in Stock...";
                        return RedirectToAction("Upsert");
                    }
                    obj.StockTransfer.ReceivedQty = -obj.StockTransfer.ReceivedQty;
                    obj.StockTransfer.StockType = SDStockType.StockType_StockReturn;
                    TempData["success"] = "Stock returned successfully";
                }
                else if (StockType == SDStockType.StockType_SaleReturn)
                {
                    obj.StockTransfer.StockType = SDStockType.StockType_SaleReturn;
                    obj.StockTransfer.SalesQty = -obj.StockTransfer.ReceivedQty;
                    obj.StockTransfer.ReceivedQty = 0;
                    obj.StockTransfer.UnitPurchasePrice = 0;
                    if (obj.StockTransfer.UnitSellingPrice <= 0)
                    {
                        obj.StockTransfer.UnitSellingPrice = (double)variantFromDB.ListPrice;
                        TempData["success"] = "Sale returned successfully";
                    }
                }
                else
                {
                    obj.StockTransfer.StockType = SDStockType.StockType_Purchase;
                    TempData["success"] = "Stock add successfully";

                }
                _unitOfWork.StockTransfer.Add(obj.StockTransfer);
            }
            else
            {
                obj.StockTransfer.ModifiedBy = _userId;
                obj.StockTransfer.ModifiedDate = DateTime.Now;
                obj.StockTransfer.Variant = null;
                _unitOfWork.StockTransfer.Update(obj.StockTransfer);
                TempData["success"] = "Stock updated successfully";
            }
            _unitOfWork.Save();

            return RedirectToAction("Upsert");
            //   return View(obj);
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var FromDbFirst = _unitOfWork.StockTransfer.GetFirstOrDefault(u => u.Id == id);

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
            var obj = _unitOfWork.StockTransfer.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.StockTransfer.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Stock deleted successfully";
            return RedirectToAction("Index");
        }
        #region API CALLS
        [Authorize]
        public IActionResult GetShopProductList(int Id)
        {
            DDList ddList = new DDList();
            ddList.Items.Add(new SelectListItem { Text = "- Select -", Value = "0" });
            ddList.Items.AddRange(_unitOfWork.ShopProduct.GetAll().Where(x => x.ShopId == applicationUser.ShopId && x.CatalogId == Id).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }));
            return PartialView("~/Views/Shared/_DDListPartial.cshtml", ddList);
        }
        [Authorize]
        public IActionResult GetVariantList(int Id)
        {
            DDList ddList = new DDList();
            ddList.Items.Add(new SelectListItem { Text = "- Select -", Value = "0" });
            ddList.Items.AddRange(_unitOfWork.Variant.GetAll().Where(x => x.ShopId == applicationUser.ShopId && x.ShopProductId == Id).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }));
            return PartialView("~/Views/Shared/_DDListPartial.cshtml", ddList);
        }
        [Authorize]
        public IActionResult GetUnitOfMeasureList(int Id)
        {
            DDList ddList = new DDList();
            ddList.Items.Add(new SelectListItem { Text = "- Select -", Value = "0" });
            Variant variantDB = _unitOfWork.Variant.GetFirstOrDefault(x => x.ShopId == applicationUser.ShopId && x.Id == Id);
            if (variantDB.IsWeight == true)
            {
                UnitOfMeasure unitOfMeasureDb = _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Name == SDUnits.Unit_packing);
                ddList.Items.Add(new SelectListItem { Text = unitOfMeasureDb.Name, Value = unitOfMeasureDb.Id.ToString() });
                ddList.Items.AddRange(_unitOfWork.UnitOfMeasure.GetAll().Where(x => x.ShopId == applicationUser.ShopId && x.IsWaight == true).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }));
            }
            else
            {
                ddList.Items.AddRange(_unitOfWork.UnitOfMeasure.GetAll().Where(x => x.ShopId == applicationUser.ShopId && x.IsWaight == false).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }));
            }
            return PartialView("~/Views/Shared/_DDListPartial.cshtml", ddList);
        }
        #endregion
        ////Temprorary Function created for sold quantity in purchasing stock and purchase price in selling stock
        //void managepurchasesoldstock(ShopCart cart)
        //{
        //    while (cart.Quantity != 0)
        //    {
        //        StockTransfer stockTransferDB = new StockTransfer();
        //        double cartRemainingQty = cart.Quantity;
        //        if (cart.Variant.IsWithoutStock == false)
        //        {
        //            stockTransferDB = _unitOfWork.StockTransfer.GetFirstOrDefault(x => x.VariantId == cart.VariantId && x.ReceivedQty != x.StockSoldQty && x.StockType == SDStockType.StockType_Purchase);
        //            if (stockTransferDB != null)
        //            {
        //                if (cartRemainingQty > 0)
        //                {
        //                    if (cart.Quantity > (stockTransferDB.ReceivedQty - stockTransferDB.StockSoldQty))
        //                    {
        //                        cart.Quantity = stockTransferDB.ReceivedQty - stockTransferDB.StockSoldQty;
        //                    }
        //                }
        //                //Manage Sold Quantity in Purchase Stock
        //                stockTransferDB.StockSoldQty = stockTransferDB.StockSoldQty + cart.Quantity;
        //                _unitOfWork.StockTransfer.Update(stockTransferDB);
        //                _unitOfWork.Save();
        //                //managepsalesoldstock(cart.Quantity, cart.VariantId, stockTransferDB.UnitPurchasePrice, stockTransferDB.Id);
        //            }
        //        }

        //        cart.Quantity = cartRemainingQty - cart.Quantity;
        //        stockTransferDB = null;
        //    }
        //}

        //void managepurchasesoldstockNothingSold(ShopCart cart)
        //{
        //    while (cart.Quantity != 0)
        //    {
        //        StockTransfer stockTransferDB = new StockTransfer();
        //        double cartRemainingQty = cart.Quantity;
        //        if (cart.Variant.IsWithoutStock == false)
        //        {
        //            stockTransferDB = _unitOfWork.StockTransfer.GetFirstOrDefault(x => x.VariantId == cart.VariantId && x.ReceivedQty == x.StockSoldQty && x.StockType == SDStockType.StockType_Purchase);
        //            if (stockTransferDB != null)
        //            {
        //                if (cartRemainingQty > 0)
        //                {
        //                    if (cart.Quantity > (stockTransferDB.ReceivedQty - stockTransferDB.StockSoldQty))
        //                    {
        //                        cart.Quantity = stockTransferDB.ReceivedQty - stockTransferDB.StockSoldQty;
        //                    }
        //                }
        //                //Manage Sold Quantity in Purchase Stock
        //                stockTransferDB.StockSoldQty = 0;
        //                _unitOfWork.StockTransfer.Update(stockTransferDB);
        //                _unitOfWork.Save();
        //                //managepsalesoldstock(cart.Quantity, cart.VariantId, stockTransferDB.UnitPurchasePrice, stockTransferDB.Id);
        //            }
        //        }

        //        cart.Quantity = cartRemainingQty - cart.Quantity;
        //        stockTransferDB = null;
        //    }
        //}

        ////Temprorary Function created for sold quantity in purchasing stock and purchase price in selling stock
        //void managepsalesoldstock(double qty, int? varId, double purchase, int purchaseId)
        //{
        //    while (qty != 0)
        //    {
        //        StockTransfer stockTransferSaleDB = new StockTransfer();
        //        SaleOrderLine saleOrderLineDB = new SaleOrderLine();
        //        double cartRemainingSaleQty = qty;
        //        stockTransferSaleDB = _unitOfWork.StockTransfer.GetFirstOrDefault(x => x.VariantId == varId && x.SalesQty <= qty && x.UnitPurchasePrice == 0 && x.StockType == SDStockType.StockType_Sale);

        //        if (stockTransferSaleDB != null)
        //        {
        //            if (cartRemainingSaleQty > 0)
        //            {
        //                if (qty > stockTransferSaleDB.SalesQty)
        //                {
        //                    qty = stockTransferSaleDB.SalesQty;
        //                }
        //            }
        //            //Manage Sold Quantity in Purchase Stock
        //            stockTransferSaleDB.UnitPurchasePrice = purchase;
        //            _unitOfWork.StockTransfer.Update(stockTransferSaleDB);
        //            saleOrderLineDB = _unitOfWork.SaleOrderLine.GetFirstOrDefault(x => x.StockTransferId == stockTransferSaleDB.Id);
        //            saleOrderLineDB.PurchaseStockId = purchaseId;
        //            _unitOfWork.SaleOrderLine.Update(saleOrderLineDB);
        //            _unitOfWork.Save();
        //        }

        //        qty = cartRemainingSaleQty - qty;
        //        stockTransferSaleDB = null;
        //        saleOrderLineDB = null;
        //    }
        //}
    }
}

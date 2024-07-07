using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.AccountManagement;
using BH.Models.InventoryManagement;
using BH.Models.OrganizationManagement;
using BH.Models.ProductManagement;
using BH.Models.ViewModels;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BHWeb.Areas.Reporting.Controllers
{
    [Area("Reporting")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin)]
    public class ReportingController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private ApplicationUser applicationUser;
        public ReportingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
        }

        public IActionResult IndexVariantReport()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            StockTransferVM stockTransferVM = new StockTransferVM();
            //stockTransferTotalList is stock list for stock balance which is not sold yet by using StockSoldQty
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.CreatedDate >= DateTime.Now.Date.AddDays(-30), includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure");
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
                    UOM = s.FirstOrDefault().Variant.UnitOfMeasure.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult IndexVariantReport(StockTransferVM? stockTransferVM)
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
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.IsDeleted != true, includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure,UnitOfMeasure,Supplier");
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
                    UOM = s.FirstOrDefault().Variant.UnitOfMeasure.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        public IActionResult IndexVariantDayReport()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            StockTransferVM stockTransferVM = new StockTransferVM();
            //stockTransferTotalList is stock list for stock balance which is not sold yet by using StockSoldQty
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.CreatedDate == DateTime.Now.Date, includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure");
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
                    UOM = s.FirstOrDefault().Variant.UnitOfMeasure.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult IndexVariantDayReport(StockTransferVM? stockTransferVM)
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
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.IsDeleted != true, includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure,UnitOfMeasure,Supplier");
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
                    UOM = s.FirstOrDefault().Variant.UnitOfMeasure.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        public IActionResult IndexProductReport()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            StockTransferVM stockTransferVM = new StockTransferVM();
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.CreatedDate >= DateTime.Now.Date.AddDays(-30), includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure");
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.Variant.ShopProductId)
                .Select(s => new StockTransferVM()
                {
                    ProductId = s.Key,
                    ReceivingQuantity = s.Sum(x => x.ReceivedQty),
                    SellingQuantity = s.Sum(x => x.SalesQty),
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    PurchaseStockIn = s.Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    Product = s.FirstOrDefault().Variant.ShopProduct.Name,
                    UOM = s.FirstOrDefault().Variant.UnitOfMeasure.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult IndexProductReport(StockTransferVM? stockTransferVM)
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
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.IsDeleted != true, includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure,Supplier");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.FilterStock(stockTransferVM);
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.Variant.ShopProductId)
                .Select(s => new StockTransferVM()
                {
                    ProductId = s.Key,
                    ReceivingQuantity = s.Sum(x => x.ReceivedQty),
                    SellingQuantity = s.Sum(x => x.SalesQty),
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    PurchaseStockIn = s.Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    Product = s.FirstOrDefault().Variant.ShopProduct.Name,
                    UOM = s.FirstOrDefault().Variant.UnitOfMeasure.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        public IActionResult IndexDateWiseSaleReport()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            StockTransferVM stockTransferVM = new StockTransferVM();
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.CreatedDate >= DateTime.Now.Date.AddDays(-30), includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure");
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.CreatedDate.Value.Date)
                .Select(s => new StockTransferVM()
                {
                    CreatedDate = s.Key,
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult IndexDateWiseSaleReport(StockTransferVM? stockTransferVM)
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
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.IsDeleted != true, includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure,Supplier");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.FilterStock(stockTransferVM);
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.CreatedDate.Value.Date)
                .Select(s => new StockTransferVM()
                {
                    CreatedDate = s.Key,
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        public IActionResult IndexHourWiseSaleReport()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            StockTransferVM stockTransferVM = new StockTransferVM();
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.CreatedDate >= DateTime.Now.Date.AddDays(-30), includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure");
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.CreatedDate.Value.Hour)
                .Select(s => new StockTransferVM()
                {
                    Hour = s.Key,
                    CreatedDate = s.FirstOrDefault().CreatedDate.Value.Date,
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult IndexHourWiseSaleReport(StockTransferVM? stockTransferVM)
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
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.IsDeleted != true, includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure,Supplier");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.FilterStock(stockTransferVM);
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.CreatedDate.Value.Hour)
                .Select(s => new StockTransferVM()
                {
                    Hour = s.Key,
                    CreatedDate = s.FirstOrDefault().CreatedDate,
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        public IActionResult IndexUserWiseSaleReport()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            StockTransferVM stockTransferVM = new StockTransferVM();
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.CreatedDate >= DateTime.Now.Date.AddDays(-30), includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure");
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.CreatedBy)
                .Select(s => new StockTransferVM()
                {
                    User = s.FirstOrDefault().ApplicationUser.FullName,
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult IndexUserWiseSaleReport(StockTransferVM? stockTransferVM)
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
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.IsDeleted != true, includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure,Supplier");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.FilterStock(stockTransferVM);
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.CreatedBy)
                .Select(s => new StockTransferVM()
                {
                    User = s.FirstOrDefault().ApplicationUser.FullName,
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        public IActionResult IndexMarginReport()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            StockTransferVM stockTransferVM = new StockTransferVM();
            //stockTransferTotalList is stock list for stock balance which is not sold yet by using StockSoldQty
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.CreatedDate >= DateTime.Now.Date.AddDays(-30), includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure");
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
                    UOM = s.FirstOrDefault().Variant.UnitOfMeasure.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult IndexMarginReport(StockTransferVM? stockTransferVM)
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
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && (x.StockType == SDStockType.StockType_Sale || x.StockType == SDStockType.StockType_SaleReturn) && x.IsDeleted != true, includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure,UnitOfMeasure,Supplier");
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
                    UOM = s.FirstOrDefault().Variant.UnitOfMeasure.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }
        public IActionResult IndexStockMovmentReport()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            StockTransferVM stockTransferVM = new StockTransferVM();
            //stockTransferTotalList is stock list for stock balance which is not sold yet by using StockSoldQty
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.CreatedDate >= DateTime.Now.Date.AddDays(-30), includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure");
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.VariantId)
                .Select(s => new StockTransferVM()
                {
                    VariantId = s.Key,
                    Code = s.FirstOrDefault().Variant.VariantCode,
                    ReceivingQuantity = s.Where(x => x.ReceivedQty >= 0).Sum(x => x.ReceivedQty),
                    ReturningToSupplierQuantity = s.Where(x => x.ReceivedQty < 0).Sum(x => x.ReceivedQty),
                    SellingQuantity = s.Sum(x => x.SalesQty),
                    Purchase = s.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                    PurchaseStockIn = s.Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    Product = s.FirstOrDefault().Variant.Name,
                    UOM = s.FirstOrDefault().Variant.UnitOfMeasure.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult IndexStockMovmentReport(StockTransferVM? stockTransferVM)
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
            //stockTransferVM.stockTransferTotalList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.StockType == SDStockType.StockType_Purchase, includeProperties: "Variant,Variant.ShopProduct");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true, includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure,UnitOfMeasure,Supplier");
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
                    PurchaseStockIn = s.Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                    Sale = s.Sum(x => x.UnitSellingPrice * x.SalesQty),
                    Product = s.FirstOrDefault().Variant.Name,
                    UOM = s.FirstOrDefault().Variant.UnitOfMeasure.Name,
                    LowStockQty = s.FirstOrDefault().Variant.LowStockWarningQuantity,
                    Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key, (int)applicationUser.ShopId),
                    ProfitLoss = s.Sum(x => x.UnitSellingPrice * x.SalesQty) - s.Sum(x => x.UnitPurchasePrice * x.SalesQty)
                }).ToList();
            return View(stockTransferVM);
        }

        public IActionResult IndexCustomerWiseVariantStockReport()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            StockTransferVM stockTransferVM = new StockTransferVM();
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x =>
                                                                                 x.ShopId == applicationUser.ShopId &&
                                                                                 x.ShopCustomerId != null &&
                                                                                 x.IsDeleted != true && (
                                                                                 x.StockType == SDStockType.StockType_Sale ||
                                                                                 x.StockType == SDStockType.StockType_SaleReturn) &&
                                                                                 x.CreatedDate >= DateTime.Now.Date.AddDays(-30), includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure,ShopCustomer,ShopCustomer.CustomerRoute");
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.ShopCustomer)
                .Select(s => new StockTransferVM()
                {

                    Product = s.FirstOrDefault().Variant.Name,
                    ShopCustomer = s.Key,
                    stockTransferVMs = s.GroupBy(v => v.Variant).Select(v => new StockTransferVM()
                    {
                        Variant = v.Key,
                        SellingQuantity = v.Sum(x => x.SalesQty),
                        Purchase = v.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                        PurchaseStockIn = v.Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                        Sale = v.Sum(x => x.UnitSellingPrice * x.SalesQty),
                        UOM = v.FirstOrDefault().Variant.UnitOfMeasure.Name,
                        LowStockQty = v.FirstOrDefault().Variant.LowStockWarningQuantity,
                        Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key.Id, (int)applicationUser.ShopId),
                        ProfitLoss = v.Sum(x => x.UnitSellingPrice * x.SalesQty) - v.Sum(x => x.UnitPurchasePrice * x.SalesQty)

                    }).ToList(),

                }).ToList();
            return View(stockTransferVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult IndexCustomerWiseVariantStockReport(StockTransferVM? stockTransferVM)
        {
            stockTransferVM.SupplierList = new SelectList(_unitOfWork.Supplier.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.VariantList = new SelectList(_unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            stockTransferVM.UserList = new SelectList(_unitOfWork.ApplicationUser.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "FullName");
            stockTransferVM.UnitOfMeasureList = new SelectList(_unitOfWork.UnitOfMeasure.GetAll(x => x.ShopId == applicationUser.ShopId), "Id", "Name");
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true, includeProperties: "Variant,Variant.ShopProduct,Variant.UnitOfMeasure,UnitOfMeasure,Supplier,ShopCustomer.CustomerRoute");
            stockTransferVM.stockTransferList = _unitOfWork.StockTransfer.FilterStock(stockTransferVM);
            stockTransferVM.stockTransferVMs = stockTransferVM.stockTransferList.GroupBy(g => g.ShopCustomer)
                .Select(s => new StockTransferVM()
                {

                    Product = s.FirstOrDefault().Variant.Name,
                    ShopCustomer = s.Key,
                    stockTransferVMs = s.GroupBy(v => v.Variant).Select(v => new StockTransferVM()
                    {
                        Variant = v.Key,
                        SellingQuantity = v.Sum(x => x.SalesQty),
                        Purchase = v.Sum(x => x.UnitPurchasePrice * x.SalesQty),
                        PurchaseStockIn = v.Sum(x => x.UnitPurchasePrice * x.ReceivedQty),
                        Sale = v.Sum(x => x.UnitSellingPrice * x.SalesQty),
                        UOM = v.FirstOrDefault().Variant.UnitOfMeasure.Name,
                        LowStockQty = v.FirstOrDefault().Variant.LowStockWarningQuantity,
                        Balance = _unitOfWork.StockTransfer.GetStockQuantity(s.Key.Id, (int)applicationUser.ShopId),
                        ProfitLoss = v.Sum(x => x.UnitSellingPrice * x.SalesQty) - v.Sum(x => x.UnitPurchasePrice * x.SalesQty)

                    }).ToList(),

                }).ToList();
            return View(stockTransferVM);
        }

        public IActionResult CustomerCashDateWiseReport()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            AccountReceivableVM obj = new AccountReceivableVM();
            obj.accountReceivable = new AccountReceivable();
            obj.RouteList = _unitOfWork.CustomerRoute.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.RouteName, Value = i.Id.ToString() });
            obj.AccountReceivableList = _unitOfWork.AccountReceivable.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true && x.ShopCustomerId != null && x.CreatedDate >= DateTime.Now.Date.AddDays(-30), includeProperties: "ShopCustomer,ShopCustomer.CustomerRoute");

            obj.AccountReceivableVMList = obj.AccountReceivableList.OrderByDescending(u => u.Id)
                                                                   .GroupBy(g => g.CreatedDate.Value.Date)
                                                                   .Select(s => new AccountReceivableVM()
                                                                   {
                                                                       CreatedDate = s.Key,
                                                                       Payable = s.Sum(x => x.TotalReceivable),
                                                                       Paid = s.Where(x => x.ReceivedAmount >= 0).Sum(x => x.ReceivedAmount),
                                                                       OldBalance = s.Where(x => x.ReceivedAmount < 0).Sum(x => x.ReceivedAmount),
                                                                       Balance = s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount),
                                                                       TotalBalance = s.Sum(x => x.ShopCustomer.Balance),
                                                                       PreviousBalance = s.Sum(x => x.ShopCustomer.Balance) - (s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount)),
                                                                       Route = s.FirstOrDefault().ShopCustomer.CustomerRoute.RouteName,
                                                                       TermDay = s.FirstOrDefault().ShopCustomer.PaymentTermsDays,
                                                                       TotalExpance = _unitOfWork.Expense.GetAll(x => x.ShopId == applicationUser.ShopId &&
                                                                                                                      x.IsDisable != true &&
                                                                                                                      x.IsDeleted != true &&
                                                                                                                      x.CreatedDate >= s.Key &&
                                                                                                                      x.CreatedDate < s.Key.AddDays(1)).Sum(x => x.Amount),
                                                                       VistedShops = s.Count()
                                                                   }).ToList();


            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult CustomerCashDateWiseReport(AccountReceivableVM obj)
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            obj.accountReceivable = new AccountReceivable();
            obj.User = applicationUser;
            obj.ShopCustomerList = _unitOfWork.ShopCustomer.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.CustomerName, Value = i.Id.ToString() });
            obj.RouteList = _unitOfWork.CustomerRoute.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.RouteName, Value = i.Id.ToString() });
            obj.AccountReceivableList = _unitOfWork.AccountReceivable.FilterAccountReceivable(obj);
            obj.CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.IsDisable != true);

            if (obj.AccountReceivableList.Count() != 0)
            {
                obj.AccountReceivableVMList = obj.AccountReceivableList.OrderByDescending(u => u.Id)
                                                                   .GroupBy(g => g.CreatedDate.Value.Date)
                                                                   .Select(s => new AccountReceivableVM()
                                                                   {
                                                                       CreatedDate = s.Key,
                                                                       Payable = s.Sum(x => x.TotalReceivable),
                                                                       Paid = s.Where(x => x.ReceivedAmount >= 0).Sum(x => x.ReceivedAmount),
                                                                       OldBalance = s.Where(x => x.ReceivedAmount < 0).Sum(x => x.ReceivedAmount),
                                                                       Balance = s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount),
                                                                       TotalBalance = s.Sum(x => x.ShopCustomer.Balance),
                                                                       PreviousBalance = s.Sum(x => x.ShopCustomer.Balance) - (s.Sum(x => x.TotalReceivable) - s.Sum(x => x.ReceivedAmount)),
                                                                       Route = s.FirstOrDefault().ShopCustomer.CustomerRoute.RouteName,
                                                                       TermDay = s.FirstOrDefault().ShopCustomer.PaymentTermsDays,
                                                                       TotalExpance = _unitOfWork.Expense.GetAll(x => x.ShopId == applicationUser.ShopId &&
                                                                                                                      x.IsDisable != true &&
                                                                                                                      x.IsDeleted != true &&
                                                                                                                      x.CreatedDate >= s.Key &&
                                                                                                                      x.CreatedDate < s.Key.AddDays(1)).Sum(x => x.Amount),
                                                                       VistedShops = s.Count()
                                                                   }).ToList();
            }


            return View(obj);
        }
        public IActionResult RouteBalanceReport()
        {
            if (applicationUser == null)
            {
                applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            }
            RouteVM obj = new RouteVM();
            obj.ShopCustomerList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true && x.CustomerRouteId != null, includeProperties: "CustomerRoute");

            obj.RouteListVM = obj.ShopCustomerList.OrderByDescending(u => u.Id)
                                                                   .GroupBy(g => g.CustomerRoute)
                                                                   .Select(s => new RouteVM()
                                                                   {
                                                                       CustomerRoute = s.Key,
                                                                       RouteBalance = (decimal)s.Sum(x => x.Balance),
                                                                       customerCount = s.Count()
                                                                   }).ToList();


            return View(obj);
        }
    }
}

using BH.DataAccess.Infrastructure.Interface.IRepository;
using BH.Models.AccountManagement;
using BH.Models.CustomerManagement;
using BH.Models.InventoryManagement;
using BH.Models.OrganizationManagement;
using BH.Models.ProductManagement;
using BH.Models.SaleManagement;
using BH.Models.ViewModels;
using BH.Utility;
using BHWeb.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace BHWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SDRoles.Role_Admin + "," + SDRoles.Role_ShopAdmin + "," + SDRoles.Role_ShopUser + "," + SDRoles.Role_Saleman)]
    public class POSController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private ApplicationUser applicationUser;
        [BindProperty]
        public ShopCartVM ShopCartVM { get; set; }
        [BindProperty]
        public SaleOrderVM SaleOrderVM { get; set; }
        public CreditNote CreditNote { get; set; }
        public POSController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
        }
        // WC = WholeSale, ReLoadOrderId = SaleOrderId, CustomerDetail = CustomerId, retainOrderDate = OrderDate if these values are set other wise will be null
        public IActionResult Index(string? WC, int? ReLoadOrderId, string? CustomerDetail, DateTime? retainOrderDate)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == _userId);
            ShopCartVM = new ShopCartVM();
            ShopCartVM.ShopCart = new();

            ///Reload Block, If EditSaleOrderId has a value then it will load SaleOrderLine data of given SaleOrder Id to ListCart
            #region Reload Order
            if (ReLoadOrderId != null)
            {
                SaleOrderVM = new SaleOrderVM();
                SaleOrderVM.SaleOrder = new SaleOrder();
                AccountReceivable accountReceivable = new AccountReceivable();

                SaleOrderVM.SaleOrder = _unitOfWork.SaleOrder.GetFirstOrDefault(u => u.Id == ReLoadOrderId);
                accountReceivable = _unitOfWork.AccountReceivable.GetFirstOrDefault(u => u.SaleOrderId == ReLoadOrderId);
                SaleOrderVM.SaleOrderLineList = _unitOfWork.SaleOrderLine.GetAll(u => u.SaleOrderId == ReLoadOrderId, includeProperties: "StockTransfer,Variant");
                foreach (var item in SaleOrderVM.SaleOrderLineList)
                {
                    //if (item.StockTransfer.SalesQty < 0)
                    //{
                    //    TempData["error"] = "Returned stock could not be Reloaded...";
                    //    return RedirectToAction(nameof(Index));
                    //}

                    OrderReload(item);

                }
                if (accountReceivable != null)
                {
                    ShopCustomer shopCustomerDB = _unitOfWork.ShopCustomer.GetFirstOrDefault(u => u.Id == accountReceivable.ShopCustomerId);
                    shopCustomerDB.Balance = shopCustomerDB.Balance + (accountReceivable.ReceivedAmount - accountReceivable.TotalReceivable);
                    _unitOfWork.AccountReceivable.Remove(accountReceivable);
                }
                _unitOfWork.SaleOrder.Remove(SaleOrderVM.SaleOrder);
                _unitOfWork.SaleOrderLine.RemoveRange(SaleOrderVM.SaleOrderLineList);
                _unitOfWork.Save();
            }
            #endregion
            ///End of the Reload block


            ShopCartVM.ListCart = _unitOfWork.ShopCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Variant").OrderByDescending(o => o.Id);
            ShopCartVM.OrderHeader = new();
            ShopCartVM.StockTransfer = new();
            ShopCartVM.VariantDataList = _unitOfWork.Variant.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.IsDisable != true);
            ShopCartVM.CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.IsDisable != true);
            ShopCartVM.UnitOfMeasureList = new List<SelectListItem>();
            ShopCartVM.UnitOfMeasureList.AddRange(_unitOfWork.UnitOfMeasure.GetAll().Where(x => x.IsDisable == false).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }));
            ShopCartVM.ShopCart.Quantity = 1;
            if (!string.IsNullOrEmpty(WC))
            {
                ShopCartVM.ShopCart.IsWholesale = true;
            }
            if (!string.IsNullOrEmpty(CustomerDetail))
            {
                ShopCartVM.ShopCustomerId = CustomerDetail;
            }
            if (retainOrderDate != null)
            {
                ShopCartVM.OrderDate = retainOrderDate;
            }
            else
            {
                ShopCartVM.OrderDate = DateTime.Now;
            }
            foreach (var cart in ShopCartVM.ListCart)
            {
                ShopCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Quantity);
            }
            return View(ShopCartVM);
        }

        private void OrderReload(SaleOrderLine saleOrderLine)
        {
            //This loop will decrease StockSold quantity of purchased Stock and it will iterate when quantity of cart is remaining 
            double tempcartQty = saleOrderLine.StockTransfer.SalesQty;
            while (tempcartQty != 0)
            {
                //Get the StockTransfer form database to update the Stock Sold attribute
                IEnumerable<StockTransfer> stockTransferListDB = _unitOfWork.StockTransfer.GetAll(x => x.VariantId == saleOrderLine.VariantId && x.StockSoldQty != 0 && x.StockType == SDStockType.StockType_Purchase);
                StockTransfer stockTransferDB = new();
                if (stockTransferListDB.Any())
                {
                    stockTransferDB = stockTransferListDB.OrderBy(x => x.Id).LastOrDefault();
                }

                //Stock sold column will be changed with amount of cart.Quantity for last received stock transfer 
                if (stockTransferDB.Id != 0)
                {

                    double cartRemainingQty = tempcartQty;

                    if (cartRemainingQty != 0)
                    {
                        if (cartRemainingQty > stockTransferDB.StockSoldQty)
                        {
                            tempcartQty = stockTransferDB.StockSoldQty;
                            cartRemainingQty -= stockTransferDB.StockSoldQty;
                        }
                        else if (-cartRemainingQty > stockTransferDB.StockSoldQty)
                        {
                            tempcartQty = stockTransferDB.StockSoldQty;
                            cartRemainingQty += stockTransferDB.StockSoldQty;
                        }
                        else
                        {
                            cartRemainingQty = 0;
                        }
                        //Manage Sold Quantity in Purchase Stock
                        if (cartRemainingQty > 0)
                        {
                            stockTransferDB.StockSoldQty = stockTransferDB.StockSoldQty - tempcartQty;
                        }
                        else
                        { stockTransferDB.StockSoldQty = stockTransferDB.StockSoldQty + tempcartQty; }
                        _unitOfWork.StockTransfer.Update(stockTransferDB);
                        _unitOfWork.Save();

                        tempcartQty = cartRemainingQty;
                    }
                }
            }

            //Here we add the cart to the POS or Reloading Order
            ShopCart cartFromDb = new ShopCart();
            ShopCart cart = new();
            cartFromDb = _unitOfWork.ShopCart.GetFirstOrDefault(u => u.ApplicationUserId == _userId && u.VariantId == saleOrderLine.VariantId);
            if (cartFromDb == null)
            {
                cart.VariantId = saleOrderLine.VariantId;
                cart.Quantity = saleOrderLine.StockTransfer.SalesQty;
                cart.ApplicationUserId = saleOrderLine.StockTransfer.ApplicationUser.Id;
                cart.UnitOfMeasureId = saleOrderLine.StockTransfer.UnitOfMeasureId;
                cart.Price = saleOrderLine.StockTransfer.UnitSellingPrice;
                cart.Discount = saleOrderLine.StockTransfer.Discount;
                cart.DiscountPercent = saleOrderLine.StockTransfer.DiscountPercent;
                _unitOfWork.ShopCart.Add(cart);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.ShopCart.IncrementCount(cartFromDb, saleOrderLine.StockTransfer.SalesQty);
                _unitOfWork.Save();
            }

            CreditNote = _unitOfWork.CreditNote.GetFirstOrDefault(u => u.CreatedBy == _userId && u.CashOut == false);
            if (CreditNote == null)
            {
                CreditNote = new CreditNote();
                CreditNote.CreatedBy = _userId;
                CreditNote.CreatedDate = DateTime.Now;
                CreditNote.ShopId = saleOrderLine.StockTransfer.ShopId;
                CreditNote.CashOut = false;
                CreditNote.Cash = -(double)(saleOrderLine.StockTransfer.SalesQty * saleOrderLine.StockTransfer.UnitSellingPrice - (double)saleOrderLine.StockTransfer.Discount - (double)saleOrderLine.StockTransfer.DiscountPercent);
                _unitOfWork.CreditNote.Add(CreditNote);
                _unitOfWork.Save();
            }
            else
            {
                CreditNote.Cash -= (double)(saleOrderLine.StockTransfer.SalesQty * saleOrderLine.StockTransfer.UnitSellingPrice - saleOrderLine.StockTransfer.Discount - saleOrderLine.StockTransfer.DiscountPercent);
                _unitOfWork.CreditNote.Update(CreditNote);
                _unitOfWork.Save();
            }


            StockTransfer tempstockTransfer = _unitOfWork.StockTransfer.GetFirstOrDefault(x => x.Id == saleOrderLine.StockTransferId);
            _unitOfWork.StockTransfer.Remove(tempstockTransfer);
            _unitOfWork.Save();
        }

        [HttpPost]
        [ActionName("Index")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult IndexPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            bool wholeSale = false;
            if (ShopCartVM.ShopCart.IsWholesale == true)
            {
                wholeSale = true;
            }


            //dropdown lists
            ShopCartVM.VariantList = _unitOfWork.Variant.GetAll().Where(x => x.ShopId == applicationUser.ShopId).Select(i => new SelectListItem { Text = i.VariantCode + " " + i.Name, Value = i.Id.ToString() });
            ShopCartVM.VariantDataList = _unitOfWork.Variant.GetAll().OrderByDescending(o => o.Id).Where(x => x.ShopId == applicationUser.ShopId);
            ShopCartVM.CustomerDataList = _unitOfWork.ShopCustomer.GetAll(x => x.ShopId == applicationUser.ShopId && x.IsDeleted != true && x.IsDisable != true);
            ShopCartVM.UnitOfMeasureList = new List<SelectListItem>();
            ShopCartVM.UnitOfMeasureList.AddRange(_unitOfWork.UnitOfMeasure.GetAll().Where(x => x.IsDisable == false).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }));


            Variant varriantDb = new();
            ShopCartVM.ShopCart.ApplicationUserId = claim.Value;
            if (ShopCartVM.ShopCart.Variant.SKU != null)
            {
                string[] sku = ShopCartVM.ShopCart.Variant.SKU.Split(",");
                varriantDb = _unitOfWork.Variant.GetFirstOrDefault(f => f.SKU == ShopCartVM.ShopCart.Variant.SKU || f.PackingSKU == ShopCartVM.ShopCart.Variant.SKU || f.VariantCode == sku[0]);

                if (ShopCartVM.TotalPrice != null && ShopCartVM.TotalPrice > 0 && ShopCartVM.ShopCart.Quantity > 0)
                {

                    if (ShopCartVM.ShopCart.UnitOfMeasureId == _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Name == SDUnits.Unit_packing).Id)
                    {
                        ShopCartVM.ShopCart.Quantity = ShopCartVM.ShopCart.Quantity * varriantDb.Packing;
                    }
                    ShopCartVM.ShopCart.Variant.ListPrice = ShopCartVM.TotalPrice / ShopCartVM.ShopCart.Quantity;
                }
                //If variant is null and the variant Id is sent then it will check that is it only a digit and now this check is not working because sku is sending the sku or variant code
                if (varriantDb == null)
                {
                    bool isDigit = IsDigitsOnly(ShopCartVM.ShopCart.Variant.SKU);
                    if (ShopCartVM.ShopCart.Variant.SKU.Length < 6 && isDigit == true)
                    {
                        varriantDb = _unitOfWork.Variant.GetFirstOrDefault(f => f.Id == Convert.ToInt32(ShopCartVM.ShopCart.Variant.SKU));
                    }
                    if (varriantDb == null)
                    {
                        TempData["error"] = "Variant does not exist...";
                        return RedirectToAction(nameof(Index));
                    }
                }


                ShopCartVM.ShopCart.VariantId = varriantDb.Id;
                if (ShopCartVM.ShopCart.Quantity == 0)
                {
                    ShopCartVM.ShopCart.Quantity = 1;
                }
                if (ShopCartVM.ShopCart.UnitOfMeasureId == null)
                {
                    ShopCartVM.ShopCart.UnitOfMeasureId = varriantDb.UnitOfMeasureId;
                }
                if (ShopCartVM.ShopCart.Variant.SKU == varriantDb.PackingSKU)
                {
                    ShopCartVM.ShopCart.UnitOfMeasureId = _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Name == SDUnits.Unit_packing).Id;
                }
                //If List price is loaded and changed or not changed the list price will be not null so that value will be set otherwise value of variant will be set
                if (ShopCartVM.ShopCart.Variant.ListPrice == null)
                {
                    ShopCartVM.ShopCart.Variant.ListPrice = (double)varriantDb.ListPrice;
                }
                if (ShopCartVM.ShopCart.IsWholesale == true)
                {
                    if (ShopCartVM.ShopCart.Variant.WholesalePrice == null)
                    {
                        ShopCartVM.ShopCart.Variant.WholesalePrice = (double)varriantDb.WholesalePrice;
                    }
                }
                ShopCartVM.AvailableQty = _unitOfWork.StockTransfer.GetStockQuantity(varriantDb.Id, (int)applicationUser.ShopId);
            }
            ShopCartVM.ListCart = _unitOfWork.ShopCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Variant").OrderByDescending(o => o.Id);
            ShopCartVM.OrderHeader = new();
            ShopCartVM.StockTransfer = new();
            if (ShopCartVM.ShopCart.Variant.ListPrice == null)
            {
                TempData["error"] = "Nothing selected...";
                return View(ShopCartVM);
            }
            //Check, if cart doesn't contains variant
            ShopCart cartFromDb = new ShopCart();
            if (varriantDb.Id == 0)
            {
                varriantDb = _unitOfWork.Variant.GetFirstOrDefault(u => u.Id == ShopCartVM.ShopCart.VariantId);
            }
            cartFromDb = _unitOfWork.ShopCart.GetFirstOrDefault(u => u.ApplicationUserId == claim.Value && u.VariantId == ShopCartVM.ShopCart.VariantId);
            if (cartFromDb == null)
            {
                ShopCartVM.ShopCart.Price = (double)ShopCartVM.ShopCart.Variant.ListPrice;
                if (ShopCartVM.ShopCart.IsWholesale == true)
                {
                    ShopCartVM.ShopCart.Price = (double)ShopCartVM.ShopCart.Variant.WholesalePrice;
                }
                ShopCartVM.StockTransfer = null;
                if (ShopCartVM.ShopCart.UnitOfMeasureId == _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Name == SDUnits.Unit_packing).Id)
                {
                    ShopCartVM.ShopCart.Quantity = ShopCartVM.ShopCart.Quantity * varriantDb.Packing;
                }
                else
                {
                    UnitOfMeasure unitOfMeasure = _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Id == ShopCartVM.ShopCart.UnitOfMeasureId);
                    ShopCartVM.ShopCart.Quantity = ShopCartVM.ShopCart.Quantity * unitOfMeasure.ValueInBaseUnit;
                }
                if (ShopCartVM.IsReturn != true)
                {
                    if (ShopCartVM.AvailableQty <= 0 || ShopCartVM.ShopCart.Quantity > ShopCartVM.AvailableQty)
                    {
                        if (varriantDb.IsWithoutStock == false)
                        {
                            TempData["error"] = ShopCartVM.AvailableQty + " Stock is available...";
                            ShopCartVM.ShopCart.VariantId = null;
                            ShopCartVM.ShopCart.Variant = null;
                            return View(ShopCartVM);
                        }
                    }
                }
                ShopCartVM.ShopCart.Variant = null;
                ///Return sale module
                if (ShopCartVM.IsReturn == true)
                {
                    TempData["error"] = ShopCartVM.ShopCart.Quantity + " Stock is returned...";
                    ShopCartVM.ShopCart.Quantity = -ShopCartVM.ShopCart.Quantity;
                }
                ///Return sale module close
                if (ShopCartVM.ShopCart.VariantId != null)
                {
                    _unitOfWork.ShopCart.Add(ShopCartVM.ShopCart);
                }
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SDSessionCart.SessionCart, _unitOfWork.ShopCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
            }
            else            //Check, if cart all-ready contains variant than increment
            {
                if (ShopCartVM.ShopCart.UnitOfMeasureId == _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Name == SDUnits.Unit_packing).Id)
                {
                    ShopCartVM.ShopCart.Quantity = ShopCartVM.ShopCart.Quantity * varriantDb.Packing;
                }
                else
                {
                    UnitOfMeasure unitOfMeasure = _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Id == ShopCartVM.ShopCart.UnitOfMeasureId);
                    ShopCartVM.ShopCart.Quantity = ShopCartVM.ShopCart.Quantity * unitOfMeasure.ValueInBaseUnit;
                }
                if (ShopCartVM.AvailableQty <= 0 || ShopCartVM.ShopCart.Quantity > ShopCartVM.AvailableQty)
                {
                    if (varriantDb.IsWithoutStock == false)
                    {
                        TempData["error"] = ShopCartVM.AvailableQty + " Stock is available...";
                        return View(ShopCartVM);
                    }
                }
                _unitOfWork.ShopCart.IncrementCount(cartFromDb, ShopCartVM.ShopCart.Quantity);
                _unitOfWork.Save();
            }
            ShopCartVM.ListCart = _unitOfWork.ShopCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Variant").OrderByDescending(o => o.Id);
            foreach (var cart in ShopCartVM.ListCart)
            {
                ShopCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Quantity);
            }
            if (ShopCartVM.ShopCart.Variant != null)
            {
                if (ShopCartVM.ShopCart.Variant.ShopProduct != null)
                {
                    ShopCartVM.ShopCart.Variant.ShopProduct.CatalogId = 0;
                }
            }
            ShopCartVM.ShopCart.Variant.ShopProductId = 0;
            ShopCartVM.ShopCart.VariantId = null;
            if (wholeSale == true)
            {
                return RedirectToAction(nameof(Index), new { WC = "wc" });
            }
            if (ShopCartVM.ShopCustomerId != null)
            {
                return RedirectToAction(nameof(Index), new { CustomerDetail = ShopCartVM.ShopCustomerId, retainOrderDate = ShopCartVM.OrderDate });
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult PlaceOrder(int? PrintSaleOrderId)     // Used only for load view of orders
        {
            SaleOrderVM = new SaleOrderVM();
            SaleOrderVM.SaleOrder = new SaleOrder();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            SaleOrderVM.SaleOrderLineList = _unitOfWork.SaleOrderLine.GetAll(u => u.SaleOrderId == PrintSaleOrderId, includeProperties: "StockTransfer,Variant");
            SaleOrderVM.SaleOrderVMs = SaleOrderVM.SaleOrderLineList.GroupBy(g => g.VariantId).Select(s => new SaleOrderVM()
            {
                VariantId = s.Key,
                Variant = s.FirstOrDefault().Variant,
                StockTransfer = s.FirstOrDefault().StockTransfer,
                SalesQtyGroupBy = s.Sum(x => x.StockTransfer.SalesQty)
            }).ToList();
            SaleOrderVM.SaleOrder.Id = (int)PrintSaleOrderId;
            foreach (var item in SaleOrderVM.SaleOrderLineList)
            {
                SaleOrderVM.OrderTotal += (item.StockTransfer.UnitSellingPrice * item.StockTransfer.SalesQty);
            }


            SaleOrderVM.SaleOrder = _unitOfWork.SaleOrder.GetFirstOrDefault(u => u.Id == SaleOrderVM.SaleOrder.Id, includeProperties: "ApplicationUser.Shop,Customer");
            SaleOrderVM.ShopName = SaleOrderVM.SaleOrder.ApplicationUser.Shop.Name;
            SaleOrderVM.ShopAddress = SaleOrderVM.SaleOrder.ApplicationUser.Shop.StreetAddress;
            return View(SaleOrderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(string? NoPrint)
        {
            if (SaleOrderVM.SaleOrder == null)
            {
                SaleOrderVM = new SaleOrderVM();
                SaleOrderVM.SaleOrder = new SaleOrder();
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShopCustomer shopCustomer = new ShopCustomer();
                if (ShopCartVM.ShopCustomerId != null)
                {
                    string[] customer = ShopCartVM.ShopCustomerId.Split(",");
                    shopCustomer = _unitOfWork.ShopCustomer.GetFirstOrDefault(u => u.CustomerCode == customer[0] && u.IsDisable != true);
                    if (shopCustomer == null)
                    {
                        TempData["error"] = "Customer does not exists...";
                        return RedirectToAction(nameof(Index));
                    }
                }
                ShopCartVM.ListCart = _unitOfWork.ShopCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Variant,UnitOfMeasure").OrderByDescending(o => o.Id);
                ShopCartVM.OrderHeader = new();

                if (ShopCartVM.ListCart.Count() != 0)
                {
                    SaleOrderVM.SaleOrder.ShopId = applicationUser.ShopId;
                    SaleOrderVM.SaleOrder.CreatedBy = claim.Value;
                    if (ShopCartVM.OrderDate != null) { SaleOrderVM.SaleOrder.CreatedDate = ShopCartVM.OrderDate; } else { SaleOrderVM.SaleOrder.CreatedDate = DateTime.Now; }
                    SaleOrderVM.SaleOrder.ProfitCenter = _userId;
                    SaleOrderVM.SaleOrder.InventoryStatus = SDStockType.StockType_Sale;
                    if (shopCustomer.Id != 0 && shopCustomer != null)
                    { SaleOrderVM.SaleOrder.ShopCustomerId = shopCustomer.Id; }
                    SaleOrderVM.SaleOrder = _unitOfWork.SaleOrder.AddAndReturn(SaleOrderVM.SaleOrder);
                    _unitOfWork.Save();
                }
                else
                {
                    //This module is to create Account Receivable if cart is empty and received amount is not null for any customer
                    if (ShopCartVM.ShopCustomerId != null && ShopCartVM.ReceivedAmount != null && ShopCartVM.ReceivedAmount != 0)
                    {
                        _unitOfWork.AccountReceivable.accountReceivableUpdate(0, 0, shopCustomer.Id, _userId, applicationUser.ShopId, (double)ShopCartVM.ReceivedAmount, ShopCartVM.OrderDate);
                        TempData["success"] = "Customer Account is updated...";
                        return RedirectToAction(nameof(Index));
                    }

                    TempData["error"] = "Cart is empty...";
                    return RedirectToAction(nameof(Index));
                }

                if (SaleOrderVM.SaleOrder.Id != 0)
                {
                    foreach (var cart in ShopCartVM.ListCart)
                    {
                        while (cart.Quantity != 0)
                        {
                            SaleOrderVM.StockTransfer = new StockTransfer();
                            SaleOrderVM.SaleOrderLine = new SaleOrderLine();
                            StockTransfer stockTransferDB = new StockTransfer();
                            Variant variant = _unitOfWork.Variant.GetFirstOrDefault(x => x.Id == cart.VariantId);
                            double cartRemainingQty = cart.Quantity;

                            if (variant.IsWithoutStock == false)
                            {
                                if (cart.Quantity > 0)
                                {
                                    stockTransferDB = _unitOfWork.StockTransfer.GetFirstOrDefault(x => x.VariantId == cart.VariantId && x.ReceivedQty != x.StockSoldQty && x.StockType == SDStockType.StockType_Purchase);
                                }
                                else
                                {
                                    IEnumerable<StockTransfer> stockTransferListDB = _unitOfWork.StockTransfer.GetAll(x => x.VariantId == cart.VariantId && x.StockSoldQty != 0 && x.StockType == SDStockType.StockType_Purchase);
                                    if (stockTransferListDB.Count() > 0)
                                    {
                                        stockTransferDB = stockTransferListDB.OrderBy(x => x.Id).LastOrDefault();
                                    }
                                }

                                //Id Stock is not available (Sold for selling query, and nothing sold for sale returning query)
                                if (stockTransferDB == null)
                                {
                                    _unitOfWork.SaleOrder.Remove(SaleOrderVM.SaleOrder);
                                    _unitOfWork.Save();
                                    //If all stock is sold of given variant, it will be redirected to Index
                                    if (cart.Quantity > 0)
                                    {
                                        TempData["error"] = "Stock is sold...";
                                        return RedirectToAction(nameof(Index));
                                    }

                                    //If no stock is sold of given variant and we are going to return sale to customer, it will be redirected to Index
                                    if (cart.Quantity < 0)
                                    {
                                        TempData["error"] = "Nothing is sold yet...";
                                        return RedirectToAction(nameof(Index));
                                    }
                                }
                                //if (stockTransferDB.Id != 0)
                                else
                                {
                                    if (cartRemainingQty > 0)
                                    {
                                        if (cart.Quantity > (stockTransferDB.ReceivedQty - stockTransferDB.StockSoldQty))
                                        {
                                            cart.Quantity = stockTransferDB.ReceivedQty - stockTransferDB.StockSoldQty;
                                        }
                                    }
                                    else
                                    {
                                        cart.Quantity = -cart.Quantity;
                                        if (cart.Quantity > stockTransferDB.StockSoldQty)
                                        {
                                            cart.Quantity = stockTransferDB.StockSoldQty;
                                        }
                                        cart.Quantity = -cart.Quantity;
                                    }
                                    //Manage Sold Quantity in Purchase Stock
                                    stockTransferDB.StockSoldQty = stockTransferDB.StockSoldQty + cart.Quantity;
                                    _unitOfWork.StockTransfer.Update(stockTransferDB);
                                    _unitOfWork.Save();
                                }
                            }
                            else
                            //Adding Stocktransfer instance to database
                            {
                                SaleOrderVM.StockTransfer.UnitPurchasePrice = (double)variant.PurchasePrice;
                            }
                            SaleOrderVM.StockTransfer.ReceivedQty = 0;
                            if (variant.IsWithoutStock == false)
                            {
                                SaleOrderVM.StockTransfer.UnitPurchasePrice = stockTransferDB.UnitPurchasePrice;
                            }
                            SaleOrderVM.StockTransfer.SalesQty = cart.Quantity;
                            SaleOrderVM.StockTransfer.UnitSellingPrice = cart.Price;
                            SaleOrderVM.StockTransfer.VariantId = (int)cart.VariantId;
                            if (cart.Discount == null) cart.Discount = 0;
                            SaleOrderVM.StockTransfer.Discount = (double)cart.Discount;
                            if (ShopCartVM.DiscountPercent == null) ShopCartVM.DiscountPercent = 0;
                            SaleOrderVM.StockTransfer.DiscountPercent = (double)ShopCartVM.DiscountPercent;
                            SaleOrderVM.StockTransfer.ShopId = applicationUser.ShopId;
                            SaleOrderVM.StockTransfer.CreatedBy = claim.Value;
                            if (ShopCartVM.OrderDate != null) { SaleOrderVM.StockTransfer.CreatedDate = ShopCartVM.OrderDate; } else { SaleOrderVM.StockTransfer.CreatedDate = DateTime.Now; }
                            SaleOrderVM.StockTransfer.StockType = SDStockType.StockType_Sale;
                            SaleOrderVM.StockTransfer.UnitOfMeasureId = cart.UnitOfMeasureId;
                            if (cart.Quantity < 0)
                            {
                                SaleOrderVM.StockTransfer.StockType = SDStockType.StockType_SaleReturn;
                            }
                            if (ShopCartVM.ShopCustomerId != null)
                            {
                                SaleOrderVM.StockTransfer.ShopCustomerId = SaleOrderVM.SaleOrder.Customer.Id;
                            }
                            SaleOrderVM.StockTransfer = _unitOfWork.StockTransfer.AddAndReturn(SaleOrderVM.StockTransfer);
                            _unitOfWork.Save();

                            //Manage Sale Order Line
                            SaleOrderVM.SaleOrderLine.StockTransferId = SaleOrderVM.StockTransfer.Id;
                            if (variant.IsWithoutStock == true)
                            {
                                SaleOrderVM.SaleOrderLine.PurchaseStockId = stockTransferDB.Id;
                            }
                            SaleOrderVM.SaleOrderLine.SaleOrderId = SaleOrderVM.SaleOrder.Id;
                            SaleOrderVM.SaleOrderLine.VariantId = (int)cart.VariantId;
                            SaleOrderVM.SaleOrderLine.ShopId = applicationUser.ShopId;
                            if (ShopCartVM.OrderDate != null) { SaleOrderVM.SaleOrderLine.CreatedDate = ShopCartVM.OrderDate; } else { SaleOrderVM.SaleOrderLine.CreatedDate = DateTime.Now; }
                            SaleOrderVM.SaleOrderLine = _unitOfWork.SaleOrderLine.AddAndReturn(SaleOrderVM.SaleOrderLine);
                            _unitOfWork.Save();

                            //CreditNote is for End of Day price
                            CreditNote = _unitOfWork.CreditNote.GetFirstOrDefault(u => u.CreatedBy == claim.Value && u.CashOut == false);
                            if (CreditNote == null)
                            {
                                CreditNote = new CreditNote();
                                CreditNote.CreatedBy = claim.Value;
                                if (ShopCartVM.OrderDate != null) { CreditNote.CreatedDate = ShopCartVM.OrderDate; } else { CreditNote.CreatedDate = DateTime.Now; }
                                CreditNote.ShopId = SaleOrderVM.StockTransfer.ShopId;
                                CreditNote.CashOut = false;
                                CreditNote.Cash = (double)(SaleOrderVM.StockTransfer.SalesQty * SaleOrderVM.StockTransfer.UnitSellingPrice - (double)SaleOrderVM.StockTransfer.Discount - (double)SaleOrderVM.StockTransfer.DiscountPercent);
                                _unitOfWork.CreditNote.Add(CreditNote);
                            }
                            else
                            {
                                CreditNote.Cash += (double)(SaleOrderVM.StockTransfer.SalesQty * SaleOrderVM.StockTransfer.UnitSellingPrice - SaleOrderVM.StockTransfer.Discount - SaleOrderVM.StockTransfer.DiscountPercent);
                                _unitOfWork.CreditNote.Update(CreditNote);
                            }
                            cart.Quantity = cartRemainingQty - cart.Quantity;
                            stockTransferDB = null;

                            SaleOrderVM.StockTransfer = null;
                            SaleOrderVM.SaleOrderLine = null;
                        }
                    }
                }
                SaleOrderVM.SaleOrderLineList = _unitOfWork.SaleOrderLine.GetAll(u => u.SaleOrderId == SaleOrderVM.SaleOrder.Id, includeProperties: "StockTransfer,Variant");
                _unitOfWork.ShopCart.RemoveRange(ShopCartVM.ListCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SDSessionCart.SessionCart, 0);
            }

            SaleOrderVM.SaleOrderLineList = _unitOfWork.SaleOrderLine.GetAll(u => u.SaleOrderId == SaleOrderVM.SaleOrder.Id, includeProperties: "StockTransfer,Variant");
            //Here groupby is used to group the order by variant to print the invoice
            SaleOrderVM.SaleOrderVMs = SaleOrderVM.SaleOrderLineList.GroupBy(g => g.VariantId).Select(s => new SaleOrderVM()
            {
                VariantId = s.Key,
                Variant = s.FirstOrDefault().Variant,
                StockTransfer = s.FirstOrDefault().StockTransfer,
                SalesQtyGroupBy = s.Sum(x => x.StockTransfer.SalesQty)
            }).ToList();
            foreach (var item in SaleOrderVM.SaleOrderLineList)
            {
                SaleOrderVM.OrderTotal += (item.StockTransfer.UnitSellingPrice * item.StockTransfer.SalesQty - item.StockTransfer.Discount - item.StockTransfer.Discount);
            }
            if (ShopCartVM.ShopCustomerId != null)
            {
                if (ShopCartVM.ReceivedAmount == null) { ShopCartVM.ReceivedAmount = 0; }
                _unitOfWork.AccountReceivable.accountReceivableUpdate(SaleOrderVM.OrderTotal, SaleOrderVM.SaleOrder.Id, SaleOrderVM.SaleOrder.Customer.Id, _userId, applicationUser.ShopId, (double)ShopCartVM.ReceivedAmount, ShopCartVM.OrderDate);
            }

            SaleOrderVM.SaleOrder = _unitOfWork.SaleOrder.GetFirstOrDefault(u => u.Id == SaleOrderVM.SaleOrder.Id, includeProperties: "ApplicationUser.Shop");
            if (SaleOrderVM.SaleOrder.ApplicationUser.Shop != null) SaleOrderVM.ShopName = SaleOrderVM.SaleOrder.ApplicationUser.Shop.Name;
            if (NoPrint == "noprint")
            {
                TempData["success"] = "Order created successfully...";
                return RedirectToAction(nameof(Index), new { retainOrderDate = ShopCartVM.OrderDate });
            }
            return View(SaleOrderVM);
        }


        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShopCart.GetFirstOrDefault(u => u.Id == cartId, includeProperties: "Variant");
            if (cart.VariantId != null)
            {
                ShopCartVM = new();
                int vId = (int)cart.VariantId;
                ShopCartVM.AvailableQty = _unitOfWork.StockTransfer.GetStockQuantity(vId, (int)applicationUser.ShopId);
                if (ShopCartVM.AvailableQty <= 0 || (cart.Quantity + 1) > ShopCartVM.AvailableQty)
                {
                    if (cart.Variant.IsWithoutStock == false)
                    {
                        TempData["error"] = ShopCartVM.AvailableQty + " Stock is available...";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            _unitOfWork.ShopCart.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShopCart.GetFirstOrDefault(u => u.Id == cartId);
            if (cart.Quantity <= 1)
            {
                _unitOfWork.ShopCart.Remove(cart);
                var count = _unitOfWork.ShopCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32(SDSessionCart.SessionCart, count);
            }
            else
            {
                _unitOfWork.ShopCart.DecrementCount(cart, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            bool wholeSale = false;
            var cart = _unitOfWork.ShopCart.GetFirstOrDefault(u => u.Id == cartId);
            if (cart.IsWholesale == true)
            {
                wholeSale = true;
            }
            _unitOfWork.ShopCart.Remove(cart);
            _unitOfWork.Save();
            var count = _unitOfWork.ShopCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SDSessionCart.SessionCart, count);
            if (wholeSale == true)
            {
                return RedirectToAction(nameof(Index), new { WC = "wc" });
            }
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedWholesale(bool isWholesale, double price, double wholesalePrice)
        {
            if (isWholesale == false)
            {
                return price;
            }
            else
            {
                return wholesalePrice;
            }
        }

        #region API CALLS
        [Authorize]
        public IActionResult GetDIVDetails(string Id)
        {
            string[] sku = Id.Split(",");
            if (Id == null) { return RedirectToAction(nameof(Index)); }
            bool isDigit = IsDigitsOnly(Id);//!Id.Any(ch => ch < '0' || ch > '9');
            int vId = 0;
            ShopCartVM shopCartVM = new ShopCartVM();
            shopCartVM.ShopCart = new();
            shopCartVM.ShopCart.Variant = new();
            if (Id.Length < 6 && isDigit == true)
            {
                vId = Convert.ToInt32(Id);
                shopCartVM.ShopCart.VariantId = vId;
                shopCartVM.ShopCart.Variant = _unitOfWork.Variant.GetFirstOrDefault(x => x.Id == vId && x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true, includeProperties: "ShopProduct");
            }
            else
            {
                shopCartVM.ShopCart.Variant.SKU = Id;
                shopCartVM.ShopCart.Variant = _unitOfWork.Variant.GetFirstOrDefault(x => (x.SKU == Id || x.PackingSKU == Id || x.VariantCode == sku[0]) && x.ShopId == applicationUser.ShopId && x.IsDisable != true && x.IsDeleted != true, includeProperties: "ShopProduct");
                if (shopCartVM.ShopCart.Variant == null)
                {
                    TempData["error"] = "Variant not exist...";
                    return PartialView("~/Views/Shared/_DIVDetailsPartial.cshtml", shopCartVM);
                }
                shopCartVM.ShopCart.VariantId = shopCartVM.ShopCart.Variant.Id;
            }
            shopCartVM.UnitOfMeasureList = new List<SelectListItem>();
            if (shopCartVM.ShopCart.Variant != null)
            {
                if (shopCartVM.ShopCart.Variant.IsWeight != true)
                {
                    shopCartVM.UnitOfMeasureList.AddRange(_unitOfWork.UnitOfMeasure.GetAll().Where(x => x.IsDisable == false && x.IsWaight == false).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }));
                }
                else
                {
                    //shopCartVM.UnitOfMeasureList.Add(new SelectListItem { Text = "- Select -", Value = "0" });
                    UnitOfMeasure unitOfMeasureDb = _unitOfWork.UnitOfMeasure.GetFirstOrDefault(u => u.Name == SDUnits.Unit_packing);
                    shopCartVM.UnitOfMeasureList.Add(new SelectListItem { Text = unitOfMeasureDb.Name, Value = unitOfMeasureDb.Id.ToString() });
                    shopCartVM.UnitOfMeasureList.AddRange(_unitOfWork.UnitOfMeasure.GetAll().Where(x => x.IsDisable == false && x.IsWaight == true).Select(i => new SelectListItem { Text = i.Name, Value = i.Id.ToString() }));
                }
                shopCartVM.AvailableQty = _unitOfWork.StockTransfer.GetStockQuantity(shopCartVM.ShopCart.Variant.Id, (int)applicationUser.ShopId);
                shopCartVM.ShopCart.Quantity = 1;
                //shopCartVM.ShopCart.IsWholesale = false;
                shopCartVM.ShopCart.UnitOfMeasureId = shopCartVM.ShopCart.Variant.UnitOfMeasureId;
            }
            return PartialView("~/Views/Shared/_DIVDetailsPartial.cshtml", shopCartVM);
        }
        #endregion
        public static bool IsDigitsOnly(string str)
        {
            return !string.IsNullOrEmpty(str) && str.All(char.IsDigit);
        }

    }
}

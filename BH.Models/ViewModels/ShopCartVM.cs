using BH.Models.CustomerManagement;
using BH.Models.InventoryManagement;
using BH.Models.ProductManagement;
using BH.Models.SaleManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.ViewModels
{
    public class ShopCartVM
    {
        public IEnumerable<ShopCart> ListCart { get; set; }
        public ShopCart? ShopCart { get; set; }
        public IEnumerable<StockTransfer> StockTransferList { get; set; }
        public StockTransfer StockTransfer { get; set; }
        public OrderHeader OrderHeader { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? CatalogList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? ShopProductList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? VariantList { get; set; }
        [ValidateNever]
        public IEnumerable<Variant>? VariantDataList { get; set; }
        public string? ShopCustomerId { get; set; }
        [ValidateNever]
        public IEnumerable<ShopCustomer>? CustomerDataList { get; set; }
        [ValidateNever]
        public List<SelectListItem>? UnitOfMeasureList { get; set; }
        [Display(Name = "Available Qty")]
        public double AvailableQty { get; set; }
        [Display(Name = "Discount %")]
        public double? DiscountPercent { get; set; }
        [Display(Name = "Received Amount")]
        public double? ReceivedAmount { get; set; }
        [Display(Name = "Return Sale")]
        [ValidateNever]
        public bool IsReturn { get; set; }
        public int? SaleOrderId { get; set; }
        [Display(Name = "Order Date")]
        public DateTime? OrderDate { get; set; }
        [Display(Name = "Total Price")]
        public double? TotalPrice { get; set; }
    }
}

using BH.Models.CustomerManagement;
using BH.Models.InventoryManagement;
using BH.Models.ProductManagement;
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
    public class StockTransferVM
    {
        public StockTransfer? StockTransfer { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CatalogList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ShopProductList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> VariantList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> UserList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> LocationList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> UnitOfMeasureList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> SupplierList { get; set; }

        [ValidateNever]
        public IEnumerable<Variant>? VariantDataList { get; set; }

        //properties for group by
        public IEnumerable<StockTransfer> stockTransferList { get; set; }
        public IEnumerable<StockTransfer> stockTransferTotalList { get; set; }
        public IEnumerable<StockTransferVM> stockTransferVMs { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }
        public int? Hour { get; set; }
        public string? User { get; set; }
        public string? Product { get; set; }
        public Variant? Variant { get; set; }
        public string? SKU { get; set; }
        public string? Code { get; set; }
        public int? ProductId { get; set; }
        public int? VariantId { get; set; }
        public int? ShopCustomerId { get; set; }
        public ShopCustomer? ShopCustomer { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? ReceivingQuantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? ReturningToSupplierQuantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public string? UOM { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? SellingQuantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? Purchase { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? PurchaseStockIn { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? PurchaseStockReturn { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? Sale { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? Balance { get; set; }
        [Display(Name = "Profit / Loss")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? ProfitLoss { get; set; }
        public double? LowStockQty { get; set; }
        [Display(Name = "Available Qty")]
        public double? AvailableQty { get; set; }

        //Searching parameter
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? RegDateFrom { get; set; }
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? RegDateTo { get; set; }
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? ReceivedDateFrom { get; set; }
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? ReceivedDateTo { get; set; }
    }
}

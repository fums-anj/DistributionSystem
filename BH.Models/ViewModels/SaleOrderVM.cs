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
    public class SaleOrderVM
    {
        public StockTransfer StockTransfer { get; set; }
        public SaleOrder SaleOrder { get; set; }
        public SaleOrderLine SaleOrderLine { get; set; }
        public IEnumerable<SaleOrder> SaleOrderList { get; set; }
        public IEnumerable<SaleOrderLine> SaleOrderLineList { get; set; }
        public IEnumerable<SaleOrderVM> SaleOrderVMs { get; set; }
        public double OrderTotal { get; set; }
        public string ShopName { get; set; }
        public string ShopAddress { get; set; }
        public string FootNote { get; set; }

        //Searching parameter
        [ValidateNever]
        public IEnumerable<SelectListItem> UserList { get; set; }
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? RegDateFrom { get; set; }
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? RegDateTo { get; set; }

        //Properties used for GroupBy
        public Variant? Variant { get; set; }
        public string VariantName { get; set; }
        public int? VariantId { get; set; }
        public double? UnitSellingPriceForGroupBy { get; set; }
        public double SalesQtyGroupBy { get; set; }
    }
}

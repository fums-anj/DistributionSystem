using BH.Models.InventoryManagement;
using BH.Models.ProductManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.SaleManagement
{
    public class SaleOrderLine : Common
    {
        [Required]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }
        [Required]
        public int Qty { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }


        public int? StockTransferId { get; set; }
        [ForeignKey("StockTransferId")]
        [ValidateNever]
        public StockTransfer? StockTransfer { get; set; }
        public int? PurchaseStockId { get; set; }
        [ForeignKey("PurchaseStockId")]
        [ValidateNever]
        public StockTransfer? PurchaseStock { get; set; }
        [Display(Name = "Variant")]
        public int VariantId { get; set; }
        [ForeignKey("VariantId")]
        [ValidateNever]
        public Variant? Variant { get; set; }

        [Display(Name = "Sale Order")]
        public int SaleOrderId { get; set; }
        [ForeignKey("SaleOrderId")]
        [ValidateNever]
        public SaleOrder? SaleOrder { get; set; }
        public int Count { get; set; }

    }
}

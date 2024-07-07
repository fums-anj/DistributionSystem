using BH.Models.ShopManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.InventoryManagement
{
    public class PurchaseOrderLine : Common
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string SKU { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }
        [Required]
        public int OrderQty { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrict { get; set; }
        [Required]
        public int? RecQty { get; set; }

        //
        [Display(Name = "UOM")]
        public int UnitOfMeasureId { get; set; }

        [Display(Name = "Purchase Order")]
        public int PurchaseOrderId { get; set; }
        [ForeignKey("PurchaseOrderId")]
        [ValidateNever]
        public PurchaseOrder? PurchaseOrder { get; set; }
        //
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        [ValidateNever]
        public Supplier? Supplier { get; set; }
    }
}

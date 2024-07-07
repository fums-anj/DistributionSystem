using BH.Models.CustomerManagement;
using BH.Models.OrganizationManagement;
using BH.Models.ProductManagement;
using BH.Models.ShopManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.InventoryManagement
{
    public class StockTransfer : Common
    {
        [Display(Name = "Stock Type")]
        public string? StockType { get; set; }
        [Display(Name = "Receiving Date")]
        [DataType(DataType.Date)]
        public DateTime? ReceivingDate { get; set; }
        [Display(Name = "Expiry Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }
        [Display(Name = "Document Ref.")]
        public String? DocumentRef { get; set; }
        [Display(Name = "Received Qty")]
        [Range(0, 9999999, ErrorMessage = "Quantity must be non negative...")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double ReceivedQty { get; set; }
        [Display(Name = "Sales Qty")]
        [Range(0, 9999999, ErrorMessage = "Quantity must be non negative...")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double SalesQty { get; set; }
        public double StockSoldQty { get; set; } = 0;
        [Display(Name = "Purchase Price")]
        [Required]
        [DataType(DataType.Currency)]
        public double UnitPurchasePrice { get; set; }
        [Display(Name = "Sale Price")]
        [Required]
        [DataType(DataType.Currency)]
        public double UnitSellingPrice { get; set; }
        [Display(Name = "Discount per item")]
        [DataType(DataType.Currency)]
        public double Discount { get; set; }
        [Display(Name = "Discount %")]
        public double DiscountPercent { get; set; }
        [Display(Name = "Variant")]
        public int VariantId { get; set; }
        [ForeignKey("VariantId")]
        [ValidateNever]
        public Variant? Variant { get; set; }
        [Display(Name = "Unit Of Measure")]
        public int? UnitOfMeasureId { get; set; }
        [ForeignKey("UnitOfMeasureId")]
        [ValidateNever]
        public UnitOfMeasure? UnitOfMeasure { get; set; }
        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }
        [ForeignKey("SupplierId")]
        [ValidateNever]
        public Supplier? Supplier { get; set; }

        [Display(Name = "Location")]
        public int? LocationId { get; set; }
        [ForeignKey("LocationId")]
        [ValidateNever]
        public Location? Location { get; set; }
        [Display(Name = "Customer")]
        public int? ShopCustomerId { get; set; }
        [ForeignKey("ShopCustomerId")]
        [ValidateNever]
        public ShopCustomer? ShopCustomer { get; set; }
    }
}

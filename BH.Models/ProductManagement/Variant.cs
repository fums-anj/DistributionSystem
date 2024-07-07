using BH.Models.OrganizationManagement;
using BH.Models.ShopManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.ProductManagement
{
    public class Variant : Common
    {
        [Required]
        public string Name { get; set; }
        [Display(Name = "Vendor SKU")]
        public string? VendorSKU { get; set; }       //Stock Keeping Unit (Code given by Vendor)
        [Display(Name = "Packing Barcode")]
        public string? PackingSKU { get; set; }
        [Display(Name = "Barcode")]
        public string? SKU { get; set; }
        [Display(Name = "Code")]
        public string? VariantCode { get; set; }
        [Display(Name = "Packing / Crtn")]
        [Range(1, int.MaxValue)]
        public int Packing { get; set; }
        public decimal? Weight { get; set; }
        [Display(Name = "Measure in weight")]
        public bool IsWeight { get; set; }
        [Display(Name = "Price Range")]
        public string? Size { get; set; }
        [Display(Name = "Color")]
        public string? TypeColor { get; set; }
        [Display(Name = "Purchase")]
        public double? PurchasePrice { get; set; }
        [Display(Name = "Price")]
        public double? ListPrice { get; set; }
        [Display(Name = "Wholesale")]
        public double? WholesalePrice { get; set; }
        [Display(Name = "Min. Qty")]
        [Range(0, 9999999, ErrorMessage = "Quantity must be non negative...")]
        public double? LowStockWarningQuantity { get; set; }
        [Display(Name = "Storage Location")]
        public string? StorageLocation { get; set; }
        [Display(Name = "Image")]
        [ValidateNever]
        public string? ImageUrl { get; set; }
        [Display(Name = "Location")]
        public int? LocationId { get; set; }
        public Location? Location { get; set; }
        [Display(Name = "Sale Without Stock")]
        public bool IsWithoutStock { get; set; }
        //
        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }
        [ValidateNever]
        public Supplier? Supplier { get; set; }

        //
        [Display(Name = "UOM")]
        public int? UnitOfMeasureId { get; set; }
        [ForeignKey("UnitOfMeasureId")]
        [ValidateNever]
        public UnitOfMeasure? UnitOfMeasure { get; set; }
        //
        [Display(Name = "Product")]
        public int ShopProductId { get; set; }
        [ForeignKey("ShopProductId")]
        [ValidateNever]
        public ShopProduct? ShopProduct { get; set; }
    }
}

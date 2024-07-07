using BH.Models.OrganizationManagement;
using BH.Models.ProductManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.SaleManagement
{
    public class ShopCart
    {
        public int Id { get; set; }
        public int? VariantId { get; set; }
        [ForeignKey("VariantId")]
        public Variant? Variant { get; set; }
        [Range(0, 1000, ErrorMessage = "Please enter a value between 0 and 1000")]
        public double Quantity { get; set; }
        [Display(Name = "Is Wholesale")]
        [ValidateNever]
        public bool IsWholesale { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [Display(Name = "UOM")]
        public int? UnitOfMeasureId { get; set; }
        [Display(Name = "UOM")]
        [ForeignKey("UnitOfMeasureId")]
        public UnitOfMeasure? UnitOfMeasure { get; set; }

        public double Price { get; set; }
        [Display(Name = "Item Discount")]
        public double? Discount { get; set; }
        [Display(Name = "Discount %")]
        public double? DiscountPercent { get; set; }
    }
}

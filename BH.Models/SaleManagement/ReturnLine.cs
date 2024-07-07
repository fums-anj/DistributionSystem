using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Models.ProductManagement;

namespace BH.Models.SaleManagement
{
    public class ReturnLine : Common
    {
        [Required]
        public string ItemName { get; set; }
        [Required]
        public string SKU { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public string UnitPrice { get; set; }
        [Required]
        public int? Qty { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public string? TotalPrice { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }
        [Display(Name = "Sale Date")]
        public DateTime SaleOrderDate { get; set; }

        //
        [Display(Name = "Condition")]
        public int ItemConditionId { get; set; }
        [ForeignKey("ItemConditionId")]
        [ValidateNever]
        public ItemCondition? ItemCondition { get; set; }

        [Display(Name = "Return Order")]
        public int ReturnOrderId { get; set; }
        [ForeignKey("ReturnOrderId")]
        [ValidateNever]
        public ReturnOrder? ReturnOrder { get; set; }

    }
}

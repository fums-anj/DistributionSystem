using BH.Models.ProductManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.InventoryManagement
{
    public class PurchaseOrder : Common
    {
        [Required]
        public string ApprovedBy { get; set; }
        [Required]
        public int CostCenter { get; set; }


        [Display(Name = "Variant")]
        public int VariantId { get; set; }
        [ForeignKey("VariantId")]
        [ValidateNever]
        public Variant Variant { get; set; }
    }
}

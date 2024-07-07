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
    public class ReturnOrder : Common
    {
        [Required]
        public string ApprovalStatus { get; set; }
        [Required]
        public string CreditNote { get; set; }
        [Required]
        public string ApprovedBy { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public string? ReturnAmount { get; set; }
        [Display(Name = "Sale Date")]
        public DateTime SaleOrderDate { get; set; }

        //
        [Display(Name = "Sale Order")]
        public int SaleOrderId { get; set; }
        [ForeignKey("SaleOrderId")]
        [ValidateNever]
        public SaleOrder? SaleOrder { get; set; }
        //
        [Display(Name = "Return Reason")]
        public int ReturnReasonId { get; set; }
        [ForeignKey("ReturnReasonId")]
        [ValidateNever]
        public ReturnReason? ReturnReason { get; set; }

    }
}

using BH.Models.AccountManagement;
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
    public class Invoice : Common
    {
        [Required]
        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalTax { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal GrossTotal { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? Discount { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal AmountPayable { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal AmountPaid { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        //
        [Display(Name = "Payment Method")]
        public int PaymentMethodId { get; set; }
        [ForeignKey("PaymentMethodId")]
        [ValidateNever]
        public PaymentMethod? PaymentMethod { get; set; }

        //
        [Display(Name = "Sale Order")]
        public int SaleOrderId { get; set; }
        [ForeignKey("SaleOrderId")]
        [ValidateNever]
        public SaleOrder? SaleOrder { get; set; }
    }
}

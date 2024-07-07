using BH.Models.OrganizationManagement;
using BH.Models.ShopManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.AccountManagement
{
    public class PaymentVoucher : Common
    {
        [Required]
        [Display(Name = "Payment Voucher")]
        public string Name { get; set; }
        [Display(Name = "Total Due")]
        [DataType(DataType.Currency)]
        public decimal TotalDue { get; set; }
        [Display(Name = "Total Paid")]
        [DataType(DataType.Currency)]
        public decimal TotalPaid { get; set; }
        [Display(Name = "Paid Date")]
        public DateTime PaidDate { get; set; }
        [Display(Name = "Payment Number")]
        public string PaymentDoc { get; set; }
        [Display(Name = "Payment Validity")]
        public string PaymentDocValidity { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        [Display(Name = "Payment Method")]
        public int PaymentMethodId { get; set; }
        [ForeignKey("PaymentMethodId")]
        [ValidateNever]
        public PaymentMethod PaymentMethod { get; set; }

        [Display(Name = "Account Payable")]
        public int AccountPayableId { get; set; }
        [ForeignKey("AccountPayableId")]
        [ValidateNever]
        public AccountPayable AccountPayable { get; set; }

        [Display(Name = "Status")]
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        [ValidateNever]
        public Status Status { get; set; }

        [Display(Name = "Location")]
        public int LocationId { get; set; }
        [ForeignKey("LocationId")]
        [ValidateNever]
        public Location Location { get; set; }
    }
}

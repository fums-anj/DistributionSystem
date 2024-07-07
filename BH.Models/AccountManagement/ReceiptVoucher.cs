using BH.Models.OrganizationManagement;
using BH.Models.ShopManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.AccountManagement
{
    public class ReceiptVoucher : Common
    {
        [Display(Name = "Total Receivable")]
        [DataType(DataType.Currency)]
        public decimal TotalReceivable { get; set; }
        [Display(Name = "Received Date")]
        public DateTime ReceivedDate { get; set; }

        //
        [Display(Name = "Account Receivable")]
        public int AccountReceivableId { get; set; }
        [ForeignKey("AccountReceivableId")]
        [ValidateNever]
        public AccountReceivable? AccountReceivable { get; set; }
        //
        [Display(Name = "Status")]
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        [ValidateNever]
        public Status? Status { get; set; }

        [Display(Name = "Location")]
        public int LocationId { get; set; }
        [ForeignKey("LocationId")]
        [ValidateNever]
        public Location? Location { get; set; }
    }
}

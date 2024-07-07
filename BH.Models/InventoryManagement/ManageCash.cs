using BH.Models.ShopManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BH.Models.InventoryManagement
{
    public class ManageCash : Common
    {
        [Display(Name = "Sale Amount")]
        public double SaleAmount { get; set; }
        [Display(Name = "Credit (Due Payment)")]
        public double Credit { get; set; }
        [Display(Name = "Debit (Paid)")]
        public double Debit { get; set; }
        public double Balance { get; set; }
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }
        [ValidateNever]
        public Supplier? Supplier { get; set; }

    }
}

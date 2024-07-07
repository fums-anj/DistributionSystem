using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.AccountManagement
{
    public class AccountPayable : Common
    {
        [Display(Name = "Cost Center")]
        public string CostCenter { get; set; }
        [Display(Name = "Payable Type")]
        public string PayableType { get; set; }
        [Display(Name = "Document Type")]
        public string DocumentType { get; set; }
        [Display(Name = "Document Number")]
        public string DocumentNum { get; set; }
        [Display(Name = "Reference Number")]
        public string ReferenceNum { get; set; }
        [Display(Name = "Total Payable")]
        [DataType(DataType.Currency)]
        public decimal TotalPayable { get; set; }
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }
        [Display(Name = "Paid Amount")]
        [DataType(DataType.Currency)]
        public decimal PaidAmount { get; set; }
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }
        [Display(Name = "Paid Date")]
        public DateTime PaidDate { get; set; }
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.AccountManagement
{
    public class Expense : Common
    {
        [Required]
        [Display(Name = "Person")]
        public string Name { get; set; }
        public string Description { get; set; }
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [Display(Name = "Approved By")]
        public string ApprovedBy { get; set; }
        [Display(Name = "Expense Date")]
        [DataType(DataType.Date)]
        public DateTime ApprovedDate { get; set; }
        [Required]
        [Display(Name = "Cost Center")]
        public string CostCenter { get; set; }

        [Display(Name = "Expense Type")]
        public int ExpenseTypeId { get; set; }
        [ForeignKey("ExpenseTypeId")]
        [ValidateNever]
        public ExpenseType? ExpenseType { get; set; }

    }
}

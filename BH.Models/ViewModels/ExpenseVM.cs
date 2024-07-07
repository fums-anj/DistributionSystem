using BH.Models.AccountManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.ViewModels
{
    public class ExpenseVM
    {
        public Expense? Expense { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ExpenseTypeList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ExpenseMonthList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ExpenseYearList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> UserList { get; set; }

        //properties for group by
        public IEnumerable<Expense> ExpenseList { get; set; }
        public IEnumerable<Expense> expenseVMs { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? ReturningToSupplierQuantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public string? UOM { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? SellingQuantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? Purchase { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? PurchaseStockIn { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? PurchaseStockReturn { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? Sale { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? Balance { get; set; }
        [Display(Name = "Profit / Loss")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? ProfitLoss { get; set; }
        public double? LowStockQty { get; set; }

        //Searching parameter
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? RegDateFrom { get; set; }
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? RegDateTo { get; set; }
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? ApprovedDateFrom { get; set; }
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? ApprovedDateTo { get; set; }
        public string? KeyWord { get; set; }
    }
}

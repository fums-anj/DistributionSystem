using BH.Models.AccountManagement;
using BH.Models.CustomerManagement;
using BH.Models.InventoryManagement;
using BH.Models.OrganizationManagement;
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
    public class AccountReceivableVM
    {
        public AccountReceivable accountReceivable { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ShopCustomerList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> RouteList { get; set; }
        [ValidateNever]
        public IEnumerable<ShopCustomer>? CustomerDataList { get; set; }

        //properties for group by
        public IEnumerable<AccountReceivable> AccountReceivableList { get; set; }
        public string? ShopCustomerCodeAndName { get; set; }
        public IEnumerable<AccountReceivableVM> AccountReceivableVMList { get; set; }
        [Display(Name = "Customer")]
        public int? ShopCustomerId { get; set; }
        public string ShopCustomerName { get; set; }
        [Display(Name = "Code")]
        public string ShopCustomerCode { get; set; }
        [Display(Name = "Address")]
        public string ShopCustomerAddress { get; set; }
        [Display(Name = "Day")]
        public string TermDay { get; set; }
        public string Route { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Bill")]
        public double Payable { get; set; }
        [Display(Name = "Received")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double Paid { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double Balance { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Blc of Visted")]
        public double? TotalBalance { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Pre. Blc")]
        public double? PreviousBalance { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Old Blc")]
        public double OldBalance { get; set; }

        public IEnumerable<Expense> ExpenseList { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expance")]
        public decimal? TotalExpance { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }
        [Display(Name = "Shops")]
        public int? VistedShops { get; set; }


        //Searching parameter
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? RegDateFrom { get; set; }
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? RegDateTo { get; set; }
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? ReceivedDateFrom { get; set; }
        [ValidateNever]
        [DataType(DataType.Date)]
        public DateTime? ReceivedDateTo { get; set; }

        public ApplicationUser? User { get; set; }
        public int? routeId { get; set; }
        
    }
}

using BH.Models.InventoryManagement;
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
    public class ManageCashVM
    {
        public ManageCash ManageCash { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> SupplierList { get; set; }

        //properties for group by
        public IEnumerable<ManageCash> manageCashList { get; set; }
        public int? SupplierId { get; set; }
        public string SupplierName { get; set; }
        [Display(Name = "Payment Term Day")]
        public string TermDay { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double Credit { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double Debit { get; set; }
        public double Balance { get; set; }
    }
}

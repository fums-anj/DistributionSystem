using BH.Models.ProductManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.ViewModels
{
    public class VariantVM
    {
        public Variant Variant { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CatalogList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ShopProductList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> LocationList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> UnitOfMeasureList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> SupplierList { get; set; }
    }
}

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
    public class ShopProductVM
    {
        public ShopProduct ShopProduct { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CatalogList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> LocationList { get; set; }
    }
}

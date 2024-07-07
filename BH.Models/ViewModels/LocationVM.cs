using BH.Models.OrganizationManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.ViewModels
{
    public class LocationVM
    {
        public Location Location { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> LoctionTypeList { get; set; }
    }
}

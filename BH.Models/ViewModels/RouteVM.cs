using BH.Models.CustomerManagement;
using BH.Models.OrganizationManagement;
using BH.Models.ProductManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.ViewModels
{
    public class RouteVM
    {
        public CustomerRoute CustomerRoute { get; set; }
        public IEnumerable<CustomerRoute> RouteList { get; set; }
        public IEnumerable<RouteVM> RouteListVM { get; set; }
        public IEnumerable<ShopCustomer> ShopCustomerList { get; set; }
        [Display(Name = "Route Balance")]
        public decimal? RouteBalance { get; set; }
        [Display(Name = "No. of Customers")]
        public int customerCount { get; set; }
        
    }
}

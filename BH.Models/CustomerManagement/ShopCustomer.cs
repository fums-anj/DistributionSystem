using BH.Models.OrganizationManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.CustomerManagement
{
    public class ShopCustomer : Common
    {
        [Display(Name = "Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Code")]
        public string? CustomerCode { get; set; }
        [Display(Name = "Phone")]
        [DataType(DataType.PhoneNumber)]
        public string CustomerPhone { get; set; }
        [Display(Name = "Address")]
        public string CustomerAddress { get; set; }
        [Display(Name = "City")]
        public string CustomerCity { get; set; }
        public double? Balance { get; set; }
        [Display(Name = "Payment Days")]
        public string? PaymentTermsDays { get; set; }
        [Display(Name = "Route")]
        [ValidateNever]
        public int? CustomerRouteId { get; set; }
        [ForeignKey("CustomerRouteId")]
        public CustomerRoute? CustomerRoute { get; set; }
        [DisplayName("Shop")]
        [ValidateNever]
        public int? ShopId { get; set; }
        [ForeignKey("ShopId")]
        [ValidateNever]
        public Shop Shop { get; set; }
        [NotMapped]
        [ValidateNever]
        public string? CreateFromPOS { get; set; }

    }
}

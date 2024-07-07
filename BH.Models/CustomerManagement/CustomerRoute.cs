using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.OrganizationManagement
{
    public class CustomerRoute : Common
    {
        [Required]
        public string RouteName { get; set; }
        [Required]
        public string RouteDay { get; set; }
        [Required]
        public string? RouteDetail { get; set; }
        [Display(Name = "Saleman")]
        public string? SalemanId { get; set; }
        [ForeignKey("SalemanId")]
        [ValidateNever]
        public ApplicationUser Saleman { get; set; }
        
        [ForeignKey("ShopId")]
        [ValidateNever]
        public Shop? Shop { get; set; }

    }
}

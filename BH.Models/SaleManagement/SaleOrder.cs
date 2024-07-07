using BH.Models.CustomerManagement;
using BH.Models.OrganizationManagement;
using BH.Models.ShopManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.SaleManagement
{
    public class SaleOrder : Common
    {
        [Required]
        public string? ProfitCenter { get; set; }
        [Required]
        public string? InventoryStatus { get; set; }

        //
        [Display(Name = "Customer")]
        public int? ShopCustomerId { get; set; }
        [ForeignKey("ShopCustomerId")]
        [ValidateNever]
        public ShopCustomer? Customer { get; set; }

        //
        [Display(Name = "Order Type")]
        public int? OrderTypeId { get; set; }
        [ForeignKey("OrderTypeId")]
        [ValidateNever]
        public OrderType? OrderType { get; set; }

        //
        [Display(Name = "Status")]
        public int? StatusId { get; set; }
        [ForeignKey("StatusId")]
        [ValidateNever]
        public Status? Status { get; set; }

        //
        [Display(Name = "Location")]
        public int? LocationId { get; set; }
        [ForeignKey("LocationId")]
        [ValidateNever]
        public Location? Location { get; set; }
        public virtual ICollection<SaleOrderLine> SaleOrderLines { get; set; }
    }
}

using BH.Models.AccountManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.ShopManagement
{
    public class Supplier : Common
    {
        [Required]
        public string Name { get; set; }
        [Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; }
        [Display(Name = "Contact Title")]
        public string? ContactTitle { get; set; }
        [Display(Name = "Email")]
        [EmailAddress]
        public string? ContactEmail { get; set; }
        [Display(Name = "Phone")]
        [Phone]
        public string? ContactPhone { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Address { get; set; }
        [Display(Name = "License")]
        public string? LicenseNum { get; set; }
        [Display(Name = "Payment Terms Days")]
        public string? PaymentTermsDays { get; set; }

        //
        [Display(Name = "Payment Method")]
        public int? PaymentMethodId { get; set; }
        [ForeignKey("PaymentMethodId")]
        [ValidateNever]
        public PaymentMethod? PaymentMethod { get; set; }
    }
}

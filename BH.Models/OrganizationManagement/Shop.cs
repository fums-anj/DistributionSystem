using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.OrganizationManagement
{
    public class Shop
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? FootNote { get; set; }
        [Display(Name = "Shop Logo")]
        public string? LogoUrl { get; set; }
        [Display(Name = "Street Address")]
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        [Display(Name = "Postal Code")]
        [DataType(DataType.PostalCode)]
        public string? PostalCode { get; set; }
        [Display(Name = "Mobile Number")]
        [DataType(DataType.PhoneNumber)]
        public string? MobileNumber { get; set; }
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
        [Display(Name = "Modified By")]
        public string? ModifiedBy { get; set; }
        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [Display(Name = "Logo")]
        [ValidateNever]
        public string? LogoImageUrl { get; set; }
        [Display(Name = "Favicon")]
        [ValidateNever]
        public string? FaviconImageUrl { get; set; }
        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }
        [Display(Name = "Disabled")]
        public bool IsDisable { get; set; }

    }
}

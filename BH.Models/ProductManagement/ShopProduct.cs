using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.ProductManagement
{
    public class ShopProduct : Common
    {
        [Required]
        public string Name { get; set; }
        [Display(Name = "Acronym")]
        [Required]
        public string? ProductAcronym { get; set; }
        [Required]
        public string? Brand { get; set; }

        //
        [Display(Name = "Catalog")]
        public int? CatalogId { get; set; }
        [ForeignKey("CatalogId")]
        [ValidateNever]
        public Catalog? Catalog { get; set; }

        [Display(Name = "Location")]
        public int? LocationId { get; set; }
    }
}

using BH.Models.OrganizationManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.ProductManagement
{
    public class Catalog : Common
    {
        [Required]
        public string Name { get; set; }

    }
}

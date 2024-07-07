using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.OrganizationManagement
{
    public class Location : Common
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Incharge { get; set; }
        [Required]
        public string? Address { get; set; }
        [Display(Name = "Profit Center")]
        public string? ProfitCenter { get; set; }
        [Display(Name = "Cost Center")]
        public string? CostCenter { get; set; }
        [Display(Name = "Location Type")]
        [ValidateNever]
        public int? LocationTypeId { get; set; }
        [ForeignKey("LocationTypeId")]
        [ValidateNever]
        public LocationType LocationType { get; set; }
    }
}

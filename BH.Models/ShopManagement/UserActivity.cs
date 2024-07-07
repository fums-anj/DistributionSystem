using BH.Models.OrganizationManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.ShopManagement
{
    public class UserActivity : Common
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string TransactionType { get; set; }
        [Required]
        public string RefDocNum { get; set; }
        [Required]
        public string EntryNum { get; set; }
        [Required]
        public string ActionType { get; set; }
        [Required]
        public string ActionedBy { get; set; }

        [Display(Name = "Location")]
        public int LocationId { get; set; }
        [ForeignKey("LocationId")]
        [ValidateNever]
        public Location Location { get; set; }
    }
}

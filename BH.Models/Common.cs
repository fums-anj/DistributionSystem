using BH.Models.OrganizationManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models
{
    public class Common
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Created By")]
        public string? CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual ApplicationUser? ApplicationUser { get; set; }
        [Display(Name = "Modified By")]
        public string? ModifiedBy { get; set; }
        [ForeignKey("ModifiedBy")]
        public virtual ApplicationUser? ModifyingUser { get; set; }
        [Display(Name = "Created Date")]
        [DataType(DataType.Date)]
        public DateTime? CreatedDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? ModifiedDate { get; set; }
        [Display(Name = "Deleted")]
        public bool IsDeleted { get; set; }
        [Display(Name = "Disabled")]
        public bool IsDisable { get; set; }

        [Display(Name = "Shop")]
        [ValidateNever]
        public int? ShopId { get; set; }
       
    }
}

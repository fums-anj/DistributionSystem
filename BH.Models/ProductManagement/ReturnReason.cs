using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.ProductManagement
{
    public class ReturnReason : Common
    {
        [Required]
        public string Name { get; set; }
    }
}

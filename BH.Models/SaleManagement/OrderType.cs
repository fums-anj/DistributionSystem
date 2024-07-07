using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.SaleManagement
{
    public class OrderType : Common
    {
        [Required]
        public string Name { get; set; }
    }
}

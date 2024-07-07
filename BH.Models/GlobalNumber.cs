using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models
{
    public class GlobalNumber
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ProficitCenterNum { get; set; }
        public int? CostCenterNum { get; set; }
        public int? SKU { get; set; }
        public int? VariantCode { get; set; }
        public int? CustomerCode { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}

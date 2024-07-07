using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.AccountManagement
{
    public class ExpenseYear : Common
    {
        [Required]
        [Display(Name = "Expense Year")]
        public string Name { get; set; }
    }
}

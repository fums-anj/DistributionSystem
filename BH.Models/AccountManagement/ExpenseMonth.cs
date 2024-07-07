using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.AccountManagement
{
    public class ExpenseMonth : Common
    {
        [Required]
        [Display(Name = "Expense Month")]
        public string Name { get; set; }
    }
}

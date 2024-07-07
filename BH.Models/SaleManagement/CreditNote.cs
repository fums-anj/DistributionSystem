using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.SaleManagement
{
    public class CreditNote : Common
    {
        [DataType(DataType.Currency)]
        public double? Cash { get; set; }
        public bool CashOut { get; set; }
    }
}

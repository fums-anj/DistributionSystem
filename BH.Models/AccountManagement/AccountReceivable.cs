using BH.Models.CustomerManagement;
using BH.Models.SaleManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Models.AccountManagement
{
    public class AccountReceivable : Common
    {
        [Display(Name = "Invoice No")]
        public int? SaleOrderId { get; set; }
        [ForeignKey("SaleOrderId")]
        [ValidateNever]
        public SaleOrder? SaleOrder { get; set; }
        [Display(Name = "Description")]
        public string? DocumentType { get; set; }
        [Display(Name = "Document Number")]
        public string? DocumentNum { get; set; }
        [Display(Name = "Customer")]
        public int? ShopCustomerId { get; set; }
        [ForeignKey("ShopCustomerId")]
        [ValidateNever]
        public ShopCustomer? ShopCustomer { get; set; }
        [Display(Name = "Receivable")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double TotalReceivable { get; set; }
        [Display(Name = "Received")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double ReceivedAmount { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Received Date")]
        public DateTime? ReceivedDate { get; set; }
    }
}

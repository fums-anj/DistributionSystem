using BH.Models.ProductManagement;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BH.Models.SaleManagement
{
    public class OrderDetail
    {
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }

        [Required]
        public int ShopProductId { get; set; }
        [ForeignKey("ShopProductId")]
        [ValidateNever]
        public ShopProduct ShopProduct { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BH.Models.ShopManagement
{
    public class Status : Common
    {
        [Required]
        public string Name { get; set; }

    }
}

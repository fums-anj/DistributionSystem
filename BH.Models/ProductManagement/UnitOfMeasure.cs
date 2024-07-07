using System.ComponentModel.DataAnnotations;

namespace BH.Models.ProductManagement
{
    public class UnitOfMeasure : Common
    {
        [Required]
        public string Name { get; set; }
        [Display(Name = "Base unit value")]
        [Range(0, double.MaxValue)]
        public double ValueInBaseUnit { get; set; }
        [Display(Name = "Measure in weight")]
        public bool IsWaight { get; set; }

    }
}

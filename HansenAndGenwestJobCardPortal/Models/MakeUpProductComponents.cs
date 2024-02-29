using System.ComponentModel.DataAnnotations;

namespace HansenAndGenwestJobCardPortal.Models
{
    public class MakeUpProductComponents
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ParentStockLink { get; set; } = "";
        [Required]
        public string StockLink { get; set; } = "";
        [Required]
        public string Code { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        [Required]
        public string Reference { get; set; } = "";
        [Required]
        public double Quantity { get; set; }
        [Required]
        public string Procedure { get; set; } = "";
        [Required]
        public double Price { get; set; }
        public bool UseSagePrice { get; set; } = true;




    }
}

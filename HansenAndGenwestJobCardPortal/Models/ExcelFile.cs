using System.ComponentModel;

namespace HansenAndGenwestJobCardPortal.Models
{
    public class ExcelFile
    {
        public string ItemNumber { get; set; } = "";
        public string Description { get; set; } = "";
        public string Description2 { get; set; } = "";
        public string Reference { get; set; } = "";
        public decimal Quantity { get; set; }
        public string Procedure { get; set; } = "";
        public decimal Price { get; set; }

        public string? ParentCode { get; set; }
        public string? ParentsParentCode { get; set; }
    }
}

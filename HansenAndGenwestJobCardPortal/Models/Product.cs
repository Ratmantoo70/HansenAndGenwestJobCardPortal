namespace HansenAndGenwestJobCardPortal.Models
{
    public class Product
    {
        public string StockLink { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool ItemActive { get; set; }
        public ProductType Type { get; set; } = ProductType.SageProduct;
    }
}

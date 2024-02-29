namespace HansenAndGenwestJobCardPortal.Models
{
    public class ImportHeader
    {
        public int ID { get; set; }
        public string Customer { get; set; } = "";
        public int CustomerID { get; set; }
        public string JobDescription { get; set; } = "";
        public string ExternalOrderNumber { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public string? JCNumber { get; set; }
        public int? JCID { get; set; }
    }
}

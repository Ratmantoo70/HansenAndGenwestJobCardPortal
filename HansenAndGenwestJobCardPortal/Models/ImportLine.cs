namespace HansenAndGenwestJobCardPortal.Models
{
    public class ImportLine
    {
        public int EvoID { get; set; }
        public string EvoCode { get; set; }
        public string EvoDesc { get; set; }
        public string ExcelCode { get; set; }
        public string ExcelDesc { get; set; }
        public double ExcelPrice { get; set; }
        public double EvoPrice { get; set; }
        public double Qty { get; set; }
        public string Procedure { get; set; }
        public string Reference { get; set; }
        public int ID { get; set; }
        public int JCLine { get; set; }
        public double LatestCost { get; set; }
        public string? ParentCode { get; set; }
        public string? ParentsParentCode { get; set; }
    }
}

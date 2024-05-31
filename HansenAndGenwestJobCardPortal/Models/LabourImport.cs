namespace HansenAndGenwestJobCardPortal.Models
{
    public class LabourImport
    {
        public int JobNo { get; set; }
        public string ItemNo { get; set; } = "";
        public string Reference { get; set; } = "";
        public string Procedure { get; set; } = "";
        public int Hours { get; set; }
    }
}

namespace HansenAndGenwestJobCardPortal.Models
{
    public class LabourExportResult
    {
        public int JobNo { get; set; }
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "";
    }
}

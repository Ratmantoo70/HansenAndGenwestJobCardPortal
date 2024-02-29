namespace HansenAndGenwestJobCardPortal.Models
{
    public class SageClient
    {
        // DCLink, Account, Name, On_Hold, , Physical2, Physical3, Physical4, Physical5, PhysicalPC, Post1, Post2, Post3, Post4, Post5, PostPC
        public int DCLink { get; set; }
        public string Account { get; set; } = "";
        public string Name { get; set; } = "";
        public bool On_Hold { get; set; }
        public string Physical1 { get; set; } = "";
        public string Physical2 { get; set; } = "";
        public string Physical3 { get; set; } = "";
        public string Physical4 { get; set; } = "";
        public string Physical5 { get; set; } = "";
        public string PhysicalPC { get; set; } = "";
        public string Post1 { get; set; } = "";
        public string Post2 { get; set; } = "";
        public string Post3 { get; set; } = "";
        public string Post4 { get; set; } = "";
        public string Post5 { get; set; } = "";
        public string PostPC { get; set; } = "";
    }
}

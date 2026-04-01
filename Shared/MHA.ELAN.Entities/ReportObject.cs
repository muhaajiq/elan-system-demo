namespace MHA.ELAN.Entities
{
    public class ViewModelReportListing
    {
        public bool IsAuthorized { get; set; }
        public Dictionary<string, string> ReportLinks { get; set; }

        public ViewModelReportListing()
        {
            IsAuthorized = false;
            ReportLinks = new Dictionary<string, string>();
        }
    }
}
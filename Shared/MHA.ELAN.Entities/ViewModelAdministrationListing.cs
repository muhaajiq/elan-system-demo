namespace MHA.ELAN.Entities
{
    public class ViewModelAdministrationListing
    {
        public bool IsAuthorized { get; set; }

        //Project Settings
        public string ReportURL { get; set; }

        //Running Number Settings
        public string RunningNumberFormatURL { get; set; }
        public string RunningNumberURL { get; set; }

        //Master Data List
        public string CompanyURL { get; set; }
        public string LocationURL { get; set; }
        public string GradeURL { get; set; }
        public string DepartmentURL { get; set; }
        public string DesignationURL { get; set; }
        public string ITRequirementsURL { get; set; }
        public string HardwareURL { get; set; }

        public ViewModelAdministrationListing()
        {
            IsAuthorized = false;
        }
    }
}

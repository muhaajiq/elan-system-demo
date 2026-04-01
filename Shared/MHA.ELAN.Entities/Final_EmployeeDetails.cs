using System.Runtime.Serialization;

namespace MHA.ELAN.Entities
{
    [Serializable]
    [DataContract]
    public class Final_EmployeeDetails
    {
        public int? ID { get; set; }
        public int? CompanyID { get; set; }
        public string CompanyTitle { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;
        public string EmployeeLogin { get; set; } = string.Empty;
        public string EmployeeStatus { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public int? DesignationID { get; set; }
        public string DesignationTitle { get; set; } = string.Empty;
        public int? GradeID { get; set; }
        public string GradeTitle { get; set; } = string.Empty;
        public int? DepartmentID { get; set; }
        public string DepartmentTitle { get; set; } = string.Empty;
        public string ReportingManagerEmail { get; set; } = string.Empty;
        public string ReportingManagerLogin { get; set; } = string.Empty;
        public string ReportingManagerName { get; set; } = string.Empty;
        public int? LocationID { get; set; }
        public string LocationTitle { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string ContractOrTemporaryStaff { get; set; } = string.Empty;
        public DateTime? JoinDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description1 { get; set; } = string.Empty;
        public string Description2 { get; set; } = string.Empty;
        public string Description3 { get; set; } = string.Empty;
        public string Description4 { get; set; } = string.Empty;
        public string Description5 { get; set; } = string.Empty;
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByLogin { get; set; } = string.Empty;
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByLogin { get; set; } = string.Empty;

        // New Reporting Manager (for reassignment)
        public PeoplePickerUser NewReportingManager { get; set; } = new();
        public string NewReportingManagerLogin { get; set; } = string.Empty;
        public string NewReportingManagerName { get; set; } = string.Empty;
        public string NewReportingManagerEmail { get; set; } = string.Empty;
    }
}

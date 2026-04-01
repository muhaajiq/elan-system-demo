using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MHA.ELAN.Entities
{
    [Serializable]
    [DataContract]
    public class EmployeeDetails
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int? CompanyID { get; set; }

        [DataMember]
        public string CompanyTitle { get; set; }

        [DataMember]
        public List<DropDownListItem> CompanyList { get; set; } = new();

        [DataMember]
        public string EmployeeName { get; set; }

        [DataMember]
        public string EmployeeEmail { get; set; }

        [DataMember]
        public string EmployeeLogin { get; set; }

        [DataMember]
        public string EmployeeStatus { get; set; }

        [DataMember]
        public string EmployeeID { get; set; }

        [DataMember]
        public int? DesignationID { get; set; }

        [DataMember]
        public string DesignationTitle { get; set; }

        [DataMember]
        public List<DropDownListItem> DesignationList { get; set; } = new();

        [DataMember]
        public int? GradeID { get; set; }

        [DataMember]
        public string GradeTitle { get; set; }

        [DataMember]
        public List<DropDownListItem> GradeList { get; set; } = new();

        [DataMember]
        public int? DepartmentID { get; set; }

        [DataMember]
        public string DepartmentTitle { get; set; }

        [DataMember]
        public List<DropDownListItem> DepartmentList { get; set; } = new();

        [DataMember]
        public string ReportingManagerEmail { get; set; }

        [DataMember]
        public string ReportingManagerLogin { get; set; }

        [DataMember]
        public string ReportingManagerName { get; set; }

        [DataMember]
        public int? LocationID { get; set; }

        [DataMember]
        public string LocationTitle { get; set; }

        [DataMember]
        public List<DropDownListItem> LocationList { get; set; } = new();

        [DataMember]
        public string MobileNo { get; set; }

        [DataMember]
        public string ContractTemporaryStaff { get; set; }

        [DataMember]
        public List<DropDownListItem> ContractTypeList { get; set; } = new();

        [DataMember]
        public DateTime? JoinedDate { get; set; } = DateTime.Now.Date;

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public string Description1 { get; set; }

        [DataMember]
        public string Description2 { get; set; }

        [DataMember]
        public string Description3 { get; set; }

        [DataMember]
        public string Description4 { get; set; }

        [DataMember]
        public string Description5 { get; set; }

        [DataMember]
        public string Remarks { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public string CreatedByLogin { get; set; }

        [DataMember]
        public DateTime? Created { get; set; }

        [DataMember]
        public string ModifiedBy { get; set; }

        [DataMember]
        public string ModifiedByLogin { get; set; }

        [DataMember]
        public DateTime? Modified { get; set; }

        [DataMember]
        public PeoplePickerUser ReportingManager { get; set; }

        [DataMember]
        public PeoplePickerUser Employee { get; set; }

        //Acknoledgement
        public bool Acknowledgement1 { get; set; }
        public bool Acknowledgement2 { get; set; }

        //Termination
        public List<ViewModelSubordinate> Subordinates { get; set; } = new();

        // New Reporting Manager (for reassignment)
        public PeoplePickerUser NewReportingManager { get; set; } = new();
        public string NewReportingManagerLogin { get; set; } = string.Empty;
        public string NewReportingManagerName { get; set; } = string.Empty;
        public string NewReportingManagerEmail { get; set; } = string.Empty;

        public int? TempFinalEmployeeDetailsID { get; set; }

        //Termination
        public DateTime? HardwareFinalReturnDate { get; set; }
        public bool HardwareReturnConfirmed { get; set; }

    }
}

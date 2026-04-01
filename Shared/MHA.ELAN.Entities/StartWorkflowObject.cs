using System.Runtime.Serialization;

namespace MHA.ELAN.Entities
{
    public class StartWorkflowObject
    {
        public string RequestType { get; set; }

        #region WF Actioners
        public List<PeoplePickerUser> ReportingManager { get; set; }

        public PeoplePickerUser Originator { get; set; }

        public List<PeoplePickerUser> InfraTeam { get; set; }

        public List<PeoplePickerUser> ApplicationTeam { get; set; }

        public List<PeoplePickerUser> ITManager { get; set; }

        public List<PeoplePickerUser> DepartmentAdmin { get; set; }

        public PeoplePickerUser Recipient { get; set; }
        #endregion

        #region WF Actioner Due Days
        public int PendingOriginatorResubmissionDueDays { get; set; }

        public int PendingReportingManagerDueDays { get; set; }

        public int PendingInfraTeamDueDays { get; set; }

        public int PendingApplicationTeamDueDays { get; set; }

        public int PendingITManagerDueDays { get; set; }

        public int PendingDepartmentAdminDueDays { get; set; }

        public int PendingRecipientDueDays { get; set; }

        public bool UseDefaultDueDays { get; set; }
        #endregion

        #region WF Actioner Due Dates
        public string PendingOriginatorResubmissionDueDate { get; set; }

        public string PendingReportingManagerDueDate { get; set; }

        public string PendingInfraTeamDueDate { get; set; }

        public string PendingApplicationTeamDueDate { get; set; }

        public string PendingITManagerDueDate { get; set; }

        public string PendingDepartmentAdminDueDate { get; set; }

        public string PendingRecipientDueDate { get; set; }
        #endregion

        [DataMember]
        public int ProcessId { get; set; }

        public StartWorkflowObject()
        {
            ReportingManager = new List<PeoplePickerUser>();
            InfraTeam = new List<PeoplePickerUser>();
            ApplicationTeam = new List<PeoplePickerUser>();
            ITManager = new List<PeoplePickerUser>();
            DepartmentAdmin = new List<PeoplePickerUser>();
        }
    }
}

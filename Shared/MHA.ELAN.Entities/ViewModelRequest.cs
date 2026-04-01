using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewModelRequest
    {
        public ViewModelRequest()
        {
            ID = null;
            EmployeeDetails = new ViewModelEmployeeDetails();
            ITRequirementsVM = new ViewModelITRequirements();
            HardwareVM = new ViewModelHardware();
            FinalEmployeeDetails = new ViewModelFinalEmployeeDetails();
            FinalFolderPermission = new List<ViewModelFinalFolderPermission>();
            FinalHardware = new List<ViewModelFinalHardware>();
            ReferenceNo = string.Empty;
            WorkflowStatus = string.Empty;
            ProcessID = null;
            TaskID = null;
            Changes = string.Empty;
            Created = null;
            CreatedBy = string.Empty;
            CreatedByLogin = string.Empty;
            Modified = null;
            ModifiedBy = string.Empty;
            ModifiedByLogin = string.Empty;
            HasError = false;
            ErrorMessage = string.Empty;
            HasSuccess = false;
            SuccessMessage = string.Empty;
            RequestTypeList = new List<DropDownListItem>();
            RequestType = string.Empty;
            RMEscalation = false;
        }

        public ViewModelEmployeeDetails EmployeeDetails { get; set; } = new ViewModelEmployeeDetails();
        public ViewModelITRequirements ITRequirementsVM { get; set; } = new ViewModelITRequirements();
        public ViewModelHardware HardwareVM { get; set; } = new ViewModelHardware();

        public ViewModelFinalEmployeeDetails FinalEmployeeDetails { get; set; } = new ViewModelFinalEmployeeDetails();
        public List<ViewModelFinalFolderPermission> FinalFolderPermission { get; set; } = new List<ViewModelFinalFolderPermission>();
        public List<ViewModelFinalHardware> FinalHardware { get; set; } = new List<ViewModelFinalHardware>();

        [DataMember]
        public int? ID { get; set; }

        [DataMember]
        public string ReferenceNo { get; set; }

        [DataMember]
        public string WorkflowStatus { get; set; }

        [DataMember]
        public int? ProcessID { get; set; }

        [DataMember]
        public int? TaskID { get; set; }

        [DataMember]
        public string Changes { get; set; }

        [DataMember]
        public DateTime? Created { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public string CreatedByLogin { get; set; }

        [DataMember]
        public DateTime? Modified { get; set; }

        [DataMember]
        public string ModifiedBy { get; set; }

        [DataMember]
        public string ModifiedByLogin { get; set; }

        [DataMember]
        public List<DropDownListItem> RequestTypeList { get; set; }

        [DataMember]
        public string RequestType { get; set; }

        [DataMember]
        public bool RMEscalation { get; set; }

        [DataMember]
        public DateTime Submitted { get; set; }

        [DataMember]
        public string SubmittedBy { get; set; }

        [DataMember]
        public string SubmittedByLogin { get; set; }
        
        [DataMember]
        public string SubmittedByEmail { get; set; }

        [DataMember]
        public bool ModifyEmployeeDetails { get; set; }
        [DataMember]
        public bool ModifyITRequirements { get; set; }
        [DataMember]
        public bool ModifyHardware { get; set; }


        #region Common VM Properties

        [DataMember]
        public bool HasError { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public bool HasSuccess { get; set; }

        [DataMember]
        public string SuccessMessage { get; set; }

        #endregion
    }

}

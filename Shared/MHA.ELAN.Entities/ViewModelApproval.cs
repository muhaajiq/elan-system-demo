using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewModelApproval
    {
        public int? RequestID { get; set; }
        public int? ProcessID { get; set; }
        public int? TaskID { get; set; }
        public ViewRequestItem ViewRequestItem { get; set; } = new ViewRequestItem();
        public ViewModelEmployeeDetails EmployeeDetails { get; set; } = new();
        public ViewModelFinalEmployeeDetails FinalEmployeeDetails { get; set; } = new();
        public List<ViewModelITRequirements> ITRequirements { get; set; } = new();
        public List<ViewModelFinalITRequirements> FinalITRequirements { get; set; } = new();
        public List<ViewModelFolderPermission> FolderPermissions { get; set; } = new();
        public List<ViewModelFinalFolderPermission> FinalFolderPermissions { get; set; } = new();
        public List<ViewModelHardware> HardwareItems { get; set; } = new();
        public List<ViewModelFinalHardware> FinalHardwareItems { get; set; } = new();
        public PartialModelWorkflowHistory PartialModelWorkflowHistory { get; set; } = new();
        public PartialModelAdminWorkflowHistory PartialModelAdminWorkflowHistory { get; set; } = new();
        public List<ViewChangesItem> changes { get; set; } = new();
        public string CurrentUser { get; set; } = string.Empty;
        public List<string> AccessGroups { get; set; } = new();
        public List<string> AccessDepartments { get; set; } = new();
        public bool IsMyTask { get; set; } = false;
        public string CurrentStage { get; set; } = string.Empty;
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public bool HasSuccess { get; set; }
        public string SuccessMessage { get; set; } = string.Empty;
        public bool IsLoaded { get; set; } = false;

        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByLogin { get; set; } = string.Empty;
        public DateTime? Created { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByLogin { get; set; } = string.Empty;
        public DateTime? Modified { get; set; }

        //Workflow
        public int ApproverTaskDueDay { get; set; }
    }
}

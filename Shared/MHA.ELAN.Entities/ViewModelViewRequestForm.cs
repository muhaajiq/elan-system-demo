namespace MHA.ELAN.Entities
{
    public class ViewModelViewRequestForm
    {
        public int? RequestID { get; set; }

        public ViewRequestItem RequestItem { get; set; } = new();
        public ViewModelEmployeeDetails EmployeeDetailsItem { get; set; } = new();
        public List<ViewModelITRequirements> ITRequirementsItems { get; set; } = new();
        public List<ViewModelHardware> HardwareItems { get; set; } = new();
        public List<ViewModelFolderPermission> FolderPermissionsItems { get; set; } = new();
        public List<ViewChangesItem> ChangesItems { get; set; } = new();
        public PartialModelWorkflowHistory PartialModelWorkflowHistory { get; set; } = new();
        public PartialModelAdminWorkflowHistory PartialModelAdminWorkflowHistory { get; set; } = new();
        //Permission checking
        public string CurrentUser { get; set; } = string.Empty;
        public List<string> AccessGroups { get; set; } = new();
        public List<int> AccessDepartments { get; set; } = new();
        public bool IsValid { get; set; } = false;
        public bool IsLoaded { get; set; } = false;

        public string CurrentStage { get;set; } = string.Empty;
    }
}

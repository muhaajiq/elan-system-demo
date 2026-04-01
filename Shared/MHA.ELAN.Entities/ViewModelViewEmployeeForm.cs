namespace MHA.ELAN.Entities
{
    public class ViewModelViewEmployeeForm
    {
        public int? EmployeeID { get; set; }
        public ViewFinalEmployeeDetailsItem EmployeeDetailsItem { get; set; } = new();

        public List<ViewRequestItem> RequestItem { get; set; } = new();
        public List<ViewFinalITRequirementsItem> ITRequirementsItems { get; set; } = new();
        public List<ViewFinalHardwareItem> HardwareItems { get; set; } = new();
        public List<ViewFinalFolderPermission> FolderPermissionsItems { get; set; } = new();
        //Permission checking
        public string CurrentUser { get; set; } = string.Empty;
        public List<string> AccessGroups { get; set; } = new();
        public List<int> AccessDepartments { get; set; } = new();
        public bool IsValid { get; set; } = false;
        public bool IsLoaded { get; set; } = false;
    }
}

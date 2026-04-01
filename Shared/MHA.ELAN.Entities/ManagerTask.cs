namespace MHA.ELAN.Entities
{
    public class ManagerTask
    {
        public int ProcessId { get; set; }
        public string ProcessName { get; set; } = string.Empty;
        public string StepName { get; set; } = string.Empty;
        public string InternalStepName { get; set; } = string.Empty;
        public string TaskStatus { get; set; } = string.Empty;
        public int TaskID { get; set; }
        public string AssigneeLogin { get; set; } = string.Empty;
        public string AssigneeName { get; set; } = string.Empty;
        public string AssigneeEmail { get; set; } = string.Empty;
        public string AssignedDate { get; set; } = string.Empty;
        public int RequestID { get; set; }
        public string ManagerLogin { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public string ManagerEmail { get; set; } = string.Empty;
    }
}

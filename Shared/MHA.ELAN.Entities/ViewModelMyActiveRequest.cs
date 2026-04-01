namespace MHA.ELAN.Entities
{
    public class ViewModelMyActiveRequest
    {
        public int PageNumber { get; set; }
        public int Count { get; set; }
        public int RowsPerPage { get; set; }
        public List<MyActiveRequestData>? Request { get; set; } = new();
    }

    public class MyActiveRequestData
    {
        public int RequestID { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string WorkflowStatus { get; set; } = string.Empty;
        public DateTime? Submitted { get; set; }
    }
}

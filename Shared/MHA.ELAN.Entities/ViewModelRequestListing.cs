using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewModelRequestListing
    {
        public bool ShowInactiveItems { get; set; }
        public string CurrentUser { get; set; } = string.Empty;
        public string CurrentUserLogin { get; set; } = string.Empty;
        public List<string> AccessGroups { get; set; } = new();
        public List<int> AccessDepartments { get; set; } = new();
        public string MemberLogin { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public int Count { get; set; }
        public int RowsPerPage { get; set; }
        public string SortField { get; set; } = string.Empty;
        public string SortFieldTable { get; set; } = string.Empty;
        public string SortDirection { get; set; } = string.Empty;
        public RequestListingSearchModel searchModel = new();
        public List<RequestListingData> RequestListing = new();
    }

    public class RequestListingData
    {
        public int RequestID { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string WorkflowStatus { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string CreatedByLogin { get; set; } = string.Empty;
    }

    public class RequestListingSearchModel
    {
        public string ReferenceNo { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty;
        public List<int> Designation { get; set; } = new();
        public List<int> Department { get; set; } = new();
        public DateTime? CreatedStartDate { get; set; }
        public DateTime? CreatedEndDate { get; set; }
        public DateTime? SubmittedStartDate { get; set; }
        public DateTime? SubmittedEndDate { get; set; }
        public List<string> WorkflowStatus { get; set; } = new();
        public List<string> RequestType { get; set; } = new();
    }
}

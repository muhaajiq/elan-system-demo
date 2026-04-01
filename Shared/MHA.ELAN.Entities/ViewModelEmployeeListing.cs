using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewModelEmployeeListing
    {
        public bool ShowInactiveItems { get; set; }
        public string CurrentUser { get; set; } = string.Empty;
        public List<string> AccessGroups { get; set; } = new();
        public List<int> AccessDepartments { get; set; } = new();
        //public string PermissionFilter { get; set; } = string.Empty ;
        public string MemberLogin { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public int Count { get; set; }
        public int RowsPerPage { get; set; }
        public string SortField { get; set; } = string.Empty;
        public string SortFieldTable { get; set; } = string.Empty;
        public string SortDirection { get; set; } = string.Empty;
        public EmployeeListingSearchModel searchModel = new();
        public List<EmployeeListingData> EmployeeListing = new();
    }

    public class EmployeeListingData
    {
        public int? ID { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string Grade { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string ReportingManager { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime? JoinedDate { get; set; }
        public string ContractOrPermanent { get; set; } = string.Empty;
        public string EmployeeStatus { get; set; } = string.Empty;
    }

    public class EmployeeListingSearchModel
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public List<int> Designation { get; set; } = new();
        public List<int> Grade { get; set; } = new();
        public List<int> Department { get; set; } = new();
        public string ReportingManager { get; set; } = string.Empty;
        public List<int> Location { get; set; } = new();
        public DateTime? JoinedStartDate { get; set; }
        public DateTime? JoinedEndDate { get; set; }
        public List<string> ContractOrPermanent { get; set; } = new();
        public List<string> EmployeeStatus { get; set; } = new();
    }
}

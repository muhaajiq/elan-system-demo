using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewFinalEmployeeDetailsItem
    {
        public int? ID { get; set; }
        public string CompanyTitle { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeLogin { get; set; } = string.Empty;
        public string EmployeeID { get; set; } = string.Empty;
        public string DesignationTitle { get; set; } = string.Empty;
        public string GradeTitle { get; set; } = string.Empty;
        public int? DepartmentID { get; set; }
        public string DepartmentTitle { get; set; } = string.Empty;
        public string ReportingManagerName { get; set; } = string.Empty;
        public string LocationTitle { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string ContractOrTemporaryStaff { get; set; } = string.Empty;
        public DateTime? JoinDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description1 { get; set; } = string.Empty;
        public string Description2 { get; set; } = string.Empty;
        public string Description3 { get; set; } = string.Empty;
        public string Description4 { get; set; } = string.Empty;
        public string Description5 { get; set; } = string.Empty;
    }
}

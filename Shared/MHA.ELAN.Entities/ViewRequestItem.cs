using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewRequestItem
    {
        public int? RequestID { get; set; }
        public int? FinalEmpID { get; set; }
        public int? ProcessID { get; set; }
        public string ReferenceNo { get; set; } = string.Empty;
        public string WorkflowStatus { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string CreatedDate { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public bool ModifyEmployeeDetails { get; set; }
        public bool ModifyITRequirements { get; set; }
        public bool ModifyHardware { get; set; }
    }
}

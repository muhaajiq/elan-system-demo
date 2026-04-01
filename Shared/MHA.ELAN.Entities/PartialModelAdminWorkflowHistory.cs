using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class PartialModelAdminWorkflowHistory
    {
        public List<WorkflowHistory> WorkflowHistoryList { get; set; } = new();
        public bool ShowManagement { get; set; } = false;
        public string ProcessID { get; set; }
        public string TxtNewActioner { get; set; }
        public bool isWFRunnning { get; set; } = false;
        public string Remark { get; set; }
        public string WorkflowDueDate { get; set; }
        //public bool IsCommentCompiler { get; set; }
        //public bool IsCommentApprover { get; set; }
        public bool IsInProgress { get; set; }
        //public int RestrictGroupID { get; set; }

        //public PartialModelAdminWorkflowHistory()
        //{
        //    WorkflowHistoryList = new List<WorkflowHistory>();
        //    ShowManagement = false;
        //    IsWFRunnning = false;
        //}
    }
}

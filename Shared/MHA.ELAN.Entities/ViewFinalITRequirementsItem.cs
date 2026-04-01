using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewFinalITRequirementsItem
    {
        public int? ID { get; set; }
        public int? FinalEmployeeDetailsID { get; set; }
        public string ItemTitle { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsAdded { get; set; } = false;
        public bool IsRemoved { get; set; } = false;
        public DateTime? DateAdded { get; set; }
        public DateTime? DateRemoved { get; set; }
        public string Remark { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewFinalHardwareItem
    {
        public int? ID { get; set; }
        public int? FinalEmployeeDetailsID { get; set; }
        public string DepartmentTitle { get; set; } = string.Empty;

        public int? ItemID { get; set; }

        public string ItemTitle { get; set; } = string.Empty;

        public int Quantity { get; set; } = 1;

        public string Remarks { get; set; } = string.Empty;
    }
}

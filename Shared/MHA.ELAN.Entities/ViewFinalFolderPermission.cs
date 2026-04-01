using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewFinalFolderPermission
    {
        public int? ID { get; set; }
        public int? FinalEmployeeDetailsID { get; set; }
        public string NameOrPath { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public bool IsWrite { get; set; } = false;
        public bool IsDelete { get; set; } = false;
        public string Status { get; set; } = string.Empty;
        public bool IsAdded { get; set; } = false;
        public bool IsRemoved { get; set; } = false;
        public DateTime? DateAdded { get; set; }
        public DateTime? DateRemoved { get; set; }
        public string Remark { get; set; } = string.Empty;
    }
}

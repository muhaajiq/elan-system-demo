using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class Final_FolderPermission
    {
        public int? ID { get; set; }
        public int? FinalEmployeeDetailsID { get; set; }
        public string NameOrPath { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
        public bool IsDelete { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsAdded { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateRemoved { get; set; }
        public string Remark { get; set; } = string.Empty;
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByLogin { get; set; } = string.Empty;
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;  
        public string ModifiedByLogin { get; set; } = string.Empty;
    }
}

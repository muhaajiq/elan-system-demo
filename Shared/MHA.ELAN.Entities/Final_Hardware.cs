using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class Final_Hardware
    {
        public int? ID { get; set; }
        public int? FinalEmployeeDetailsID { get; set; }
        public int? DepartmentID { get; set; }
        public string DepartmentTitle { get; set; } = string.Empty;
        public int ItemID { get; set; }
        public string ItemTitle { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string RemarkHistory { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime? DateAssigned { get; set; }
        public bool IsReturned { get; set; }
        public DateTime? DateReturned { get; set; }
        public bool IsReceived { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByLogin { get; set; } = string.Empty;
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByLogin { get; set; } = string.Empty;
    }
}

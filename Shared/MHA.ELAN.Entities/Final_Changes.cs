using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class Final_Changes
    {
        [Key]
        public int ID { get; set; }
        public string ChangeDetails { get; set; } = string.Empty;
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByLogin { get; set; } = string.Empty;
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByLogin { get; set; } = string.Empty;

        public Final_EmployeeDetails? Final_EmployeeDetails { get; set; }
    }
}

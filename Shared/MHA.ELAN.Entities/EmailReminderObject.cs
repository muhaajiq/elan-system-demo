using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class EmailReminderObject
    {
        public string HRManagerName { get; set; }
        public string HRManagerEmail { get; set; }
        public int MonthsUntilExpiry { get; set; }
        public List<EmailReminderEmployeeDetails> EmployeeDetails { get; set; }
    }
}

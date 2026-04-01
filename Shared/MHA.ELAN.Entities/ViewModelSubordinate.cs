using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewModelSubordinate
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string ReportingManagerName { get; set; }
        public string ReportingManagerEmail { get; set; }
        public string ReportingManagerLogin { get; set; }
    }

}

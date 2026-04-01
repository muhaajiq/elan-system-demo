using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewModelFinalEmployeeDetails
    {
        public ViewModelFinalEmployeeDetails()
        {
            Final_EmployeeDetails = new Final_EmployeeDetails();
        }

        [DataMember]
        public Final_EmployeeDetails Final_EmployeeDetails { get; set; }
    }
}

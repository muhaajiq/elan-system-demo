using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    [Serializable]
    [DataContract]
    public class ViewModelEmployeeDetails
    {
        public ViewModelEmployeeDetails()
        {
            EmployeeDetailsVM = new EmployeeDetails();
        }

        [DataMember]
        public EmployeeDetails EmployeeDetailsVM { get; set; }

        public int? RequestID { get; set; }
        public string RequestType { get; set; } = string.Empty;

        #region Common VM Properties
        [DataMember]
        public bool HasError { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public bool HasSuccess { get; set; }

        [DataMember]
        public string SuccessMessage { get; set; }

        #endregion

        public string GetMissingRequiredFieldsMessage(ViewModelRequest RequestModel)
        {
            var details = EmployeeDetailsVM;

            if (RequestModel.RequestType?.Equals("Modification Request", StringComparison.OrdinalIgnoreCase) == true && !RequestModel.ModifyEmployeeDetails)
            {
                return string.Empty;
            }

            var missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(RequestModel.RequestType))
                missingFields.Add("Type");

            if (!details.CompanyID.HasValue || string.IsNullOrWhiteSpace(details.CompanyID.Value.ToString()))
                missingFields.Add("Company");

            if (string.IsNullOrWhiteSpace(details.EmployeeName))
                missingFields.Add("Employee Name");

            if (string.IsNullOrWhiteSpace(details.EmployeeID))
                missingFields.Add("Employee ID");

            if (string.IsNullOrWhiteSpace(details.DesignationTitle))
                missingFields.Add("Designation");

            if (!details.GradeID.HasValue || string.IsNullOrWhiteSpace(details.GradeID.Value.ToString()))
                missingFields.Add("Grade");

            if (!details.DepartmentID.HasValue || string.IsNullOrWhiteSpace(details.DepartmentID.Value.ToString()))
                missingFields.Add("Department");

            if (details.ReportingManager == null || string.IsNullOrWhiteSpace(details.ReportingManagerLogin))
                missingFields.Add("Reporting Manager");

            if (!details.LocationID.HasValue || string.IsNullOrWhiteSpace(details.LocationID.Value.ToString()))
                missingFields.Add("Location");

            if (string.IsNullOrWhiteSpace(details.MobileNo))
                missingFields.Add("Mobile No");

            if (string.IsNullOrWhiteSpace(details.ContractTemporaryStaff))
                missingFields.Add("Contract/Temporary Staff");

            if (!details.JoinedDate.HasValue)
                missingFields.Add("Joined Date");

            if (details.EndDate.HasValue && details.JoinedDate.HasValue && details.EndDate < details.JoinedDate)
                missingFields.Add("End Date must be after Joined Date");

            // Require End Date if Termination Request OR Contract/Temporary Staff
            if ((RequestModel.RequestType?.Equals("Termination Request", StringComparison.OrdinalIgnoreCase) == true
                    || (!string.IsNullOrWhiteSpace(details.ContractTemporaryStaff) &&
                        details.ContractTemporaryStaff != "Permanent Staff"))
                && !details.EndDate.HasValue)
            {
                missingFields.Add("End Date (required for Contract/Temporary Staff or Termination Request)");
            }

            if (missingFields.Count == 0)
                return string.Empty;

            return "Please fill in the following required fields before saving:<br /><br />" +
                string.Join("<br />", missingFields.Select(f => $"• {f}"));
        }
    }
}

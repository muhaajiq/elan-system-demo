using MHA.Framework.Core.SP;
using MHA.Framework.Core.Workflow.BO;
using MHA.ELAN.Data;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using MHA.ELAN.Framework.JSONConstants;
using Azure.Core;
using Microsoft.Graph;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Business
{
    public class EmailReminderBL
    {
        private static readonly JSONAppSettings appSettings;

        static EmailReminderBL()
        {
            appSettings = ConfigurationManager.GetAppSetting();
        }

        public EmailReminderObject InitReminderEmailObj(int remainingMonths, string spHostUrl, string accessToken)
        {
            EmailReminderObject obj = new();

            obj.MonthsUntilExpiry = remainingMonths;

            Microsoft.SharePoint.Client.User departmentAdmin = GetDepartmentAdmin(spHostUrl, accessToken);

            obj.HRManagerName = departmentAdmin.Title;
            obj.HRManagerEmail = departmentAdmin.Email;

            obj.EmployeeDetails = RetrieveExpiringContractEmployees(remainingMonths);

            return obj;
        }

        public Microsoft.SharePoint.Client.User GetDepartmentAdmin(string spHostUrl, string accessToken)
        {
            using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
            {
                string queryCondition = GeneralQueryHelper.ConcatCriteria(null, ConstantHelper.SPColumn.Department.Title, "Text", ConstantHelper.SPColumnValue.Department.Title.HRDepartment, "Eq", true);

                Microsoft.SharePoint.Client.ListItem item = GeneralQueryHelper.GetSPItem(clientContext, ConstantHelper.SPList.Department, queryCondition, null);

                FieldUserValue? adminUserValue = item[ConstantHelper.SPColumn.Department.Admin] as FieldUserValue;

                if (adminUserValue == null) throw new Exception("Missing HR Department Admin.");

                Microsoft.SharePoint.Client.User? HRadminUser = SharePointHelper.GetUserById(clientContext, adminUserValue.LookupId);

                return HRadminUser;
            }
        }

        public List<EmailReminderEmployeeDetails> RetrieveExpiringContractEmployees(int remainingMonths)
        {
            List<EmailReminderEmployeeDetails> model = new();

            EmailReminderDA reminderEmailDA = new();

            DataTable DT = reminderEmailDA.RetrieveExpiringContractEmployees(remainingMonths);

            model = ConvertEntitiesHelper.ConvertReminderEmailObject(DT);

            return model;
        }

        public void SendEmployeeContractExpiryReminderEmail(string emailTitle, EmailReminderObject obj, string spHostURL, string accessToken)
        {
            string emailSubject = string.Empty;
            string emailBody = string.Empty;
            string employeeTableHtml = string.Empty;

            EmailTemplateBL.GetEmailTemplateByTemplateTitle(emailTitle, ref emailSubject, ref emailBody, spHostURL, accessToken);
            if (!string.IsNullOrEmpty(emailSubject) && !string.IsNullOrEmpty(emailBody))
            {
                //Initialize the CC Email Address Collection
                MailAddressCollection emailCC = new MailAddressCollection();

                //Build email subject
                if (emailTitle.Equals(ConstantHelper.EmailTemplateKeyTitle.EmployeeContractExpirationReminder))
                {
                    employeeTableHtml = BuildEmployeeDetailsTable(obj);
                    emailBody = BuildEmailBody(emailBody, employeeTableHtml, obj);
                }
                else throw new Exception("Invalid email notification title has been selected");

                MailAddressCollection emailTo = new MailAddressCollection();
                if (!string.IsNullOrEmpty(obj.HRManagerEmail))
                    emailTo.Add(new MailAddress(obj.HRManagerEmail));

                EmailHelper.SendEmailWithSender(appSettings.rerAccountEmail, appSettings.rerAccountPassword, emailTo, emailCC, emailSubject, emailBody, new List<System.Net.Mail.Attachment>());
            }
        }

        public void UpdateIsReminderEmailSentFlag(EmailReminderObject obj)
        {
            EmailReminderDA reminderEmailDA = new();

            bool isSuccess = false;

            List<int> employeeIDs = new();

            foreach (EmailReminderEmployeeDetails item in obj.EmployeeDetails)
            {
                employeeIDs.Add(item.ID);
            }

            isSuccess = reminderEmailDA.UpdateIsReminderEmailSentFlagByEmployeeID(employeeIDs);
        }

        private static string BuildEmployeeDetailsTable(EmailReminderObject obj)
        {
            string employeeDetailsHtml = string.Empty;

            if (obj?.EmployeeDetails != null && obj.EmployeeDetails.Any())
            {
                employeeDetailsHtml = @"
                <table border='1' cellspacing='0' cellpadding='5' style='border-collapse:collapse;'>
                    <tr>
                        <th>No</th>
                        <th>Employee Name</th>
                        <th>Designation</th>
                        <th>Department</th>
                        <th>Contract End Date</th>
                    </tr>";

                int no = 1;

                foreach (var item in obj.EmployeeDetails)
                {
                    employeeDetailsHtml += $@"
                    <tr>
                        <td>{no++}</td>
                        <td>{item.EmployeeName}</td>
                        <td>{item.Designation}</td>
                        <td>{item.Department}</td>
                        <td>{item.ContractEndDate}</td>
                    </tr>";
                }

                employeeDetailsHtml += "</table>";
            }

            return employeeDetailsHtml;
        }

        private static string BuildEmailBody(string emailBody, string employeeTableHtml, EmailReminderObject obj)
        {
            emailBody = emailBody.Replace("{HRManagerName}", obj.HRManagerName);
            emailBody = emailBody.Replace("{MonthsUntilExpiry}", obj.MonthsUntilExpiry.ToString());
            emailBody = emailBody.Replace("{EmployeeDetailsTable}", employeeTableHtml);

            return emailBody;
        }
    }
}

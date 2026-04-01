using MHA.Framework.Core.Workflow.BO;
using MHA.ELAN.Data;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Helpers;
using MHA.ELAN.Framework.JSONConstants;
using System.Data;
using System.Globalization;

namespace MHA.ELAN.TSCH.AutoAssignManager
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            JSONAppSettings appSettings = ConfigurationManager.GetAppSetting();
            AutoAssignManagerTask(appSettings);
        }
        
        public static void AutoAssignManagerTask(JSONAppSettings appSettings)
        {
            // 1. Calculate the timestamp for 24 hours ago
            string fromDateStr = DateTime.UtcNow.AddHours(-24).ToString(appSettings.DefaultDateTimeWithSecondFormat);
            DateTime fromDate = DateTime.MinValue;
            DateTime.TryParseExact(fromDateStr, appSettings.DefaultDateTimeWithSecondFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate);

            // 2. Retrieve tasks that meet the criteria
            WorkflowDA workflowDA = new WorkflowDA();
            DataTable dt = workflowDA.GetManagerTask(fromDate);
            List<ManagerTask> managerTaskList = ConvertEntitiesHelper.ConvertManagerTaskObj(dt);

            // 3. Assign the task to the role manager's addedBy 
            if (managerTaskList != null && managerTaskList.Count > 0)
            {
                string ConnString = ConnectionStringHelper.GetGenericWFConnString();
                MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(ConnString);

                var actionByUser = new PeoplePickerUser();
                actionByUser.Login = appSettings.rerAccountLogin;
                actionByUser.Email = appSettings.rerAccountEmail;
                actionByUser.Name = appSettings.rerAccountName;
                var actionBy = WorkflowHelper.ConstructActioner(actionByUser);

                foreach (var task in managerTaskList)
                {
                    // Retrieve manager's manager details
                    PeoplePickerUser manager = new PeoplePickerUser();
                    manager.Login = task.ManagerLogin;
                    manager.Email = task.ManagerEmail;
                    manager.Name = task.ManagerName;

                    Actioner newManagerActioner = new Actioner();
                    newManagerActioner = WorkflowHelper.ConstructActioner(manager);

                    // Reassign task to manager's manager
                    ProcessInstance instance = wfBL.GetProcessInstance(task.ProcessId);
                    wfBL.AddOrReassignActionersForStep(task.ProcessId, instance.CurStepTemplateID, new List<Actioner> { newManagerActioner } , false, true, actionBy);

                    // Update escalation flag
                    RequestDA requestDA = new RequestDA();
                    requestDA.UpdateRMEscalationFlagForRequest(task.RequestID, true);
                }
            }
        }

        //public static string GetAccessTokenFromHybridApp(string SPHostURL)
        //{
        //    JSONAppSettings appsettings = ConfigurationManager.GetAppSetting();

        //    string accessToken = string.Empty;

        //    string tenantId = appsettings.TenantId;
        //    string clientId = appsettings.ClientId;
        //    string certificateName = appsettings.CertName;
        //    string certificatePath = "";
        //    string certificatePassword = appsettings.CertPassword;

        //    X509Certificate2 certificate2 = new X509Certificate2(certificatePath, certificatePassword);

        //    var am_1 = PnP.Framework.AuthenticationManager.CreateWithCertificate(clientId, certificate2, tenantId);
        //    accessToken = am_1.GetAccessToken(SPHostURL);

        //    return accessToken;

        //}
    }
}

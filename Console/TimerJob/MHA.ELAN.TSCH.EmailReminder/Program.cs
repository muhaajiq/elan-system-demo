using MHA.ELAN.Business;
using MHA.ELAN.Data;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using MHA.ELAN.Framework.JSONConstants;
using Microsoft.Graph;
using System.Data;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;

namespace MHA.ELAN.TSCH.EmployeeContractEndReminder
{
    public class Program
    {
        private static EmailReminderBL _reminderEmailBL = new();

        static void Main(string[] args)
        {
            JSONAppSettings appSettings = ConfigurationManager.GetAppSetting();
            LogHelper logHelper = new();

            try
            {
                SendExpiringEmployeeContractsEmailReminder(appSettings);
            }
            catch (Exception ex)
            {
                logHelper.LogMessage("Schedular - SendExpiringEmployeeContractsEmailReminder: " + ex.ToString());
            }
        }

        public static bool SendExpiringEmployeeContractsEmailReminder(JSONAppSettings appSettings)
        {
            EmailReminderObject obj = new();
            bool isSuccess = false;

            try
            {
                int monthsUntilReminder = int.Parse(appSettings.MonthsUntilContractExpiry);

                if (monthsUntilReminder == 0) throw new Exception("MonthsUntilContractExpiry must be greater than 0.");

                string accessToken = GetAccessTokenFromHybridApp(appSettings.SPHostUrl);

                obj = _reminderEmailBL.InitReminderEmailObj(monthsUntilReminder, appSettings.SPHostUrl, accessToken);

                if (obj.HRManagerEmail == null) throw new Exception("HR Department Manager not found.");

                if (obj.EmployeeDetails != null && obj.EmployeeDetails.Count > 0)
                    _reminderEmailBL.SendEmployeeContractExpiryReminderEmail(ConstantHelper.EmailTemplateKeyTitle.EmployeeContractExpirationReminder, obj, appSettings.SPHostUrl, accessToken);

                _reminderEmailBL.UpdateIsReminderEmailSentFlag(obj);

                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

            return isSuccess;
        }

        public static string GetAccessTokenFromHybridApp(string SPHostURL)
        {
            JSONAppSettings appsettings = ConfigurationManager.GetAppSetting();

            string accessToken = string.Empty;
            //TODO : Cert Path
            string relativeCertPath = Path.Combine(AppContext.BaseDirectory, appsettings.CertPath);
            string certificatePath = Path.GetFullPath(relativeCertPath);

            string tenantId = appsettings.TenantId;
            string clientId = appsettings.ClientId;
            string certificateName = appsettings.CertName;
            string certificatePassword = appsettings.CertPassword;
            X509Certificate2 certificate2 = new X509Certificate2(certificatePath, certificatePassword);

            var am_1 = PnP.Framework.AuthenticationManager.CreateWithCertificate(clientId, certificate2, tenantId);
            accessToken = am_1.GetAccessToken(SPHostURL);

            return accessToken;

        }
    }
}

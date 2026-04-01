using MHA.Framework.Core.SP;
using MHA.Framework.Core.Workflow.BL;
using MHA.Framework.Core.Workflow.BO;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.JSONConstants;
using Microsoft.SharePoint.Client;
using System.Data;
using System.Net;
using System.Text.RegularExpressions;

namespace MHA.ELAN.Framework.Helpers
{
    public static class ProjectHelperExtension
    {
        private static readonly JSONAppSettings appSettings;

        static ProjectHelperExtension()
        {
            appSettings = ConfigurationManager.GetAppSetting();
        }

        public static void ExecuteQueryWithIncrementalRetry(this ClientContext context, string sourceFunction = "")
        {
            int retryAttempts = 0;
            int backoffInterval = 500;
            int backoffIncrease = 100;
            int retryCount = 1;
            bool retry = false;
            ClientRequestWrapper wrapper = null;
            int.TryParse(appSettings.NoOfRetry, out retryCount);
            int.TryParse(appSettings.ThrottlingSleeptime, out backoffInterval);
            int.TryParse(appSettings.ThrottlingTimeIncrease, out backoffIncrease);

            if (retryCount <= 0)
                throw new ArgumentException("Provide a retry count greater than zero.");
            if (backoffInterval <= 0)
                throw new ArgumentException("Provide a delay greater than zero.");

            while (retryAttempts < retryCount)
            {
                try
                {
                    if (!retry)
                    {
                        context.ExecuteQuery();
                        return;
                    }
                    else
                    {
                        //increment the retry count
                        retryAttempts++;

                        // retry the previous request using wrapper
                        if (wrapper != null && wrapper.Value != null)
                        {
                            context.RetryQuery(wrapper.Value);
                            return;
                        }
                        // retry the previous request as normal
                        else
                        {
                            context.ExecuteQuery();
                            return;
                        }
                    }
                }
                catch (WebException wex)
                {
                    var response = wex.Response as HttpWebResponse;
                    if (response != null && (response.StatusCode == (HttpStatusCode)429 || response.StatusCode == (HttpStatusCode)503))
                    {
                        wrapper = (ClientRequestWrapper)wex.Data["ClientRequest"];
                        retry = true;

                        // Delay for the requested seconds
                        System.Threading.Thread.Sleep(backoffInterval);

                        // Increase counters
                        backoffInterval += backoffIncrease;

                    }
                    else
                    {
                        throw wex;
                    }
                }
            }
            throw new Exception(string.Format("Maximum retry attempts {0}, have been attempted.", retryCount));
        }
    }

    public class ProjectHelper
    {
        private static readonly MHA.Framework.Core.General.SQLHelper sqlHelper;

        static ProjectHelper()
        {
            sqlHelper = new MHA.Framework.Core.General.SQLHelper(ConnectionStringHelper.GetGenericWFConnString());
        }

        public static bool RequiredRetryException(string exceptionMessage)
        {
            bool result = false;
            exceptionMessage = exceptionMessage.ToLower();

            if (exceptionMessage.Contains(ConstantHelper.ExceptionType.SaveConflict) ||
               exceptionMessage.Contains(ConstantHelper.ExceptionType.VersionConflict) ||
               exceptionMessage.Contains(ConstantHelper.ExceptionType.OperationTimedOut) ||
               exceptionMessage.Contains(ConstantHelper.ExceptionType.HResult) ||
               exceptionMessage.Contains(ConstantHelper.ExceptionType.UnderlineClosed))
            {
                result = true;
            }

            return result;
        }

        #region URL-based data retrieval
        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        public static string GetRelativeUrlFromUrl(string url)
        {
            string relativeUrl = url;
            bool isAbsoluteUrl = IsAbsoluteUrl(url);
            if (isAbsoluteUrl)
            {
                Uri uri = new Uri(url);
                relativeUrl = uri.AbsolutePath;

                if (!string.IsNullOrEmpty(uri.Fragment))
                {
                    relativeUrl += uri.Fragment;
                }
            }
            relativeUrl = Uri.UnescapeDataString(relativeUrl);
            return relativeUrl.TrimEnd('/');
        }

        public static string GetSPHostURLDomain(string spHostURL, string spWebServerRelativeURL)
        {
            string spHostDomain = string.Empty;
            if (!string.IsNullOrEmpty(spHostURL) && !string.IsNullOrEmpty(spWebServerRelativeURL))
            {
                spHostDomain = spHostURL.Replace(spWebServerRelativeURL, string.Empty);
            }
            if (spHostDomain[spHostDomain.Length - 1] == '/')
            {
                spHostDomain = spHostDomain.Substring(0, spHostDomain.Length - 1);
            }
            return spHostDomain;
        }
        #endregion

        public static string ReplaceKeywordWithValue(string orignalKeyword, string sentence, int number)
        {
            string preKeyword = orignalKeyword.Substring(0, orignalKeyword.Length - 1) + ":";
            string postKeyword = "}";
            string keywordPattern = preKeyword + "\\d+" + postKeyword;
            Regex regex = new Regex(keywordPattern);
            if (regex.IsMatch(sentence))
            {
                string osub = regex.Match(sentence).Value;
                string sub = osub.Replace(preKeyword, string.Empty);
                sub = sub.Replace(postKeyword, string.Empty);
                int runningLength = Convert.ToInt32(sub);
                string runningFormat = string.Empty;

                for (int i = 0; i < runningLength; i++)
                {
                    runningFormat += "0";
                }
                sentence = sentence.Replace(osub, string.Format("{0:" + runningFormat + postKeyword, number));
            }
            return sentence;
        }

        #region Drop down list items
        public static List<DropDownListItem> GetListItemsAsDDLItems(ListItemCollection listItemCollection, string displayColumnName, string displayColumnName2, bool isIncludeBlank = false, bool showInactive = false)
        {
            List<DropDownListItem> listResult = new List<DropDownListItem>();

            if (listItemCollection != null && listItemCollection.Count > 0)
            {
                foreach (ListItem listItem in listItemCollection)
                {
                    string title = FieldHelper.GetFieldValueAsString(listItem, displayColumnName);
                    string status = FieldHelper.GetFieldValueAsString(listItem, displayColumnName2);
                    string id = FieldHelper.GetFieldValueAsString(listItem, "ID");

                    if (status == ConstantHelper.ItemStatus.Inactive && !showInactive)
                        continue;

                    DropDownListItem listOption = new DropDownListItem
                    {
                        Text = title,
                        Value = id,
                        Status = status,
                        Id = id
                    };

                    listResult.Add(listOption);
                }
            }

            listResult = listResult.OrderBy(x => x.Text).ToList();
            return listResult;
        }

        public static List<DropDownListItem> GetProcessTemplateDDL()
        {
            List<ProcessTemplate> processTemplateList = new List<ProcessTemplate>();
            List<DropDownListItem> processTemplateDDL = new List<DropDownListItem>();

            processTemplateList = GetWorkflowProcessTemplate();

            foreach (ProcessTemplate obj in processTemplateList)
            {
                if (obj.ProcessName == (ConstantHelper.WorkflowName.ELANWorkflow))
                    processTemplateDDL.Add(new DropDownListItem { Text = obj.ProcessName, Value = obj.ProcessID.ToString() });
            }
            return processTemplateDDL;
        }

        public static List<ProcessTemplate> GetWorkflowProcessTemplate()
        {
            List<ProcessTemplate> processTemplateList = new List<ProcessTemplate>();
            string ConnString = ConnectionStringHelper.GetGenericWFConnString();
            WorkflowBL wfBL = new WorkflowBL(ConnString);
            processTemplateList = wfBL.GetAllProcessTemplate();
            return processTemplateList;
        }

        public List<string> GetWorkflowStatus()
        {
            List<string> obj = new();

            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingOriginatorResubmission);
            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingReportingManagerApproval);
            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingInfraTeamAction);
            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingApplicationTeamAction);
            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingITManagerApproval);
            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingDepartmentAdminAction);
            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingAcknowledgement);
            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.Completed);
            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.Approved);
            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.Rejected);
            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.Terminated);
            //obj.Add(ConstantHelper.WorkflowStepName.ELANWorkflow.Draft);

            obj.Add(ConstantHelper.WorkflowStatus.PENDING_ORIGINATOR_RESUBMISSION);
            obj.Add(ConstantHelper.WorkflowStatus.PENDING_REPORTING_MANAGER_APPROVAL);
            obj.Add(ConstantHelper.WorkflowStatus.PENDING_INFRA_TEAM_ACTION);
            obj.Add(ConstantHelper.WorkflowStatus.PENDING_APPLICATION_TEAM_ACTION);
            obj.Add(ConstantHelper.WorkflowStatus.PENDING_IT_MANAGER_APPROVAL);
            obj.Add(ConstantHelper.WorkflowStatus.PENDING_DEPARTMENT_ADMIN_ACTION);
            obj.Add(ConstantHelper.WorkflowStatus.PENDING_ACKNOWLEDGEMENT);
            obj.Add(ConstantHelper.WorkflowStatus.COMPLETED);
            obj.Add(ConstantHelper.WorkflowStatus.APPROVED);
            obj.Add(ConstantHelper.WorkflowStatus.REJECTED);
            obj.Add(ConstantHelper.WorkflowStatus.TERMINATED);
            obj.Add(ConstantHelper.WorkflowStatus.DRAFT);

            return obj;
        }

        public List<string> GetRequestType()
        {
            List<string> obj = new();

            obj.Add(ConstantHelper.RequestForm.RequestType.NewRequestType);
            obj.Add(ConstantHelper.RequestForm.RequestType.ModificationRequestType);
            obj.Add(ConstantHelper.RequestForm.RequestType.TerminationRequestType);
            obj.Add(ConstantHelper.RequestForm.RequestType.TransferRequestType);
            obj.Add(ConstantHelper.RequestForm.RequestType.PromotionRequestType);

            return obj;
        }

        public List<string> GetContractOrPermanent()
        {
            List<string> obj = new();
            obj.Add(ConstantHelper.SQLTableColumnValue.FinalEmployeeDetails.ContractOrTemporaryStaff.Contract);
            obj.Add(ConstantHelper.SQLTableColumnValue.FinalEmployeeDetails.ContractOrTemporaryStaff.Permanent);

            return obj;
        }

        public List<string> GetEmployeeStatus()
        {
            List<string> obj = new();

            obj.Add(ConstantHelper.SQLTableColumnValue.FinalEmployeeDetails.EmployeeStatus.Active);
            obj.Add(ConstantHelper.SQLTableColumnValue.FinalEmployeeDetails.EmployeeStatus.Resigned);

            return obj;
        }

        public List<DropDownListItem> LoadDropDownListItem(string listName, bool showInactive, string spHostUrl, string accessToken)
        {
            using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
            {
                ListItemCollection listItem = GeneralQueryHelper.GetSPItems(clientContext, listName, string.Empty, null);

                List<DropDownListItem> dropDownListItem = new List<DropDownListItem>();

                dropDownListItem = ProjectHelper.GetListItemsAsDDLItems(listItem, ConstantHelper.SPColumn.Title, ConstantHelper.SPColumn.Status, false, showInactive);

                return dropDownListItem;
            }
        }
        #endregion

        #region Files
        public static byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[input.Length];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        #endregion

        #region Date
        public static DateTime? NormalizeDate(DateTime? date)
        {
            return (date.HasValue && date.Value != DateTime.MinValue) ? date : null;
        }

        #endregion

        #region SQL
        //TODO: Update in SQLHelper to accept to parameter for filtering in WHERE clause
        public static bool UpdateWithFilters(string tableName, List<string> fieldNames, List<object> fieldValues, List<string> filterNames, List<object> filterValues)
        {
            string setClause = string.Join(", ", fieldNames.Select(f => $"[{f}] = @{f}"));
            string whereClause = string.Join(" AND ", filterNames.Select(f => $"[{f}] = @{f}_filter"));

            string query = $@"
                UPDATE [{tableName}]
                SET {setClause}
                WHERE {whereClause}";

            var paramNames = new List<string>();
            var paramValues = new List<object>();

            // Fields
            for (int i = 0; i < fieldNames.Count; i++)
            {
                paramNames.Add("@" + fieldNames[i]);
                paramValues.Add(fieldValues[i]);
            }

            // Filters
            for (int i = 0; i < filterNames.Count; i++)
            {
                paramNames.Add("@" + filterNames[i] + "_filter");
                paramValues.Add(filterValues[i]);
            }

            sqlHelper.SQLExecute(true, query, paramNames, paramValues);

            return true;
        }

        public static bool Exists(string tableName, List<string> filterNames, List<object> filterValues)
        {
            string whereClause = string.Join(" AND ", filterNames.Select(f => $"[{f}] = @{f}"));
            string query = $"SELECT COUNT(1) FROM [{tableName}] WHERE {whereClause}";

            var paramNames = filterNames.Select(f => "@" + f).ToList();
            object result = sqlHelper.SQLExecuteScalar_RetObj(true, query, paramNames, filterValues);

            return Convert.ToInt32(result) > 0;
        }
        #endregion
    }
}

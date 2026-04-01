using MHA.Framework.Core.Workflow.BO;
using MHA.ELAN.Data;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using MHA.ELAN.Framework.JSONConstants;
using Microsoft.SharePoint.Client;
using System.Data;

namespace MHA.ELAN.Business
{
    public class HomeBL
    {
        private static readonly JSONAppSettings appSettings;

        static HomeBL()
        {
            appSettings = ConfigurationManager.GetAppSetting();
        }

        public async Task<ViewModelMyPendingTask> GetMyPendingTask(ViewModelMyPendingTask vmMyTask, string spHostURL, string accessToken)
        {
            using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
            {
                try
                {
                    //int pageSelectionLimit = int.Parse(appSettings.MyPagingSelectionShowLimit);
                    int pageSelectionLimit = ConstantHelper.MyPagingSelectionShowLimit;

                    #region Get My Pending Task
                    clientContext.Load(clientContext.Web, web => web.Title, user => user.CurrentUser, relativeURL => relativeURL.ServerRelativeUrl, url => url.Url);
                    clientContext.ExecuteQueryWithIncrementalRetry();
                    User spUser = clientContext.Web.CurrentUser;
                    Actioner currentUser = new Actioner(spUser.LoginName, spUser.Title, spUser.Email);
                    string applicationName = ProjectHelper.GetRelativeUrlFromUrl(spHostURL);

                    #region Get Current and Upcoming Task
                    //Get My Pending And Incoming Task
                    HomeDA homeDA = new HomeDA();
                    DataTable myPendingTaskDataTable = homeDA.GetMyPendingTaskData(vmMyTask, currentUser, applicationName);

                    //Get My Pending And Incoming Task Count
                    int totalRows = homeDA.GetMyPendingTaskDataCount(currentUser, applicationName);
                    vmMyTask.TotalCount = totalRows;
                    #endregion

                    DateTime currentDate = DateTimeHelper.GetCurrentDateTime();

                    foreach (DataRow row in myPendingTaskDataTable.Rows)
                    {
                        MyPendingTask myTaskObj = new MyPendingTask();
                        myTaskObj = ConvertEntitiesHelper.ConvertMyPendingTaskObj(row, clientContext);
                        if (myTaskObj != null && myTaskObj.DueDate.HasValue && myTaskObj.DueDate != DateTime.MinValue)
                        {
                            myTaskObj.IsOverDue = currentDate.Date > myTaskObj.DueDate.Value.Date;
                        }

                        MyPendingTask duplicatedPendingTask = vmMyTask.MyTaskList.FirstOrDefault(x => x.TaskID > 0 && String.Equals(x.TaskURL, myTaskObj.TaskURL));
                        if (duplicatedPendingTask == null)
                        {
                            vmMyTask.MyTaskList.Add(myTaskObj);
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    LogHelper logHelper = new LogHelper();
                    logHelper.LogMessage("HomeBL - GetMyPendingTask Error: " + ex.ToString());
                }
            }

            return vmMyTask;
        }

        public async Task<ViewModelMyActiveRequest> GetMyActiveRequest(ViewModelMyActiveRequest vm, string spHostURL, string accessToken)
        {
            using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
            {
                try
                {
                    clientContext.Load(clientContext.Web, web => web.Title, user => user.CurrentUser, relativeURL => relativeURL.ServerRelativeUrl, url => url.Url);
                    clientContext.ExecuteQueryWithIncrementalRetry();
                    User spUser = clientContext.Web.CurrentUser;
                    Actioner currentUser = new Actioner(spUser.LoginName, spUser.Title, spUser.Email);

                    List<MyActiveRequestData> reqObj = new();

                    List<string> workflowStatusList = new List<string>
                    {
                        ConstantHelper.WorkflowStatus.COMPLETED,
                        ConstantHelper.WorkflowStatus.APPROVED,
                        ConstantHelper.WorkflowStatus.REJECTED,
                        ConstantHelper.WorkflowStatus.TERMINATED,
                        ConstantHelper.WorkflowStatus.DRAFT,
                        ConstantHelper.RequestForm.WorkflowNewRequest.WorkflowStatusEmpty
                    };

                    string workflowStatuses = string.Join(",", workflowStatusList);

                    HomeDA homeDA = new HomeDA();
                    DataTable myActiveRequestDataTable = homeDA.GetActiveRequestData(vm, currentUser, workflowStatuses);
                    int totalRows = homeDA.GetActiveRequestDataCount(currentUser, workflowStatuses);
                    vm.Count = totalRows;

                    foreach (DataRow row in myActiveRequestDataTable.Rows)
                    {
                        MyActiveRequestData obj = new();
                        obj = ConvertEntitiesHelper.ConvertMyActiveRequestObj(row);

                        reqObj.Add(obj);
                    }

                    vm.Request = reqObj;
                }
                catch (Exception ex)
                {
                    LogHelper logHelper = new LogHelper();
                    logHelper.LogMessage("HomeBL - GetMyActiveRequest Error: " + ex.ToString());
                }
            }

            return vm;
        }
    }
}

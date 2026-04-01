using MHA.Framework.Core.SP;
using MHA.Framework.Core.Workflow.BL;
using MHA.Framework.Core.Workflow.BO;
using MHA.ELAN.Data;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using MHA.ELAN.Framework.JSONConstants;
using Microsoft.SharePoint.Client;
using System.Data;
using System.Net.Mail;

namespace MHA.ELAN.Business
{
    public class WorkflowBL
    {
        private static readonly JSONAppSettings appSettings;

        static WorkflowBL()
        {
            appSettings = ConfigurationManager.GetAppSetting();
        }

        #region Workflow History
        public async Task<PartialModelWorkflowHistory> InitWorkflowHistory(int processID, string spHostURL, string accessToken)
        {
            PartialModelWorkflowHistory modelWFHist = new PartialModelWorkflowHistory();
            try
            {
                if (processID != -1)
                {
                    modelWFHist.WorkflowHistoryList = GetWorkflowHistory(processID);
                    modelWFHist.ProcessID = processID + "";

                    string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                    ProcessInstance processInstance = wfBL.GetProcessInstance(processID);

                    if (processInstance.CompletionDate == DateTime.MinValue)
                    {
                        string attributeName = string.Format("<Key>{0}</Key><Value>", ConstantHelper.WorkflowKeywords.Common.WorkflowDueDate);
                        int startIndex = processInstance.KeywordsXML.IndexOf(attributeName);

                        if (startIndex > -1)
                        {
                            string workflowDueDate = processInstance.KeywordsXML.Substring(startIndex + attributeName.Length);
                            modelWFHist.WorkflowDueDate = workflowDueDate.Substring(0, workflowDueDate.IndexOf("</Value>"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage("WorkflowBL - InitWorkflowHistory Error: " + ex.ToString());
            }
            return modelWFHist;
        }

        public async Task<PartialModelAdminWorkflowHistory> InitAdminWFHistory(int processID, string spHostURL, string accessToken)
        {
            PartialModelAdminWorkflowHistory obj = new PartialModelAdminWorkflowHistory();
            try
            {
                if (processID > 0)
                {
                    obj.WorkflowHistoryList = GetWorkflowHistory(processID);
                    obj.ProcessID = processID + "";
                    obj.isWFRunnning = WorkflowHelper.IsWorkflowRunning(processID);
                    //InstanceDL
                    string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                    ProcessInstance processInstance = wfBL.GetProcessInstance(processID);

                    if (processInstance.CompletionDate == DateTime.MinValue)
                    {
                        string attributeName = string.Format("<Key>{0}</Key><Value>", ConstantHelper.WorkflowKeywords.Common.WorkflowDueDate);
                        int startIndex = processInstance.KeywordsXML.IndexOf(attributeName);

                        if (startIndex > -1)
                        {
                            string workflowDueDate = processInstance.KeywordsXML.Substring(startIndex + attributeName.Length);
                            obj.WorkflowDueDate = workflowDueDate.Substring(0, workflowDueDate.IndexOf("</Value>"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage("WorkflowBL - InitAdminWFHistory Error: " + ex.ToString());
            }
            return obj;
        }

        public List<WorkflowHistory> GetWorkflowHistory(int processID)
        {
            LogHelper logHelper = new LogHelper();
            List<WorkflowHistory> wfHistoryList = new List<WorkflowHistory>();

            try
            {
                WorkflowDA workflowDA = new WorkflowDA();
                DataTable wfDT = workflowDA.GetWorkflowHistory(processID);
                List<WorkflowHistory> workflowList = ConvertEntitiesHelper.ConvertWorkflowHistoryObj(wfDT); ;

                foreach (WorkflowHistory item in workflowList)
                {
                    item.ShowRemoveLink = ShowRemoveAction(item.Status);
                    item.ShowRemoveCheckbox = ShowRemoveAction(item.Status);

                    wfHistoryList.Add(item);
                }
            }
            catch (Exception ex)
            {
                logHelper.LogMessage("WorkflowBL - GetWorkflowHistory Error: " + ex.ToString());
            }
            return wfHistoryList;
        }
        #endregion

        #region Delegation
        public async Task<ViewModelMyDelegate> InitMyDelegate(string spHostURL, string accessToken)
        {
            ViewModelMyDelegate vmMyDelegate = new ViewModelMyDelegate();

            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    User curUser = SharePointHelper.GetCurrentUser(clientContext);

                    vmMyDelegate.IsAdmin = SharePointHelper.IsUserInGroup(clientContext, curUser.LoginName, ConstantHelper.SPSecurityGroup.ElanAdmin);

                    PeoplePickerUser curUserObj = new PeoplePickerUser();
                    curUserObj.Email = curUser.Email;
                    curUserObj.Login = curUser.LoginName;
                    curUserObj.Name = curUser.Title;

                    if (vmMyDelegate.IsAdmin)
                        vmMyDelegate.DelegationList = GetAllDelegationList(spHostURL, clientContext);
                    else
                        vmMyDelegate.DelegationList = GetMyDelegationList(curUserObj, spHostURL, clientContext);
                }

                if (vmMyDelegate.DelegationList?.Count > 0)
                    vmMyDelegate.HasItem = true;

                vmMyDelegate.DateFormat = appSettings.DefaultDateFormat;
                vmMyDelegate.NewDelegationURL = string.Format(ConstantHelper.URLTemplate.NewDelegationUrlTemplate, appSettings.RemoteAppURL, spHostURL);
                vmMyDelegate.RemoveDelegationURL = string.Format(ConstantHelper.URLTemplate.NewDelegationUrlTemplate, appSettings.RemoteAppURL, spHostURL);

            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage("WorkflowBL - InitMyDelegate Error: " + ex.ToString());
            }

            return vmMyDelegate;
        }

        public List<DelegationObject> GetAllDelegationList(string spHostURL, ClientContext clientContext)
        {
            List<DelegationInstance> instanceList = new List<DelegationInstance>();
            List<DelegationObject> resultList = new List<DelegationObject>();

            try
            {
                string ConnString = ConnectionStringHelper.GetGenericWFConnString();
                DelegationBL delegateBl = new DelegationBL(ConnString);

                instanceList = delegateBl.GetAllDelegations(ProjectHelper.GetRelativeUrlFromUrl(spHostURL));

                DateTime currentDateTime = DateTimeHelper.GetCurrentDateTime();

                if (instanceList != null && instanceList.Count > 0)
                {
                    foreach (DelegationInstance obj in instanceList)
                    {
                        obj.DelegationStartDate = SPDateTimeHelper.ConvertFromUTCWithSPTimeZone(obj.DelegationStartDate, clientContext);
                        obj.DelegationEndDate = SPDateTimeHelper.ConvertFromUTCWithSPTimeZone(obj.DelegationEndDate, clientContext);

                        DelegationObject delegateObj = new DelegationObject();
                        delegateObj.Active = obj.Active;
                        delegateObj.ApplicationName = obj.ApplicationName;
                        delegateObj.DelegationEndDate = obj.DelegationEndDate;
                        delegateObj.DelegationFrom = obj.DelegationFrom;
                        delegateObj.DelegationFromEmail = obj.DelegationFromEmail;
                        delegateObj.DelegationFromFriendlyName = obj.DelegationFromFriendlyName;
                        delegateObj.DelegationID = obj.DelegationID;
                        delegateObj.DelegationStartDate = obj.DelegationStartDate;
                        delegateObj.DelegationTo = obj.DelegationTo;
                        delegateObj.DelegationToEmail = obj.DelegationToEmail;
                        delegateObj.DelegationToFriendlyName = obj.DelegationToFriendlyName;
                        delegateObj.ProcessName = obj.ProcessName;
                        delegateObj.ProcessTemplateID = obj.ProcessTemplateID;
                        delegateObj.IsAbleToRemove = true;

                        if (currentDateTime.Date <= obj.DelegationEndDate.Date)
                        {
                            resultList.Add(delegateObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage("WorkflowBL - GetAllDelegationList Error: " + ex.ToString());
            }
            return resultList;
        }

        public List<DelegationObject> GetMyDelegationList(PeoplePickerUser curUser, string spHostURL, ClientContext clientContext)
        {
            List<DelegationInstance> instanceList = new List<DelegationInstance>();
            List<DelegationObject> resultList = new List<DelegationObject>();

            try
            {
                string ConnString = ConnectionStringHelper.GetGenericWFConnString();
                DelegationBL delegateBl = new DelegationBL(ConnString);
                Actioner actioner = new Actioner(curUser.Login, curUser.Name, curUser.Email);
                instanceList = delegateBl.GetMyDelegations(actioner, ProjectHelper.GetRelativeUrlFromUrl(spHostURL));
                DateTime currentDate = DateTimeHelper.GetCurrentDateTime();

                if (instanceList != null && instanceList.Count > 0)
                {
                    foreach (DelegationInstance obj in instanceList)
                    {
                        obj.DelegationStartDate = SPDateTimeHelper.ConvertFromUTCWithSPTimeZone(obj.DelegationStartDate, clientContext);
                        obj.DelegationEndDate = SPDateTimeHelper.ConvertFromUTCWithSPTimeZone(obj.DelegationEndDate, clientContext);

                        DelegationObject delegateObj = new DelegationObject();
                        delegateObj.Active = obj.Active;
                        delegateObj.ApplicationName = obj.ApplicationName;
                        delegateObj.DelegationEndDate = obj.DelegationEndDate;
                        delegateObj.DelegationFrom = obj.DelegationFrom;
                        delegateObj.DelegationFromEmail = obj.DelegationFromEmail;
                        delegateObj.DelegationFromFriendlyName = obj.DelegationFromFriendlyName;
                        delegateObj.DelegationID = obj.DelegationID;
                        delegateObj.DelegationStartDate = obj.DelegationStartDate;
                        delegateObj.DelegationTo = obj.DelegationTo;
                        delegateObj.DelegationToEmail = obj.DelegationToEmail;
                        delegateObj.DelegationToFriendlyName = obj.DelegationToFriendlyName;
                        delegateObj.ProcessName = obj.ProcessName;
                        delegateObj.ProcessTemplateID = obj.ProcessTemplateID;
                        if (delegateObj.DelegationFromFriendlyName.Equals(curUser.Name))
                            delegateObj.IsAbleToRemove = true;
                        else
                            delegateObj.IsAbleToRemove = false;

                        if (currentDate.Date <= obj.DelegationEndDate.Date)
                        {
                            resultList.Add(delegateObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage("WorkflowBL - GetMyDelegationList Error: " + ex.ToString());
            }

            return resultList;
        }

        #region New Delegation
        public async Task<ViewModelNewDelegate> InitNewDelegate(string spHostURL, string accessToken)
        {
            ViewModelNewDelegate vmNewDelegate = new ViewModelNewDelegate();

            try
            {
                vmNewDelegate.ProcessTemplateList = ProjectHelper.GetProcessTemplateDDL();
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    User curUser = SharePointHelper.GetCurrentUser(clientContext);
                    vmNewDelegate.isAdmin = SharePointHelper.IsUserInGroup(clientContext, curUser.LoginName, ConstantHelper.SPSecurityGroup.ElanAdmin);
                    vmNewDelegate.curUserName = curUser.Title;
                }
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage("WorkflowBL - InitNewDelegate Error: " + ex.ToString());

                vmNewDelegate.HasError = true;
                vmNewDelegate.ErrorMessage = string.Format(ConstantHelper.ErrorMessage.UnexpectedErrorOccur, ex.Message);
            }
            return vmNewDelegate;
        }

        public async Task<string> SetDelegation(ViewModelNewDelegate vmNewDelegate, string spHostURL, string accessToken)
        {
            LogHelper logHelper = new LogHelper();
            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    User curUser = SharePointHelper.GetCurrentUser(clientContext);

                    PeoplePickerUser fromUser = new PeoplePickerUser();
                    if (vmNewDelegate.isAdmin)
                    {
                        fromUser = vmNewDelegate.txtFromUser ?? throw new ArgumentNullException("Delegation From field cannot be null value.");
                    }
                    else
                    {
                        //delegate from current user
                        fromUser = new PeoplePickerUser { Email = curUser.Email, Login = curUser.LoginName, Name = curUser.Title };
                    }

                    PeoplePickerUser toUser = vmNewDelegate.txtTouser;
                    if (toUser == null)
                    {
                        throw new ArgumentNullException("Delegation To field cannot be null value.");
                    }

                    string applicationName = ProjectHelper.GetRelativeUrlFromUrl(spHostURL);

                    DelegationInstance delegateObj = new DelegationInstance();
                    delegateObj.DelegationEndDate = DateTimeHelper.ConvertToUTCDateTime(vmNewDelegate.dateTo.AddDays(1).Date.AddSeconds(-1));
                    delegateObj.DelegationFrom = fromUser.Login;
                    delegateObj.DelegationFromEmail = fromUser.Email;
                    delegateObj.DelegationFromFriendlyName = fromUser.Name;
                    delegateObj.DelegationStartDate = DateTimeHelper.ConvertToUTCDateTime(vmNewDelegate.dateFrom);
                    delegateObj.DelegationTo = toUser.Login;
                    delegateObj.DelegationToEmail = toUser.Email;
                    delegateObj.DelegationToFriendlyName = toUser.Name;
                    delegateObj.ApplicationName = applicationName;
                    int processID = -1;

                    string connString = ConnectionStringHelper.GetGenericWFConnString();
                    DelegationBL delegateBl = new DelegationBL(connString);
                    Actioner actioner = new Actioner(fromUser.Login, fromUser.Name, fromUser.Email);
                    List<DelegationInstance> delegations = delegateBl.GetMyDelegations(actioner, applicationName);
                    bool hasError = true;
                    foreach (var d in delegations)
                    {
                        if (delegateObj.DelegationStartDate < d.DelegationStartDate && d.DelegationStartDate < delegateObj.DelegationEndDate)
                        {
                            hasError = true;
                        }
                        else if (delegateObj.DelegationStartDate < d.DelegationEndDate && d.DelegationEndDate <= delegateObj.DelegationEndDate)
                        {
                            hasError = true;
                        }
                        else if (d.DelegationStartDate < delegateObj.DelegationStartDate && d.DelegationEndDate > delegateObj.DelegationEndDate)
                        {
                            hasError = true;
                        }
                        else
                        {
                            hasError = false;
                        }
                        if (hasError)
                        {
                            DateTime overlappedStartDate = DateTimeHelper.ConvertToLocalDateTime(d.DelegationStartDate);
                            DateTime overlappedEndDate = DateTimeHelper.ConvertToLocalDateTime(d.DelegationEndDate);

                            int tempProcessID = 0;
                            // Selected all processes
                            if (!int.TryParse(vmNewDelegate.ddlProcessTemplate, out tempProcessID))
                            {
                                return string.Format("Unable to submit delegation for all processes because the selected dates overlap with an existing delegation ({0}: {1} to {2})", d.ProcessName, overlappedStartDate.ToString(appSettings.DefaultDateFormat), overlappedEndDate.ToString(appSettings.DefaultDateFormat));
                            }
                            else
                            {
                                if (d.ProcessTemplateID == tempProcessID)
                                {
                                    return string.Format("Unable to submit delegation because the selected dates overlap with an existing delegation ({0}: {1} to {2})", d.ProcessName, overlappedStartDate.ToString(appSettings.DefaultDateFormat), overlappedEndDate.ToString(appSettings.DefaultDateFormat));
                                }
                            }
                        }
                    }

                    if (!int.TryParse(vmNewDelegate.ddlProcessTemplate, out processID))
                    {
                        List<ProcessTemplate> processInstanceList = new List<ProcessTemplate>();
                        processInstanceList = ProjectHelper.GetWorkflowProcessTemplate();
                        foreach (ProcessTemplate obj in processInstanceList)
                        {
                            delegateObj.ProcessTemplateID = obj.ProcessID;
                            InsertDelegation(delegateObj, curUser.LoginName);
                        }
                    }
                    else
                    {

                        delegateObj.ProcessTemplateID = processID;
                        InsertDelegation(delegateObj, curUser.LoginName);
                    }

                    //Send delegation notification
                    SendGeneralWorkflowEmailNotification(ConstantHelper.EmailTemplateKeyTitle.WorkflowTaskDelegatedNotification, toUser, null, null, fromUser, vmNewDelegate.dateFrom, vmNewDelegate.dateTo, spHostURL, accessToken, string.Empty, string.Empty);
                }
            }
            catch (Exception ex)
            {
                logHelper.LogMessage("WorkflowBL - SetDelegation Error: " + ex.ToString());
                return ConstantHelper.ErrorMessage.UnexpectedErrorOccur;
            }

            return string.Empty;
        }

        public int InsertDelegation(DelegationInstance delegationObj, string curUserLogin)
        {
            string ConnString = ConnectionStringHelper.GetGenericWFConnString();
            DelegationBL delegateBl = new DelegationBL(ConnString);
            return delegateBl.InsertDelegationInstance(delegationObj, curUserLogin);
        }
        #endregion

        #region Remove Delegation
        public int DeleteDelegation(int delegationID, string curUserLogin)
        {
            string ConnString = ConnectionStringHelper.GetGenericWFConnString();
            DelegationBL delegateBl = new DelegationBL(ConnString);
            return delegateBl.DeleteDelegationInstance(delegationID, curUserLogin);
        }
        #endregion

        #endregion

        #region Workflow
        public static string StartWorkflow(StartWorkflowObject startWFObject, ViewModelRequest vm, string spHostURL, ClientContext clientContext, string appAccessToken, ref int processId)
        {
            string errorMessage = string.Empty;
            string rerAccountEmail = appSettings.rerAccountEmail;
            string rerAccountPassword = appSettings.rerAccountPassword;

            string WFConnString = ConnectionStringHelper.GetGenericWFConnString();

            try
            {
                //Step 1 - Set stage actioners
                //Step 2 - Set keywords
                //Step 3 - Start workflow

                #region Workflow Matrix
                PeoplePickerUser ReportingManagerUser = new PeoplePickerUser
                {
                    Login = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerLogin,
                    Name = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerName,
                    Email = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerEmail
                };

                startWFObject.ReportingManager = new List<PeoplePickerUser> { ReportingManagerUser };
                startWFObject.InfraTeam = SharePointHelper.GetUsersFromSharePointGroup(clientContext, ConstantHelper.WorkflowMatrix.InfraTeam);
                startWFObject.ApplicationTeam = SharePointHelper.GetUsersFromSharePointGroup(clientContext, ConstantHelper.WorkflowMatrix.ApplicationTeam);
                startWFObject.ITManager = SharePointHelper.GetUsersFromSharePointGroup(clientContext, ConstantHelper.WorkflowMatrix.ITManager);
                startWFObject.DepartmentAdmin = SharePointHelper.GetUsersFromSharePointGroup(clientContext, ConstantHelper.WorkflowMatrix.DepartmentAdmin);

                startWFObject.RequestType = vm.RequestType;
                switch (startWFObject.RequestType)
                {
                    case ConstantHelper.RequestForm.RequestType.NewRequestType:
                        Require(ConstantHelper.WorkflowMatrix.ReportingManager, startWFObject.ReportingManager);
                        Require(ConstantHelper.WorkflowMatrix.InfraTeam, startWFObject.InfraTeam);
                        Require(ConstantHelper.WorkflowMatrix.ApplicationTeam, startWFObject.ApplicationTeam);
                        Require(ConstantHelper.WorkflowMatrix.ITManager, startWFObject.ITManager);
                        Require(ConstantHelper.WorkflowMatrix.DepartmentAdmin, startWFObject.DepartmentAdmin);
                        break;

                    case ConstantHelper.RequestForm.RequestType.ModificationRequestType:
                    case ConstantHelper.RequestForm.RequestType.TransferRequestType:
                    case ConstantHelper.RequestForm.RequestType.PromotionRequestType:
                        Require(ConstantHelper.WorkflowMatrix.ReportingManager, startWFObject.ReportingManager);
                        if (vm.ModifyITRequirements)
                        {
                            Require(ConstantHelper.WorkflowMatrix.ReportingManager, startWFObject.ReportingManager);
                            Require(ConstantHelper.WorkflowMatrix.InfraTeam, startWFObject.InfraTeam);
                            Require(ConstantHelper.WorkflowMatrix.ApplicationTeam, startWFObject.ApplicationTeam);
                            Require(ConstantHelper.WorkflowMatrix.ITManager, startWFObject.ITManager);
                        }
                        if (vm.ModifyHardware)
                        {
                            Require(ConstantHelper.WorkflowMatrix.ReportingManager, startWFObject.ReportingManager);
                            Require(ConstantHelper.WorkflowMatrix.DepartmentAdmin, startWFObject.DepartmentAdmin);
                        }
                        break;

                    case ConstantHelper.RequestForm.RequestType.TerminationRequestType:
                        Require(ConstantHelper.WorkflowMatrix.ReportingManager, startWFObject.ReportingManager);
                        Require(ConstantHelper.WorkflowMatrix.InfraTeam, startWFObject.InfraTeam);
                        Require(ConstantHelper.WorkflowMatrix.ApplicationTeam, startWFObject.ApplicationTeam);
                        Require(ConstantHelper.WorkflowMatrix.ITManager, startWFObject.ITManager);
                        Require(ConstantHelper.WorkflowMatrix.DepartmentAdmin, startWFObject.DepartmentAdmin);
                        break;

                    default:
                        throw new Exception("Invalid request type specified.");
                }
                #endregion

                #region Workflow Due Days
                WorkflowDA wfDA = new WorkflowDA();

                startWFObject.PendingReportingManagerDueDays = wfDA.GetWFStepDueDateDaysFromDb(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingReportingManagerApproval, WFConnString);
                startWFObject.PendingInfraTeamDueDays = wfDA.GetWFStepDueDateDaysFromDb(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingInfraTeamAction, WFConnString);
                startWFObject.PendingApplicationTeamDueDays = wfDA.GetWFStepDueDateDaysFromDb(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingApplicationTeamAction, WFConnString);
                startWFObject.PendingITManagerDueDays = wfDA.GetWFStepDueDateDaysFromDb(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingITManagerApproval, WFConnString);
                startWFObject.PendingDepartmentAdminDueDays = wfDA.GetWFStepDueDateDaysFromDb(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingDepartmentAdminAction, WFConnString);
                startWFObject.PendingRecipientDueDays = wfDA.GetWFStepDueDateDaysFromDb(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingAcknowledgement, WFConnString);
                startWFObject.PendingOriginatorResubmissionDueDays = wfDA.GetWFStepDueDateDaysFromDb(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingOriginatorResubmission, WFConnString);

                #endregion

                //Working Days
                List<DayOfWeek> daysOfWeek = new List<DayOfWeek>
                {
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday
                };

                //Calculate Stage Due Date
                DateTime workflowDueDate = DateTimeHelper.GetCurrentDateTime();
                startWFObject = ModifyELANTaskDueDate(startWFObject, ref workflowDueDate, daysOfWeek);

                List<int> reminderDays = new List<int>()
                {
                    startWFObject.PendingReportingManagerDueDays,
                    startWFObject.PendingInfraTeamDueDays,
                    startWFObject.PendingApplicationTeamDueDays,
                    startWFObject.PendingITManagerDueDays,
                    startWFObject.PendingDepartmentAdminDueDays,
                    startWFObject.PendingRecipientDueDays,
                    startWFObject.PendingOriginatorResubmissionDueDays,
                };

                startWFObject.UseDefaultDueDays = true;

                #region Contruct Email
                //MailAddressCollection actionerMails = GetWFActionersMails(startWFObject, startWFObject?.Originator?.Email, clientContext);

                MailAddressCollection notificationToMails = new MailAddressCollection();
                EmailHelper.AddEmail(startWFObject.Originator?.Email, ref notificationToMails);

                #endregion

                //Check if there is workflow running
                bool isWorkflowRunning = WorkflowHelper.IsWorkflowRunning(vm.ReferenceNo);
                if (isWorkflowRunning)
                {
                    throw new Exception("This ELAN request have active workflow running and cannot be launched again. Please try again later.");
                }

                //TODO
                //Check if there is any active workflow running for this employee
                if (vm.RequestType == ConstantHelper.RequestForm.RequestType.ModificationRequestType || vm.RequestType == ConstantHelper.RequestForm.RequestType.TransferRequestType || vm.RequestType == ConstantHelper.RequestForm.RequestType.PromotionRequestType || vm.RequestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType)
                {
                    RequestDA reqDA = new RequestDA();
                    DataTable dt = reqDA.RetrieveRequestByFinalEmpID(vm.EmployeeDetails.EmployeeDetailsVM.ID.ToString());
                    List<ViewRequestItem> reqList = ConvertEntitiesHelper.ConvertRequestItemList(dt);

                    bool isEmpWorkflowRunning = WorkflowHelper.IsWorkflowRunning(reqList);
                    if (isWorkflowRunning)
                    {
                        throw new Exception("This employee already has an active workflow in progress. Please wait until the current workflow is completed before starting a new one.");
                    }
                }

                //Step 1 - Set stage actioners based on request type
                StageActioners stageActioners = SetWFActioners(startWFObject, vm);

                //Final Stage
                stageActioners.NextStage(ConstantHelper.WorkflowStepName.ELANWorkflow.Completed, new MailAddressCollection(), notificationToMails);
                stageActioners.NextStage(ConstantHelper.WorkflowStepName.ELANWorkflow.Rejected, new MailAddressCollection(), notificationToMails);

                //Step 2 - Set keywords
                ProcessKeywords processKeywords = SetWFProcessKeywords(vm, startWFObject, spHostURL, ConstantHelper.WorkflowName.ELANWorkflow, workflowDueDate, reminderDays, rerAccountEmail, rerAccountPassword, appAccessToken);

                //Step 3 - Start Workflow
                Actioner Originator = WorkflowHelper.ConstructActioner(startWFObject.Originator);
                string applicationName = ProjectHelper.GetRelativeUrlFromUrl(spHostURL);
                MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                processId = wfBL.StartWorkflow(ConstantHelper.WorkflowName.ELANWorkflow, applicationName, Originator, processKeywords, stageActioners, "");
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage(String.Format("Workflow BL - StartWorkflow Error: {0} ", ex.ToString()));
                errorMessage = string.Format("Error: {0}", ex.Message);
            }

            return errorMessage;
        }

        private static StageActioners SetWFActioners(StartWorkflowObject startWFObject, ViewModelRequest vm)
        {
            StageActioners stageActioners = new StageActioners(ConstantHelper.WorkflowStepName.ELANWorkflow.PendingReportingManagerApproval, startWFObject.PendingReportingManagerDueDays);

            if (startWFObject.ReportingManager.Count > 0)
            {
                WorkflowHelper.AddStageActionerFromPeoplePickerList(ref stageActioners, startWFObject.ReportingManager);
            }

            switch (startWFObject.RequestType)
            {
                case ConstantHelper.RequestForm.RequestType.NewRequestType:
                    WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingInfraTeamAction, startWFObject.PendingInfraTeamDueDays, startWFObject.InfraTeam);

                    WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingApplicationTeamAction, startWFObject.PendingApplicationTeamDueDays, startWFObject.ApplicationTeam);

                    WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingITManagerApproval, startWFObject.PendingITManagerDueDays, startWFObject.ITManager);

                    WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingDepartmentAdminAction, startWFObject.PendingDepartmentAdminDueDays, startWFObject.DepartmentAdmin);

                    break;

                case ConstantHelper.RequestForm.RequestType.ModificationRequestType:
                case ConstantHelper.RequestForm.RequestType.TransferRequestType:
                case ConstantHelper.RequestForm.RequestType.PromotionRequestType:
                    if (vm.ModifyITRequirements)
                    {
                        WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingInfraTeamAction, startWFObject.PendingInfraTeamDueDays, startWFObject.InfraTeam);

                        WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingApplicationTeamAction, startWFObject.PendingApplicationTeamDueDays, startWFObject.ApplicationTeam);

                        WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingITManagerApproval, startWFObject.PendingITManagerDueDays, startWFObject.ITManager);
                    }
                    if (vm.ModifyHardware)
                    {
                        WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingDepartmentAdminAction, startWFObject.PendingDepartmentAdminDueDays, startWFObject.DepartmentAdmin);
                    }
                    break;

                case ConstantHelper.RequestForm.RequestType.TerminationRequestType:
                    WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingInfraTeamAction, startWFObject.PendingInfraTeamDueDays, startWFObject.InfraTeam);

                    WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingApplicationTeamAction, startWFObject.PendingApplicationTeamDueDays, startWFObject.ApplicationTeam);

                    WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingITManagerApproval, startWFObject.PendingITManagerDueDays, startWFObject.ITManager);

                    WorkflowHelper.AddStage(ref stageActioners, ConstantHelper.WorkflowStepName.ELANWorkflow.PendingDepartmentAdminAction, startWFObject.PendingDepartmentAdminDueDays, startWFObject.DepartmentAdmin);
                    break;

                default:
                    throw new Exception("Invalid request type specified.");
            }

            return stageActioners;
        }

        public static void Require(string roleName, List<PeoplePickerUser> list)
        {
            if (list == null || list.Count == 0)
                throw new Exception(
                    string.Format(ConstantHelper.ErrorMessage.MissingWorkflowMatrix, roleName)
                );
        }

        public static StartWorkflowObject ModifyELANTaskDueDate(StartWorkflowObject startWorkflowObject, ref DateTime workflowDueDate, List<DayOfWeek> daysOfWeek)
        {
            DateTime currentDate = DateTimeHelper.GetCurrentDateTime();

            //1.Pending Reporting Manager Approval - Reporting Manager
            if (startWorkflowObject.ReportingManager.Count > 0)
            {
                currentDate = DateTimeHelper.GetNextWorkingDate(daysOfWeek, currentDate, startWorkflowObject.PendingReportingManagerDueDays);
                startWorkflowObject.PendingReportingManagerDueDate = currentDate.ToString(appSettings.DefaultDateFormat);
            }

            //2.Pending Infra Team Action – Infra Team
            if (startWorkflowObject.InfraTeam.Count > 0)
            {
                currentDate = DateTimeHelper.GetNextWorkingDate(daysOfWeek, currentDate, startWorkflowObject.PendingInfraTeamDueDays);
                startWorkflowObject.PendingInfraTeamDueDate = currentDate.ToString(appSettings.DefaultDateFormat);
            }

            //3.Pending Application Team Action - Application Team
            if (startWorkflowObject.ApplicationTeam.Count > 0)
            {
                currentDate = DateTimeHelper.GetNextWorkingDate(daysOfWeek, currentDate, startWorkflowObject.PendingApplicationTeamDueDays);
                startWorkflowObject.PendingApplicationTeamDueDate = currentDate.ToString(appSettings.DefaultDateFormat);
            }

            //4.Pending IT Manager Approval - IT Manager
            if (startWorkflowObject.ITManager.Count > 0)
            {
                currentDate = DateTimeHelper.GetNextWorkingDate(daysOfWeek, currentDate, startWorkflowObject.PendingITManagerDueDays);
                startWorkflowObject.PendingITManagerDueDate = currentDate.ToString(appSettings.DefaultDateFormat);
            }

            //5.Pending Department Admin Action - Department Admin
            if (startWorkflowObject.DepartmentAdmin.Count > 0)
            {
                currentDate = DateTimeHelper.GetNextWorkingDate(daysOfWeek, currentDate, startWorkflowObject.PendingDepartmentAdminDueDays);
                startWorkflowObject.PendingDepartmentAdminDueDate = currentDate.ToString(appSettings.DefaultDateFormat);
            }

            //6.Pending Acknowledgement - Recipient
            if (startWorkflowObject.Recipient != null)
            {
                currentDate = DateTimeHelper.GetNextWorkingDate(daysOfWeek, currentDate, startWorkflowObject.PendingRecipientDueDays);
                startWorkflowObject.PendingRecipientDueDate = currentDate.ToString(appSettings.DefaultDateFormat);
            }

            //7.Pending Originator Resubmission - Originator
            if (startWorkflowObject.Originator != null)
            {
                currentDate = DateTimeHelper.GetNextWorkingDate(daysOfWeek, currentDate, startWorkflowObject.PendingOriginatorResubmissionDueDays);
                startWorkflowObject.PendingOriginatorResubmissionDueDate = currentDate.ToString(appSettings.DefaultDateFormat);
            }

            workflowDueDate = currentDate;
            return startWorkflowObject;
        }

        private static ProcessKeywords SetWFProcessKeywords(ViewModelRequest vm, StartWorkflowObject startWFObject, string spHostURL, string workflowName, DateTime workflowDueDate, List<int> reminderDays, string rerAccountEmail, string rerAccountPassword, string appAccessToken)
        {
            string smtp = appSettings.AG_EMAILHOST;
            string smtp_Port = appSettings.AG_EMAILPORT;
            string emailFrom = appSettings.AG_EMAILFROM;
            string emailPassword = string.Empty;
            string encKey = appSettings.AG_ENCKEY;
            string emailDefailtCred = appSettings.AG_EMAILDEFAULTCREDENTIAL;
            string emailUseSSL = appSettings.AG_EMAILUSESSL;
            string emailUseDefailtCred = appSettings.AG_EMAILUSEDEFAULTCREDENTIAL;

            if (!string.IsNullOrEmpty(rerAccountEmail))
                emailFrom = rerAccountEmail;

            if (!string.IsNullOrEmpty(rerAccountPassword))
                emailPassword = rerAccountPassword;

            if (string.IsNullOrEmpty(smtp) || string.IsNullOrEmpty(smtp_Port) || string.IsNullOrEmpty(emailFrom))
                throw new ArgumentNullException("SMTP host name, SMTP port number and Email address sender in webconfig value cannot be null");
            int emailPort = int.Parse(smtp_Port);

            ProcessKeywords processKeywords = new ProcessKeywords(vm.ReferenceNo, spHostURL, smtp, emailPort, emailFrom, encKey, emailDefailtCred, emailUseSSL, emailUseDefailtCred, emailFrom, emailPassword);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.SPHostURL, spHostURL);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.AppAccessToken, appAccessToken);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.SPWebURL, appSettings.RemoteAppURL);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.WorkflowName, workflowName);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.WorkflowDueDate, workflowDueDate.ToString(appSettings.DefaultDateFormat));

            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.RequestType, vm.RequestType);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.RequestID, vm.ID.ToString());
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.RequestRefNo, vm.ReferenceNo);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.EmployeeId, vm.EmployeeDetails.EmployeeDetailsVM.EmployeeID);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.EmployeeName, vm.EmployeeDetails.EmployeeDetailsVM.EmployeeName);

            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.Originator, startWFObject.Originator?.Name);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.OriginatorSubmittedByName, vm.SubmittedBy);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.OriginatorSubmittedByLogin, vm.SubmittedByLogin);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.OriginatorSubmittedByEmail, vm.SubmittedByEmail);

            //TimeZone for Working Day Calculation
            string timeZoneName = appSettings.CentralTimeZone;
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.AG_TimeZoneName, timeZoneName);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.WorkflowCycleDueDays, string.Join(",", reminderDays));

            string viewEmployeeLink = string.Format(ConstantHelper.URLTemplate.EmployeeDisplayFormUrlTemplate, appSettings.RemoteAppURL.TrimEnd('/'), vm.EmployeeDetails.EmployeeDetailsVM.EmployeeID, spHostURL);
            string viewRequestLink = string.Format(ConstantHelper.URLTemplate.RequestDisplayFormUrlTemplate, appSettings.RemoteAppURL.TrimEnd('/'), vm.ID, spHostURL);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.ViewEmployeeLink, viewEmployeeLink);
            processKeywords.AddKeywordValue(ConstantHelper.WorkflowKeywords.Common.ViewRequestLink, viewRequestLink);

            return processKeywords;
        }

        public static bool CompleteAWorkflowStep(string processID, string taskID, string remarks, string actionName, string spHostUrl, string accessToken)
        {
            bool completeSuccess = false;
            int _ProcessID = -1;
            int _TaskID = -1;

            if (int.TryParse(processID, out _ProcessID) && int.TryParse(taskID, out _TaskID))
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(clientContext);
                    Actioner actioner = new Actioner(currentUser.LoginName, currentUser.Title, currentUser.Email);
                    string wfConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(wfConnString);
                    wfBL.ActionTask(_TaskID, _ProcessID, actionName, actioner, remarks + "");
                    completeSuccess = true;
                }
            }
            else
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage("WorkflowBL - CompleteAWorkflowStep Error: Failed To Update AppAccessToken");
            }

            return completeSuccess;
        }

        public static int GetNewDueDay(string currentStep, string workflowCycleDueDays)
        {
            int dueDay = -1;
            try
            {
                if (!string.IsNullOrEmpty(workflowCycleDueDays))
                {
                    List<int> dueDays = workflowCycleDueDays.Split(',').Select(x => int.Parse(x)).ToList();
                    switch (currentStep)
                    {
                        case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingReportingManagerApproval:
                            dueDay = dueDays[0];
                            break;
                        case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingInfraTeamAction:
                            dueDay = dueDays[1];
                            break;
                        case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingApplicationTeamAction:
                            dueDay = dueDays[2];
                            break;
                        case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingITManagerApproval:
                            dueDay = dueDays[3];
                            break;
                        case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingDepartmentAdminAction:
                            dueDay = dueDays[4];
                            break;
                        case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingAcknowledgement:
                            dueDay = dueDays[5];
                            break;
                        case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingOriginatorResubmission:
                            dueDay = dueDays[6];
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage("WorfklowBL - GetNewDueDay error : " + ex.ToString());
            }

            return dueDay;
        }
        #endregion

        #region Workflow Code (Used by workflow engine)
        public string UpdateWFStage(string refkey, int processID, string processXML)
        {
            //string result = string.Empty;
            string curStep = string.Empty;
            string result = "Default";

            RequestDA da = new RequestDA();

            try
            {
                ProcessKeywords keywords = new ProcessKeywords(processXML);
                string connString = ConnectionStringHelper.GetGenericWFConnString();
                var wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(connString);

                string spHostUrl = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.SPHostURL);
                string appAccessToken = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.AppAccessToken);
                string requestID = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.RequestID);
                int intRequestID = int.Parse(requestID);
                string currentStep = wfBL.GetCurrentStepName(processID);

                if (curStep.Equals(ConstantHelper.WorkflowStatus.TERMINATED))
                {
                    result = String.Empty;
                }

                da.UpdateRequestWF(currentStep, intRequestID);
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage("WorkflowBL - UpdateWFStage Error : " + ex);
            }
            return result;
        }
        #endregion

        #region Workflow Management
        public static bool AddActioner(int processID, List<PeoplePickerUser> newActioner, string spHostURL, string accessToken, string appAccessToken)
        {
            LogHelper logHelper = new LogHelper();
            bool actionSuccess = false;
            try
            {
                using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    User curUser = SharePointHelper.GetCurrentUser(ctx);
                    PeoplePickerUser currentUser = ConvertEntitiesHelper.ConvertPeoplePickerUser(curUser);

                    if (WorkflowHelper.IsWorkflowRunning(processID))
                    {
                        actionSuccess = UpdateActioner(processID, newActioner, currentUser, spHostURL, accessToken, appAccessToken, false);
                    }
                    else
                    {
                        logHelper.LogMessage("WorkflowBL Error - Add Actioner Failed - Selected Workflow is not running : " + processID);
                        actionSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                logHelper.LogMessage(string.Format("WorkflowBL - Add Actioner Error: {0}", ex));
            }
            return actionSuccess;
        }

        private static bool UpdateActioner(int processID, List<PeoplePickerUser> newActioner, PeoplePickerUser currentUser, string spHostUrl, string accessToken, string appAccessToken, bool isReassign, bool addTaskNow = true)
        {
            bool ActionSuccess = false;
            List<Actioner> Users = new List<Actioner>();
            Users = WorkflowHelper.ContructActionerList(newActioner);
            Actioner wfCurrentUser = new Actioner();
            wfCurrentUser = WorkflowHelper.ConstructActioner(currentUser);

            string ConnString = ConnectionStringHelper.GetGenericWFConnString();
            MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(ConnString);
            ProcessInstance instance = wfBL.GetProcessInstance(processID);

            wfBL.AddOrReassignActionersForStep(processID, instance.CurStepTemplateID, Users, isReassign, addTaskNow, wfCurrentUser);

            ActionSuccess = true;
            return ActionSuccess;
        }

        public static bool ReassignActioner(int processID, int taskID, PeoplePickerUser newActioner, string spHostURL, string accessToken, string comments)
        {
            bool actionSuccess = false;
            LogHelper logHelper = new LogHelper();

            try
            {
                using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    User curUser = SharePointHelper.GetCurrentUser(ctx);
                    PeoplePickerUser currentUser = ConvertEntitiesHelper.ConvertPeoplePickerUser(curUser);
                    Actioner actionBy = WorkflowHelper.ConstructActioner(currentUser);
                    Actioner newWFActioner = WorkflowHelper.ConstructActioner(newActioner);
                    string connString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(connString);
                    ProcessInstance processInstance = wfBL.GetProcessInstance(processID);

                    var curTask = wfBL.GetTask(taskID);

                    PeoplePickerUser oldActioner = new PeoplePickerUser()
                    {
                        Email = curTask.Assignee?.Email,
                        Login = curTask.Assignee?.LoginName,
                        Name = curTask.Assignee?.Name
                    };

                    if (!wfBL.IsTaskCompleted(taskID))
                    {
                        if (!string.IsNullOrEmpty(comments))
                            comments = $"Reassigned to {newActioner.Name}. Remarks: {comments}";
                        else
                            comments = $"Reassigned to {newActioner.Name}";

                        int newTaskId = wfBL.ReassignTaskAndGetTaskIDForAdmin(taskID, actionBy, newWFActioner, comments);

                        if (newTaskId > -1) actionSuccess = true;

                        List<PeoplePickerUser> newUsers = new List<PeoplePickerUser>();
                        newUsers.Add(newActioner);
                        PeoplePickerUser taskOwner = WorkflowHelper.GetTaskOwnerByTaskID(taskID, wfBL);
                        List<PeoplePickerUser> taskOwnerList = new List<PeoplePickerUser>() { taskOwner };

                        //Get Delegate To
                        DelegationBL delegationBL = new DelegationBL(connString);
                        List<DelegationInstance> delegationInstancesList = delegationBL.GetDelegationsToUsers(new List<Actioner>() { newWFActioner }, processInstance.ApplicationName, processInstance.ProcessTemplateID);
                        DelegationInstance delegationInstance = delegationInstancesList?.FirstOrDefault(x => x.Active);

                        //Construct Delegatee
                        PeoplePickerUser delegatTo = new PeoplePickerUser();
                        if (delegationInstance != null)
                        {
                            string email = delegationInstance.DelegationToEmail;
                            string loginName = delegationInstance.DelegationTo;
                            string name = delegationInstance.DelegationToFriendlyName;

                            delegatTo.Email = email;
                            delegatTo.Login = loginName;
                            delegatTo.Name = name;
                        }

                        ProcessKeywords keywords = new ProcessKeywords(processInstance.KeywordsXML);
                        string reqRefNo = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.RequestRefNo);
                        string reqType = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.RequestType);

                        SendGeneralWorkflowEmailNotification(ConstantHelper.EmailTemplateKeyTitle.WorkflowTaskReassignedNotification, newActioner, oldActioner, currentUser, delegatTo, DateTime.MinValue, DateTime.MinValue, spHostURL, accessToken, reqRefNo, reqType);
                    }
                }
            }
            catch (Exception ex)
            {
                logHelper.LogMessage(string.Format("Workflow BL - Reassign Actioner Error: {0}", ex));
            }

            return actionSuccess;
        }

        public static bool RemoveActioner(int processID, int taskID, string workflowName, string remarks, string spHostURL, string accessToken, string appAccessToken)
        {
            bool actionSuccess = false;
            PeoplePickerUser currentUser = new PeoplePickerUser();

            try
            {
                using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    User curUser = SharePointHelper.GetCurrentUser(ctx);
                    currentUser = ConvertEntitiesHelper.ConvertPeoplePickerUser(curUser);
                }

                string connString = ConnectionStringHelper.GetGenericWFConnString();
                MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(connString);

                if (!wfBL.IsTaskCompleted(taskID))
                {
                    //Determine workflow name
                    string actionName = string.Empty;
                    if (!string.IsNullOrEmpty(workflowName))
                    {
                        if (workflowName.Equals(ConstantHelper.WorkflowName.ELANWorkflow))
                        {
                            string currentStep = wfBL.GetCurrentStepInternalName(processID);
                            switch (currentStep)
                            {
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingReportingManagerApproval:
                                    actionName = ConstantHelper.WorkflowActionName.Approve;
                                    break;
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingInfraTeamAction:
                                    actionName = ConstantHelper.WorkflowActionName.Complete;
                                    break;
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingApplicationTeamAction:
                                    actionName = ConstantHelper.WorkflowActionName.Complete;
                                    break;
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingITManagerApproval:
                                    actionName = ConstantHelper.WorkflowActionName.Approve;
                                    break;
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingDepartmentAdminAction:
                                    actionName = ConstantHelper.WorkflowActionName.Complete;
                                    break;
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingAcknowledgement:
                                    actionName = ConstantHelper.WorkflowActionName.Acknowledge;
                                    break;
                                default:
                                    throw new DataException("Invalid Workflow Step Name.");
                            }
                        }

                        else
                            throw new DataException("Invalid Workflow Name.");

                        Actioner actioner = new Actioner();
                        actioner = WorkflowHelper.ConstructActioner(currentUser);

                        wfBL.RemoveTask(processID, taskID, actionName, actioner, remarks);
                        actionSuccess = true;
                    }
                    else
                        throw new DataException("Workflow Type cannot be null or empty.");
                }
                else
                {
                    throw new Exception("Selected task already completed.");
                }
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage(string.Format("WorkflowBL - RemoveActioner Error: {0}", ex.ToString()));
            }

            return actionSuccess;
        }

        public static bool RemoveAllActioner(int processID, List<int> taskIDList, List<string> workflowNameList, string remarks, string spHostURL, string accessToken, string appAccessToken)
        {
            bool actionSuccess = false;
            PeoplePickerUser currentUser = new PeoplePickerUser();

            try
            {
                using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    User curUser = SharePointHelper.GetCurrentUser(ctx);
                    currentUser = ConvertEntitiesHelper.ConvertPeoplePickerUser(curUser);
                }

                //Determine workflow name
                string actionName;
                Actioner actioner = new Actioner();
                actioner = WorkflowHelper.ConstructActioner(currentUser);

                string ConnString = ConnectionStringHelper.GetGenericWFConnString();
                MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(ConnString);
                for (var i = 0; i < taskIDList.Count; i++)
                {
                    actionName = string.Empty;
                    if (!string.IsNullOrEmpty(workflowNameList[i]))
                    {
                        if (workflowNameList[i].Equals(ConstantHelper.WorkflowName.ELANWorkflow))
                        {
                            string currentStep = wfBL.GetCurrentStepInternalName(processID);
                            switch (currentStep)
                            {
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingReportingManagerApproval:
                                    actionName = ConstantHelper.WorkflowActionName.Approve;
                                    break;
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingInfraTeamAction:
                                    actionName = ConstantHelper.WorkflowActionName.Complete;
                                    break;
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingApplicationTeamAction:
                                    actionName = ConstantHelper.WorkflowActionName.Complete;
                                    break;
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingITManagerApproval:
                                    actionName = ConstantHelper.WorkflowActionName.Approve;
                                    break;
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingDepartmentAdminAction:
                                    actionName = ConstantHelper.WorkflowActionName.Complete;
                                    break;
                                case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingAcknowledgement:
                                    actionName = ConstantHelper.WorkflowActionName.Acknowledge;
                                    break;
                                default:
                                    throw new DataException("Invalid Workflow Step Name.");
                            }
                        }
                        else
                            throw new DataException("Invalid Workflow Name.");

                        wfBL.RemoveTask(processID, taskIDList[i], actionName, actioner, remarks);
                        actionSuccess = true;
                    }
                    else
                        throw new DataException("Workflow Type cannot be null or empty.");
                }

                actionSuccess = true;
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage(string.Format("WorkflowBL - RemoveAllActioner Error: {0}", ex.ToString()));
            }

            return actionSuccess;
        }

        public static bool AddActionerForPendingCompletion(string processID, string taskID, User curUser, ClientContext context, ViewModelApproval vm)
        {
            LogHelper logHelper = new LogHelper();
            bool actionSuccess = false;

            if (string.IsNullOrEmpty(processID) || string.IsNullOrEmpty(taskID))
            {
                logHelper.LogMessage("AddActionerForPendingCompletion: Invalid processID or taskID.");
                return false;
            }

            if (curUser == null)
            {
                logHelper.LogMessage("AddActionerForPendingCompletion: Failed to load the current user.");
                return false;
            }

            try
            {
                var curActioner = new Actioner(curUser.LoginName, curUser.Title, curUser.Email);
                string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);

                int.TryParse(processID, out int processIDInt);
                MHA.Framework.Core.Workflow.DL.InstanceDL instanceDL = new MHA.Framework.Core.Workflow.DL.InstanceDL(WFConnString);
                string currentStep = wfBL.GetCurrentStepInternalName(processIDInt);

                ProcessKeywords keywords = wfBL.GetKeywords(processIDInt);

                string pendingAcknowledgementStep = ConstantHelper.WorkflowStepName.ELANWorkflow.PendingAcknowledgement;
                StepInstance stepInstance = instanceDL.GetStepInstanceByStepName(processIDInt, pendingAcknowledgementStep);

                // assign to 
                List<Actioner> empActionerList = new List<Actioner>();
                string requestType = vm?.ViewRequestItem?.RequestType;

                if (requestType == ConstantHelper.RequestForm.RequestType.NewRequestType ||
                    requestType == ConstantHelper.RequestForm.RequestType.ModificationRequestType ||
                    requestType == ConstantHelper.RequestForm.RequestType.PromotionRequestType ||
                    requestType == ConstantHelper.RequestForm.RequestType.TransferRequestType)
                {
                    Actioner actioner = new Actioner
                    {
                        LoginName = vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin,
                        Name = vm.EmployeeDetails.EmployeeDetailsVM.EmployeeName,
                        Email = vm.EmployeeDetails.EmployeeDetailsVM.EmployeeEmail
                    };
                    empActionerList.Add(actioner);
                }
                else if (requestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType)
                {
                    Actioner actioner = new Actioner
                    {
                        LoginName = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerLogin,
                        Name = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerName,
                        Email = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerEmail
                    };
                    empActionerList.Add(actioner);
                }
                else
                {
                    throw new Exception($"Unsupported RequestType: '{requestType}'.");
                }

                if (empActionerList.Count == 0)
                {
                    throw new Exception($"No actioner to assign to the stage");
                }

                wfBL.UpdateActionersForStep(processIDInt, stepInstance.InternalStepName, empActionerList, true, true, curActioner);
                actionSuccess = true;
            }
            catch (Exception ex)
            {
                logHelper.LogMessage($"AddActionerForPendingCompletion encountered an error: {ex.Message}\n{ex.StackTrace}");
            }
            return actionSuccess;
        }
        #endregion

        #region Email Notifications
        private static void SendGeneralWorkflowEmailNotification(string emailTitle, PeoplePickerUser newActioner, PeoplePickerUser oldActioner, PeoplePickerUser currentUser, PeoplePickerUser delegator, DateTime startDate, DateTime endDate, string spHostURL, string accessToken, string reqRefNo, string reqType)
        {
            string emailSubject = string.Empty;
            string emailBody = string.Empty;
            EmailTemplateBL.GetEmailTemplateByTemplateTitle(emailTitle, ref emailSubject, ref emailBody, spHostURL, accessToken);
            if (!string.IsNullOrEmpty(emailSubject) && !string.IsNullOrEmpty(emailBody))
            {
                //Initialize the CC Email Address Collection
                MailAddressCollection emailCC = new MailAddressCollection();

                string homePageURL = string.Format(ConstantHelper.URLTemplate.HomePageUrlTemplate, appSettings.RemoteAppURL.TrimEnd('/'), spHostURL);

                //Build email subject
                if (emailTitle.Equals(ConstantHelper.EmailTemplateKeyTitle.WorkflowTaskReassignedNotification))
                {
                    emailSubject = BuildReassignedEmailSubject(emailSubject);
                    emailBody = BuildReassignedEmailBody(emailBody, homePageURL, newActioner, oldActioner, currentUser, reqRefNo, reqType);

                    //Push the delegatee to CC
                    if (delegator != null && !string.IsNullOrEmpty(delegator.Email))
                        emailCC.Add(new MailAddress(delegator.Email));
                }
                else if (emailTitle.Equals(ConstantHelper.EmailTemplateKeyTitle.WorkflowTaskDelegatedNotification))
                {
                    emailSubject = BuildDelegatedEmailSubject(emailSubject, delegator);
                    emailBody = BuildDelegatedEmailBody(emailBody, homePageURL, newActioner, delegator, startDate, endDate);
                }
                else
                    throw new Exception("Invalid email notification title has been selected");

                MailAddressCollection emailTo = new MailAddressCollection();
                if (!string.IsNullOrEmpty(newActioner.Email))
                    emailTo.Add(new MailAddress(newActioner.Email));

                EmailHelper.SendEmailWithSender(appSettings.rerAccountEmail, appSettings.rerAccountPassword, emailTo, emailCC, emailSubject, emailBody, new List<System.Net.Mail.Attachment>());
            }
        }

        private static string BuildReassignedEmailSubject(string emailSubject)
        {
            emailSubject = emailSubject.Replace("{ProjectName}", ConstantHelper.Module.ELAN);
            return emailSubject;
        }

        private static string BuildReassignedEmailBody(string emailBody, string homePageURL, PeoplePickerUser newActioner, PeoplePickerUser oldActioner, PeoplePickerUser currentUser, string reqRefNo, string reqType)
        {
            emailBody = emailBody.Replace("{ProjectName}", ConstantHelper.Module.ELAN);
            emailBody = emailBody.Replace("{HomePageURL}", homePageURL);
            emailBody = emailBody.Replace("{NewActioner}", newActioner.Name);
            emailBody = emailBody.Replace("{OldActioner}", oldActioner.Name);
            emailBody = emailBody.Replace("{Assigner}", currentUser.Name);

            emailBody = emailBody.Replace("{ReqRefNo}", reqRefNo);
            emailBody = emailBody.Replace("{ReqType}", reqType);
            return emailBody;
        }

        private static string BuildDelegatedEmailSubject(string emailSubject, PeoplePickerUser delegator)
        {
            emailSubject = emailSubject.Replace("{ProjectName}", ConstantHelper.Module.ELAN);
            emailSubject = emailSubject.Replace("{Delegator}", delegator.Name);
            return emailSubject;
        }

        private static string BuildDelegatedEmailBody(string emailBody, string homePageURL, PeoplePickerUser newActioner, PeoplePickerUser delegator, DateTime startDate, DateTime endDate)
        {
            emailBody = emailBody.Replace("{ProjectName}", ConstantHelper.Module.ELAN);
            emailBody = emailBody.Replace("{HomePageURL}", homePageURL);
            emailBody = emailBody.Replace("{Delegator}", delegator.Name);
            emailBody = emailBody.Replace("{NewActioner}", newActioner.Name);
            emailBody = emailBody.Replace("{StartDate}", startDate.ToString(appSettings.DefaultDateFormat));
            emailBody = emailBody.Replace("{EndDate}", endDate.ToString(appSettings.DefaultDateFormat));
            return emailBody;
        }
        #endregion

        #region General Functions
        private bool ShowRemoveAction(string status)
        {
            bool isShow = false;

            bool isInprogress = status.Equals(ConstantHelper.TaskStatus.IN_Progress);

            if (isInprogress)
            {
                isShow = true;
            }

            return isShow;
        }
        #endregion

        #region Lock Method
        private static object workflowLock = new object();
        public static bool CheckIsTaskNotStarted(string taskId, bool isMyTask)
        {
            bool result = false;
            if (isMyTask && !string.IsNullOrEmpty(taskId) && int.TryParse(taskId, out int _taskId))
            {
                try
                {
                    WorkflowDA workflowDA = new WorkflowDA();
                    lock (workflowLock)
                    {
                        //Check is exist in table
                        if (!workflowDA.IsTaskOngoing(_taskId))
                        {
                            //Insert New Row in table
                            workflowDA.InsertOnGoingTasks(_taskId);
                            result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper logHelper = new LogHelper();
                    logHelper.LogMessage("WorkflowBL - CheckIsTaskNotStarted Error: TaskID : " + taskId + " :" + ex.ToString());
                }
            }
            return result;
        }

        public static bool CloseRunningTask(string taskId, bool isNotSkip)
        {
            bool result = false;
            if (isNotSkip)
            {
                if (!string.IsNullOrEmpty(taskId) && int.TryParse(taskId, out int _taskId))
                {
                    try
                    {
                        WorkflowDA workflowDA = new WorkflowDA();
                        lock (workflowLock)
                        {
                            //Check is exist in table
                            workflowDA.UpdateOnGoingTasks(_taskId);
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper logHelper = new LogHelper();
                        logHelper.LogMessage("WorkflowBL - CloseRunningTask Error: TaskID : " + taskId + " :" + ex.ToString());
                    }
                }
            }
            return result;
        }
        #endregion
    }
}

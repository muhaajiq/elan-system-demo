using MHA.Framework.Core.SP;
using MHA.Framework.Core.Workflow.BO;
using MHA.ELAN.Data;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using MHA.ELAN.Framework.JSONConstants;
using Microsoft.SharePoint.Client;
using System.Data;
using System.Net.Mail;
using static MHA.ELAN.Framework.Constants.ConstantHelper;

namespace MHA.ELAN.Business
{
    public class ApprovalFormBL
    {
        private static readonly JSONAppSettings appSettings;

        static ApprovalFormBL()
        {
            appSettings = ConfigurationManager.GetAppSetting();
        }

        #region Init
        public async Task<ViewModelApproval> InitApprovalForm(ViewModelApproval vm, string spHostUrl, string accessToken, string RequestId, string TaskId)
        {
            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(clientContext);
                    vm.CurrentUser = currentUser.LoginName;

                    string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                    Actioner actioner = new Actioner(currentUser.LoginName, currentUser.Title, currentUser.Email);

                    var isMyTask = wfBL.CheckIsMyTask(int.Parse(TaskId), actioner);

                    if (isMyTask)
                    {
                        vm.IsMyTask = isMyTask;
                        string[] groups = [SPSecurityGroup.ElanAdmin, SPSecurityGroup.DepartmentAdmin, SPSecurityGroup.ITManager, SPSecurityGroup.ITApplications, SPSecurityGroup.ITInfra, SPSecurityGroup.ElanMember];

                        foreach (string group in groups)
                        {
                            bool inGroup = false;

                            inGroup = SharePointHelper.IsUserInGroup(clientContext, string.Empty, group);

                            vm.AccessGroups.Add(group);
                        }

                        if (vm.AccessGroups.Contains(SPSecurityGroup.DepartmentAdmin))
                        {
                            string queryCondition = GeneralQueryHelper.ConcatCriteria(null, "Admin", "Integer", currentUser.Id.ToString(), "Contains", true);

                            ListItemCollection departmentItems = GeneralQueryHelper.GetSPItems(clientContext, SPList.Department, queryCondition, [SPColumn.Department.Title]);

                            if (departmentItems != null)
                            {
                                foreach (ListItem item in departmentItems)
                                {
                                    vm.AccessDepartments.Add(item[SPColumn.Department.Title].ToString());
                                }
                            }
                        }

                        //temp object to store original value
                        ViewModelApproval obj = new();
                        int requestId = int.Parse(RequestId);
                        obj = vm;

                        //Store retrieved data into temp object
                        RequestDA reqDA = new RequestDA();
                        DataTable requestDT = reqDA.RetrieveRequestByID(requestId);
                        obj.ViewRequestItem = ConvertEntitiesHelper.ConvertViewRequestItemObj(requestDT);

                        DataTable employeeDT = reqDA.RetrieveEmployeeDetailsByRequestID(requestId);
                        obj.EmployeeDetails = ConvertEntitiesHelper.ConvertViewEmployeeDetailsItemObj(employeeDT);

                        DataTable hardwareDT = reqDA.RetrieveHardwareByRequestID(requestId);
                        obj.HardwareItems = ConvertEntitiesHelper.ConvertViewHardwareItemObj(hardwareDT);

                        DataTable ITReqDT = reqDA.RetrieveITRequirementsByRequestID(requestId);
                        obj.ITRequirements = ConvertEntitiesHelper.ConvertViewITRequirementsItemObj(ITReqDT);

                        DataTable folderPermissionDT = reqDA.RetrieveFolderPermissionByRequestID(requestId);
                        obj.FolderPermissions = ConvertEntitiesHelper.ConvertViewFolderPermissionObj(folderPermissionDT);

                        DataTable changeDT = reqDA.RetrieveChangesByRequestID(requestId);
                        obj.changes = ConvertEntitiesHelper.ConvertChangesObj(changeDT);

                        obj.EmployeeDetails.RequestType = obj.ViewRequestItem.RequestType;
                        obj.CurrentStage = wfBL.GetCurrentStepName(obj.ViewRequestItem.ProcessID.Value);

                        vm = obj;

                        if (vm.ViewRequestItem.RequestType == RequestForm.RequestType.TerminationRequestType)
                        {
                            if (!string.IsNullOrEmpty(vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManagerLogin))
                            {
                                vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManager = new PeoplePickerUser
                                {
                                    Login = vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManagerLogin,
                                    Name = vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManagerName,
                                    Email = vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManagerEmail
                                };
                            }

                            await GetSubordinatesByRepManagerLogin(vm);
                        }

                        ////check role, if permission met, assign temp object to vm and return vm
                        //if (vm.AccessGroups.Contains(SPSecurityGroup.ElanAdmin))
                        //{
                        //    vm = obj;
                        //    vm.IsValid = true;
                        //}
                        //else if (vm.AccessGroups.Contains(SPSecurityGroup.DepartmentAdmin))
                        //{
                        //    if (vm.AccessDepartments.Contains(vm.EmployeeDetails.DepartmentTitle)) vm = obj;
                        //    vm.IsValid = true;
                        //}
                    }
                    else
                    {
                        vm.HasError = true;
                        vm.IsMyTask = false;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return vm;
        }

        public async Task<RequestFormVisibilitySettings> SetVisibilitySettings(string currentStage)
        {
            RequestFormVisibilitySettings vm = new();

            if (currentStage == null) return vm;

            if (currentStage == WorkflowStatus.PENDING_REPORTING_MANAGER_APPROVAL)
            {
                vm.ShowRequireAmendment = true;
                vm.ShowApprove = true;
                vm.ShowReject = true;
                vm.ShowClose = true;

                vm.ShowInfra = true;
                vm.ShowApplications = true;
                vm.ShowFolderPermission = true;
            }
            else if (currentStage == WorkflowStatus.PENDING_INFRA_TEAM_ACTION)
            {
                vm.ShowSave = true;
                vm.ShowComplete = true;
                vm.ShowClose = true;

                vm.ShowInfra = true;
                vm.ShowFolderPermission = true;
                vm.ShowEmployeePeoplePicker = true;
            }
            else if (currentStage == WorkflowStatus.PENDING_APPLICATION_TEAM_ACTION)
            {
                vm.ShowSave = true;
                vm.ShowComplete = true;
                vm.ShowClose = true;

                vm.ShowApplications = true;
            }
            else if (currentStage == WorkflowStatus.PENDING_IT_MANAGER_APPROVAL)
            {
                vm.ShowRequireAmendment = true;
                vm.ShowApprove = true;
                vm.ShowClose = true;

                vm.ShowInfra = true;
                vm.ShowApplications = true;
                vm.ShowFolderPermission = true;
            }
            else if (currentStage == WorkflowStatus.PENDING_DEPARTMENT_ADMIN_ACTION)
            {
                vm.ShowSave = true;
                vm.ShowComplete = true;
                vm.ShowClose = true;

                vm.ShowInfra = true;
                vm.ShowApplications = true;
                vm.ShowFolderPermission = true;

                vm.ShowHardwareDetails = true;
                vm.ShowColumnsAsEditable = true;
            }
            else if (currentStage == WorkflowStatus.PENDING_ACKNOWLEDGEMENT)
            {
                vm.ShowComplete = true;
                vm.ShowClose = true;

                vm.ShowInfra = true;
                vm.ShowApplications = true;
                vm.ShowFolderPermission = true;
                vm.ShowAcknowledgement = true;

                vm.ShowHardwareDetails = true;
            }
            else
            {
                vm.ShowClose = true;
            }

            return vm;
        }

        #endregion

        #region Require Amendment
        public async Task<ViewModelApproval> RequireAmendmentRequest(ViewModelApproval vm, string spHostUrl, string accessToken)
        {
            RequestDA da = new RequestDA();
            bool isTaskNotStarted = false;
            int processID = vm.ProcessID.GetValueOrDefault();
            int taskID = vm.TaskID.GetValueOrDefault();
            bool ActionSuccess = false;
            string remarks = vm.EmployeeDetails.EmployeeDetailsVM.Remarks;

            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(clientContext);
                    Actioner actioner = new Actioner(currentUser.LoginName, currentUser.Title, currentUser.Email);

                    string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                    bool isTaskActive = wfBL.CheckIsMyTask(taskID, actioner);
                    isTaskNotStarted = WorkflowBL.CheckIsTaskNotStarted(taskID.ToString(), isTaskActive);

                    if (isTaskActive && isTaskNotStarted)
                    {
                        if (vm.ViewRequestItem.WorkflowStatus == WorkflowStatus.PENDING_REPORTING_MANAGER_APPROVAL)
                        {
                            da.UpdateApprovalRequest(vm, isRemarksOnly: true);
                        }
                        else if (vm.ViewRequestItem.WorkflowStatus == WorkflowStatus.PENDING_IT_MANAGER_APPROVAL)
                        {
                            da.UpdateApprovalRequest(vm);
                        }

                        if (processID > 0)
                        {
                            ProcessKeywords keywords = wfBL.GetKeywords(processID);
                            keywords.AddKeywordValue(WorkflowKeywords.Common.Remarks, remarks);
                            wfBL.UpdateKeywords(processID, keywords);

                            // Assign back to originator
                            string originatorLogin = keywords.GetKeywordValue(WorkflowKeywords.Common.OriginatorSubmittedByLogin);
                            string originatorName = keywords.GetKeywordValue(WorkflowKeywords.Common.OriginatorSubmittedByName);
                            string originatorEmail = keywords.GetKeywordValue(WorkflowKeywords.Common.OriginatorSubmittedByEmail);

                            if (!string.IsNullOrEmpty(originatorLogin) && !string.IsNullOrEmpty(originatorName) && !string.IsNullOrEmpty(originatorEmail))
                            {
                                var originator = new Actioner(originatorLogin, originatorName, originatorEmail);
                                string resubmissionStage = WorkflowStepName.ELANWorkflow.PendingOriginatorResubmission;

                                MHA.Framework.Core.Workflow.DL.InstanceDL instanceDL = new MHA.Framework.Core.Workflow.DL.InstanceDL(WFConnString);
                                StepInstance stepInstance = instanceDL.GetStepInstanceByStepName(processID, resubmissionStage);

                                wfBL.UpdateActionersForStep(processID, stepInstance.InternalStepName, new List<Actioner> { originator }, true, true, actioner);

                                // Update due date
                                string workflowCycleDueDays = keywords.GetKeywordValue(WorkflowKeywords.Common.WorkflowCycleDueDays);
                                int dueDay = WorkflowBL.GetNewDueDay(resubmissionStage, workflowCycleDueDays);
                                wfBL.UpdateStepTaskDueDate(processID, resubmissionStage, dueDay);
                            }
                        }

                        ActionSuccess = WorkflowBL.CompleteAWorkflowStep(
                            processID.ToString(),
                            taskID.ToString(),
                            remarks,
                            WorkflowActionName.RequireAmendment,
                            spHostUrl,
                            accessToken
                        );

                        vm.HasSuccess = ActionSuccess;
                    }
                    else if (!isTaskActive)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                    else if (!isTaskNotStarted)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.TaskInProgressErrorMsg;
                    }
                    else
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                }
            }
            catch (Exception ex)
            {
                vm.HasError = true;
                vm.ErrorMessage = ErrorMessage.UnexpectedErrorOccur;
            }
            finally
            {
                WorkflowBL.CloseRunningTask(taskID.ToString(), isTaskNotStarted);
            }

            return vm;
        }
        #endregion

        #region Approve
        public async Task<ViewModelApproval> ApproveRequest(ViewModelApproval vm, string spHostUrl, string accessToken)
        {
            RequestDA da = new RequestDA();
            bool isTaskNotStarted = false;
            int processID = vm.ProcessID.GetValueOrDefault();
            int taskID = vm.TaskID.GetValueOrDefault();
            bool ActionSuccess = false;
            string remarks = vm.EmployeeDetails.EmployeeDetailsVM.Remarks;

            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(clientContext);
                    vm.ModifiedBy = currentUser.Title;
                    vm.ModifiedByLogin = currentUser.LoginName;
                    vm.Modified = DateTimeHelper.GetCurrentDateTime();

                    Actioner actioner = new Actioner(currentUser.LoginName, currentUser.Title, currentUser.Email);

                    string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                    bool isTaskActive = wfBL.CheckIsMyTask(taskID, actioner);
                    isTaskNotStarted = WorkflowBL.CheckIsTaskNotStarted(taskID.ToString(), isTaskActive);

                    if(isTaskActive && isTaskNotStarted)
                    {
                        if(vm.ViewRequestItem.WorkflowStatus == WorkflowStatus.PENDING_REPORTING_MANAGER_APPROVAL)
                        {
                            da.UpdateApprovalRequest(vm, isRemarksOnly: true);
                        }
                        else if(vm.ViewRequestItem.WorkflowStatus == WorkflowStatus.PENDING_IT_MANAGER_APPROVAL)
                        {
                            da.UpdateApprovalRequest(vm);
                        }

                        if (processID > 0)
                        {
                            ProcessKeywords keywords = wfBL.GetKeywords(processID);
                            keywords.AddKeywordValue(WorkflowKeywords.Common.Remarks, remarks);
                            wfBL.UpdateKeywords(processID, keywords);
                        }

                        ActionSuccess = WorkflowBL.CompleteAWorkflowStep(
                            processID.ToString(), 
                            taskID.ToString(), 
                            remarks, 
                            WorkflowActionName.Approve, 
                            spHostUrl, 
                            accessToken);

                        vm.HasSuccess = ActionSuccess;
                    }
                    else if (!isTaskActive)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                    else if (!isTaskNotStarted)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.TaskInProgressErrorMsg;
                    }
                    else
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                WorkflowBL.CloseRunningTask(taskID.ToString(), isTaskNotStarted);
            }

            return vm;
        }
        #endregion

        #region Reject
        public async Task<ViewModelApproval> RejectRequest(ViewModelApproval vm, string spHostUrl, string accessToken)
        {
            RequestDA da = new RequestDA();
            bool isTaskNotStarted = false;
            int processID = vm.ProcessID.GetValueOrDefault();
            int taskID = vm.TaskID.GetValueOrDefault();
            bool ActionSuccess = false;
            string rejectRemarks = vm.EmployeeDetails.EmployeeDetailsVM.Remarks;

            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(clientContext);
                    Actioner actioner = new Actioner(currentUser.LoginName, currentUser.Title, currentUser.Email);

                    string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                    bool isTaskActive = wfBL.CheckIsMyTask(taskID, actioner);
                    isTaskNotStarted = WorkflowBL.CheckIsTaskNotStarted(taskID.ToString(), isTaskActive);

                    if (isTaskActive && isTaskNotStarted)
                    {
                        da.UpdateApprovalRequest(vm, isRemarksOnly: true);

                        if (processID > 0)
                        {
                            ProcessKeywords keywords = wfBL.GetKeywords(processID);
                            rejectRemarks = string.Format(InfoMessage.RejectRemarks, rejectRemarks);
                            keywords.AddKeywordValue(WorkflowKeywords.Common.Remarks, rejectRemarks);
                            wfBL.UpdateKeywords(processID, keywords);
                        }

                        ActionSuccess = WorkflowBL.CompleteAWorkflowStep(
                            processID.ToString(), 
                            taskID.ToString(), 
                            rejectRemarks, 
                            WorkflowActionName.Reject, 
                            spHostUrl, 
                            accessToken);

                        vm.HasSuccess = ActionSuccess;
                    }
                    else if (!isTaskActive)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                    else if (!isTaskNotStarted)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.TaskInProgressErrorMsg;
                    }
                    else
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                WorkflowBL.CloseRunningTask(taskID.ToString(), isTaskNotStarted);
            }

            return vm;
        }
        #endregion

        #region Save
        public async Task<ViewModelApproval> SaveRequest(ViewModelApproval vm, string spHostUrl, string accessToken)
        {
            RequestDA da = new RequestDA();
            int processID = vm.ProcessID.GetValueOrDefault();
            int taskID = vm.TaskID.GetValueOrDefault();

            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(clientContext);
                    vm.ModifiedBy = currentUser.Title;
                    vm.ModifiedByLogin = currentUser.LoginName;
                    vm.Modified = DateTimeHelper.GetCurrentDateTime();

                    Actioner actioner = new Actioner(currentUser.LoginName, currentUser.Title, currentUser.Email);
                    string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                    bool isTaskActive = wfBL.CheckIsMyTask(taskID, actioner);

                    if (isTaskActive)
                    {
                        da.UpdateApprovalRequest(vm);
                        vm.HasSuccess = true;
                    }
                    else if (!isTaskActive)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return vm;
        }
        #endregion

        #region Complete
        public async Task<ViewModelApproval> CompleteRequest(ViewModelApproval vm, string spHostUrl, string accessToken)
        {
            RequestDA da = new RequestDA();
            bool isTaskNotStarted = false;
            int processID = vm.ProcessID.GetValueOrDefault();
            int taskID = vm.TaskID.GetValueOrDefault();
            bool ActionSuccess = false;
            string remarks = vm.EmployeeDetails.EmployeeDetailsVM.Remarks;
            string currentWorkflowStep = string.Empty;

            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(clientContext);
                    vm.ModifiedBy = currentUser.Title;
                    vm.ModifiedByLogin = currentUser.LoginName;
                    vm.Modified = DateTimeHelper.GetCurrentDateTime();

                    Actioner actioner = new Actioner(currentUser.LoginName, currentUser.Title, currentUser.Email);

                    string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                    currentWorkflowStep = wfBL.GetCurrentStepName(processID);
                    bool isTaskActive = wfBL.CheckIsMyTask(taskID, actioner);
                    isTaskNotStarted = WorkflowBL.CheckIsTaskNotStarted(taskID.ToString(), isTaskActive);

                    if (isTaskActive && isTaskNotStarted)
                    {
                        bool success = da.UpdateApprovalRequest(vm);

                        if (success) 
                        {
                            if (processID > 0)
                            {
                                ProcessKeywords keywords = wfBL.GetKeywords(processID);
                                keywords.AddKeywordValue(WorkflowKeywords.Common.Remarks, remarks);
                                wfBL.UpdateKeywords(processID, keywords);
                            }

                            if (currentWorkflowStep == WorkflowStatus.PENDING_DEPARTMENT_ADMIN_ACTION)
                            {
                                //Add recipient actioners when the last actioner that complete the task
                                var workflowDA = new WorkflowDA();
                                int stepID = workflowDA.GetStepTemplateIDByInternalName(WorkflowStepName.ELANWorkflow.PendingDepartmentAdminAction);
                                DataTable activeActioners = workflowDA.GetSpecificStageActiveActioner(processID, stepID);

                                bool shouldAddActioner = activeActioners?.Rows.Count == 1;
                                bool actionerAdded = true;

                                if (shouldAddActioner)
                                {
                                    actionerAdded = WorkflowBL.AddActionerForPendingCompletion(processID.ToString(), taskID.ToString(), currentUser, clientContext, vm);
                                }

                                if (actionerAdded)
                                {
                                    ActionSuccess = WorkflowBL.CompleteAWorkflowStep(
                                        processID.ToString(),
                                        taskID.ToString(),
                                        remarks,
                                        WorkflowActionName.Complete,
                                        spHostUrl,
                                        accessToken
                                    );
                                }
                            }
                            else
                            {
                                ActionSuccess = WorkflowBL.CompleteAWorkflowStep(
                                    processID.ToString(),
                                    taskID.ToString(),
                                    remarks,
                                    WorkflowActionName.Complete,
                                    spHostUrl,
                                    accessToken
                                );
                            }

                            vm.HasSuccess = ActionSuccess;
                        }
                    }
                    else if (!isTaskActive)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                    else if (!isTaskNotStarted)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.TaskInProgressErrorMsg;
                    }
                    else
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                }
            }
            catch (Exception ex)
            {
                vm.HasError = true;
                vm.ErrorMessage = ErrorMessage.UnexpectedErrorOccur;
            }
            finally
            {
                WorkflowBL.CloseRunningTask(taskID.ToString(), isTaskNotStarted);
            }

            return vm;
        }
        #endregion

        #region Acknowledge
        public static async Task<ViewModelApproval> AcknowledgeRequest(ViewModelApproval vm, string spHostUrl, string accessToken)
        {
            RequestDA da = new RequestDA();
            bool isTaskNotStarted = false;
            int processID = vm.ProcessID.GetValueOrDefault();
            int taskID = vm.TaskID.GetValueOrDefault();
            int requestID = vm.RequestID.GetValueOrDefault();
            bool ActionSuccess = false;
            string remarks = vm.EmployeeDetails.EmployeeDetailsVM.Remarks;
            string currentWorkflowStep = string.Empty;

            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(clientContext);
                    vm.ModifiedBy = currentUser.Title;
                    vm.ModifiedByLogin = currentUser.LoginName;
                    vm.Modified = DateTimeHelper.GetCurrentDateTime();

                    Actioner actioner = new Actioner(currentUser.LoginName, currentUser.Title, currentUser.Email);

                    string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                    currentWorkflowStep = wfBL.GetCurrentStepName(processID);
                    bool isTaskActive = wfBL.CheckIsMyTask(taskID, actioner);
                    isTaskNotStarted = WorkflowBL.CheckIsTaskNotStarted(taskID.ToString(), isTaskActive);

                    if (isTaskActive && isTaskNotStarted)
                    {
                        bool success = da.UpdateApprovalRequest(vm);

                        if (success)
                        {
                            if (processID > 0)
                            {
                                ProcessKeywords keywords = wfBL.GetKeywords(processID);
                                keywords.AddKeywordValue(WorkflowKeywords.Common.Remarks, remarks);
                                wfBL.UpdateKeywords(processID, keywords);
                            }

                            ActionSuccess = WorkflowBL.CompleteAWorkflowStep(
                                    processID.ToString(),
                                    taskID.ToString(),
                                    remarks,
                                    WorkflowActionName.Acknowledge,
                                    spHostUrl,
                                    accessToken);

                            vm.HasSuccess = ActionSuccess;

                            if (ActionSuccess && currentWorkflowStep == WorkflowStatus.PENDING_ACKNOWLEDGEMENT)
                            {
                                var obj = new ViewModelApproval();

                                #region Insert/Update - Final Table
                                DataTable dtRequest = da.RetrieveRequestByRequestID(requestID);
                                obj.ViewRequestItem = ConvertEntitiesHelper.ConvertViewRequestItemObj(dtRequest);

                                DataTable dtEmployeeDetails = da.RetrieveEmployeeDetailsByRequestID(requestID);
                                obj.FinalEmployeeDetails = ConvertEntitiesHelper.ConvertViewModelFinalEmployeeDetailsObj(dtEmployeeDetails);

                                DataTable dtItReq = da.RetrieveITRequirementsByRequestID(requestID);
                                obj.FinalITRequirements = ConvertEntitiesHelper.ConvertViewModelFinalITRequirementsObj(dtItReq);

                                DataTable dtFolderPermission = da.RetrieveFolderPermissionByRequestID(requestID);
                                obj.FinalFolderPermissions = ConvertEntitiesHelper.ConvertViewModelFinalFolderPermission(dtFolderPermission);

                                DataTable dtHardware = da.RetrieveHardwareByRequestID(requestID);
                                obj.FinalHardwareItems = ConvertEntitiesHelper.ConvertViewModelFinalHardwareObj(dtHardware);

                                da.CreateFinalEmployeeInstance(obj);
                                #endregion

                                //TODO: Send Email (Who is going to receive the email for the termination request) - currently Reporting Manager
                                //TODO: New Email Receipt for Property Return Acknowledgement? - Currently send new email to Reporting Manager
                                #region Generate PDF and Send Email

                                if (obj.ViewRequestItem.RequestType == RequestForm.RequestType.TerminationRequestType)
                                {
                                    // ===== Manager Copy (Resigned Employee Property Return) =====
                                    var managerPdf = ReceiptPdfHelper.GenerateManagerReceiptPdf(obj.FinalEmployeeDetails, obj.FinalHardwareItems);
                                    var managerAttachments = new List<System.Net.Mail.Attachment>
                                    {
                                        new System.Net.Mail.Attachment(new MemoryStream(managerPdf), "PropertyReturnAcknowledgement_ManagerCopy.pdf", "application/pdf")
                                    };

                                    var emailTo = new MailAddressCollection
                                    {
                                        new MailAddress(obj.FinalEmployeeDetails.Final_EmployeeDetails.ReportingManagerEmail)
                                    };
                                    var emailCC = new MailAddressCollection();

                                    var emailSubject = $"Property Return Acknowledgement - {obj.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeName}";
                                    var emailBody = $"Dear {obj.FinalEmployeeDetails.Final_EmployeeDetails.ReportingManagerName},<br/><br/>" +
                                                    $"Attached is the property return acknowledgement receipt for your resigned employee, {obj.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeName}, for your reference.<br/><br/>" +
                                                    $"Regards,<br/>System Administrator";

                                    EmailHelper.SendEmailWithSender(
                                        appSettings.rerAccountEmail,
                                        appSettings.rerAccountPassword,
                                        emailTo, emailCC,
                                        emailSubject,
                                        emailBody,
                                        managerAttachments
                                    );
                                }
                                else
                                {
                                    // ===== Employee Copy (Normal flow) =====
                                    var itItems = obj.FinalHardwareItems
                                        .Where(h => h.Final_Hardware.DepartmentTitle == RequestForm.DepartmentName.ITDepartment)
                                        .ToList();

                                    var normalItems = obj.FinalHardwareItems
                                        .Where(h => h.Final_Hardware.DepartmentTitle != RequestForm.DepartmentName.ITDepartment)
                                        .ToList();

                                    var emailTo = new MailAddressCollection
                                    {
                                        new MailAddress(obj.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeEmail)
                                    };
                                    var emailCC = new MailAddressCollection();

                                    var emailSubject = $"Property Collection Acknowledgement Receipt - {obj.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeName}";
                                    var emailBody = $"Dear {obj.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeName},<br/><br/>" +
                                                    $"Please find attached your acknowledgement receipt.<br/><br/>Regards,<br/>System Administrator";

                                    // ===== IT Department Items =====
                                    if (itItems.Any())
                                    {
                                        var itPdf = ReceiptPdfHelper.GenerateITDepartReceiptPdf(obj.FinalEmployeeDetails, itItems);
                                        var itAttachments = new List<System.Net.Mail.Attachment>
                                        {
                                            new System.Net.Mail.Attachment(new MemoryStream(itPdf), "AcknowledgementReceipt_IT.pdf", "application/pdf")
                                        };

                                        EmailHelper.SendEmailWithSender(
                                            appSettings.rerAccountEmail,
                                            appSettings.rerAccountPassword,
                                            emailTo, emailCC,
                                            "[IT Items] " + emailSubject,
                                            emailBody,
                                            itAttachments
                                        );
                                    }

                                    // ===== Normal Items =====
                                    if (normalItems.Any())
                                    {
                                        var normalPdf = ReceiptPdfHelper.GenerateNormalReceiptPdf(obj.FinalEmployeeDetails, normalItems, obj.ViewRequestItem.ReferenceNo);
                                        var normalAttachments = new List<System.Net.Mail.Attachment>
                                        {
                                            new System.Net.Mail.Attachment(new MemoryStream(normalPdf), "AcknowledgementReceipt.pdf", "application/pdf")
                                        };

                                        EmailHelper.SendEmailWithSender(
                                            appSettings.rerAccountEmail,
                                            appSettings.rerAccountPassword,
                                            emailTo, emailCC,
                                            emailSubject,
                                            emailBody,
                                            normalAttachments
                                        );
                                    }
                                }
                                #endregion

                                #region Update Subordinates
                                if (vm.ViewRequestItem.RequestType == RequestForm.RequestType.TerminationRequestType)
                                {
                                    DataTable subordinateDT = da.RetrieveFinalEmployeeDetailsByRepManagerLogin(obj.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeLogin);
                                    da.UpdateNewReportingManager(obj, subordinateDT);
                                }
                                #endregion
                            }
                        }
                    }
                    else if (!isTaskActive)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                    else if (!isTaskNotStarted)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.TaskInProgressErrorMsg;
                    }
                    else
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                }
            }
            catch (Exception ex)
            {
                vm.HasError = true;
                vm.ErrorMessage = ErrorMessage.UnexpectedErrorOccur;
            }
            finally
            {
                WorkflowBL.CloseRunningTask(taskID.ToString(), isTaskNotStarted);
            }

            return vm;
        }

        public static async Task<byte[]> GenerateReceiptPdf(ViewModelApproval vm, bool isITDepartment, string spHostUrl, string accessToken)
        {
            if (isITDepartment)
            {
                return ReceiptPdfHelper.GenerateITDepartReceiptPdf(vm.FinalEmployeeDetails, vm.FinalHardwareItems);
            }
            else
            {
                return ReceiptPdfHelper.GenerateNormalReceiptPdf(vm.FinalEmployeeDetails, vm.FinalHardwareItems, vm.ViewRequestItem.ReferenceNo);
            }
        }

        #endregion

        #region Resubmit
        public async Task<ViewModelRequest> ResubmitELANRequest(ViewModelRequest vm, string spHostURL, string accessToken)
        {
            LogHelper logHelper = new LogHelper();
            RequestDA da = new RequestDA();
            bool isTaskNotStarted = false;
            int processID = vm.ProcessID.GetValueOrDefault();
            int taskID = vm.TaskID.GetValueOrDefault();
            bool ActionSuccess = false;
            string remarks = vm.EmployeeDetails.EmployeeDetailsVM.Remarks;

            try
            {
                TokenHelper.CheckValidAccessToken(accessToken, spHostURL);
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(clientContext);
                    vm.ModifiedBy = currentUser.Title;
                    vm.ModifiedByLogin = currentUser.LoginName;
                    vm.Modified = DateTimeHelper.GetCurrentDateTime();

                    Actioner actioner = new Actioner(currentUser.LoginName, currentUser.Title, currentUser.Email);

                    string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                    bool isTaskActive = wfBL.CheckIsMyTask(taskID, actioner);
                    isTaskNotStarted = WorkflowBL.CheckIsTaskNotStarted(taskID.ToString(), isTaskActive);

                    if (isTaskActive && isTaskNotStarted)
                    {
                        //Save data into db
                        RequestBL requestBL = new RequestBL();
                        vm = requestBL.SaveSubmitELANRequest(vm, clientContext, true, spHostURL, accessToken);

                        if (processID > 0)
                        {
                            ProcessKeywords keywords = wfBL.GetKeywords(processID);
                            keywords.AddKeywordValue(WorkflowKeywords.Common.Remarks, remarks);
                            keywords.AddKeywordValue(WorkflowKeywords.Common.EmployeeName, vm.EmployeeDetails.EmployeeDetailsVM.EmployeeName);
                            keywords.AddKeywordValue(WorkflowKeywords.Common.EmployeeId, vm.EmployeeDetails.EmployeeDetailsVM.EmployeeID);
                            wfBL.UpdateKeywords(processID, keywords);
                        }

                        ActionSuccess = WorkflowBL.CompleteAWorkflowStep(
                            processID.ToString(),
                            taskID.ToString(),
                            remarks,
                            WorkflowActionName.Resubmit,
                            spHostURL,
                            accessToken);

                        vm.HasSuccess = ActionSuccess;
                    }
                    else if (!isTaskActive)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                    else if (!isTaskNotStarted)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.TaskInProgressErrorMsg;
                    }
                    else
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                }
            }
            catch (Exception ex)
            {
                logHelper.LogMessage("ApprovalFormBL - ResubmitELANRequest Error: " + ex.ToString());

                vm.HasError = true;
                vm.ErrorMessage = ErrorMessage.SubmitError;
            }
            finally
            {
                WorkflowBL.CloseRunningTask(taskID.ToString(), isTaskNotStarted);
            }

            return vm;
        }

        //Resubmit SaveDraft
        public async Task<ViewModelRequest> ReSaveRequest(ViewModelRequest vm, string spHostUrl, string accessToken)
        {
            LogHelper logHelper = new LogHelper();
            RequestDA da = new RequestDA();
            int processID = vm.ProcessID.GetValueOrDefault();
            int taskID = vm.TaskID.GetValueOrDefault();

            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(clientContext);

                    #region Modification Info
                    vm.ModifiedBy = currentUser.Title;
                    vm.ModifiedByLogin = currentUser.LoginName;
                    vm.Modified = DateTimeHelper.GetCurrentDateTime();

                    void ApplyModificationInfo(dynamic target)
                    {
                        target.ModifiedBy = vm.ModifiedBy;
                        target.ModifiedByLogin = vm.ModifiedByLogin;
                        target.Modified = vm.Modified;
                    }

                    //EmployeeDetails
                    ApplyModificationInfo(vm.EmployeeDetails.EmployeeDetailsVM);
                    //ITRequirements
                    foreach (var itReq in vm.ITRequirementsVM.SelectedITRequirements)
                    {
                        ApplyModificationInfo(itReq);
                    }
                    //Folder Permissions
                    foreach (var fdReq in vm.ITRequirementsVM.FolderPermissions)
                    {
                        ApplyModificationInfo(fdReq);
                    }
                    //Hardwares
                    foreach (var hwReq in vm.HardwareVM.SelectedHardwareItems)
                    {
                        ApplyModificationInfo(hwReq);
                    }
                    #endregion

                    #region Creation Info For New Added Item

                    void ApplyCreationInfo(dynamic target)
                    {
                        target.CreatedBy = currentUser.Title;
                        target.CreatedByLogin = currentUser.LoginName;
                        target.Created = DateTimeHelper.GetCurrentDateTime();
                    }

                    //ITRequirements
                    foreach (var itReq in vm.ITRequirementsVM.SelectedITRequirements)
                    {
                        ApplyCreationInfo(itReq);
                    }
                    //Folder Permissions
                    foreach (var fdReq in vm.ITRequirementsVM.FolderPermissions)
                    {
                        ApplyCreationInfo(fdReq);
                    }
                    //Hardwares
                    foreach (var hwReq in vm.HardwareVM.SelectedHardwareItems)
                    {
                        ApplyCreationInfo(hwReq);
                    }
                    #endregion

                    Actioner actioner = new Actioner(currentUser.LoginName, currentUser.Title, currentUser.Email);
                    string WFConnString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);
                    bool isTaskActive = wfBL.CheckIsMyTask(taskID, actioner);

                    if (isTaskActive)
                    {
                        bool success = da.UpdateRequest(vm, false);
                        vm.HasSuccess = success;

                        //TODO: Update keywords xml for mypendingtask
                        //var keywords = wfBL.GetKeywords(processID);
                        //string employeeName = keywords.GetKeywordValue(WorkflowKeywords.Common.EmployeeName);
                        //string employeeId = keywords.GetKeywordValue(WorkflowKeywords.Common.EmployeeId);
                    }
                    else if (!isTaskActive)
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = ErrorMessage.NonActiveTask;
                    }
                }
            }
            catch (Exception ex)
            {
                logHelper.LogMessage("ApprovalFormBL - SaveELANRequest Error: " + ex.ToString());

                vm.HasError = true;
                vm.ErrorMessage = ErrorMessage.SaveError;
            }

            return vm;
        }

        #endregion

        #region Termination Request
        public async Task<ViewModelApproval> GetSubordinatesByRepManagerLogin(ViewModelApproval vm)
        {
            try
            {
                RequestDA da = new RequestDA();
                DataTable dt = da.RetrieveFinalEmployeeDetailsByRepManagerLogin(vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin);

                vm.EmployeeDetails.EmployeeDetailsVM.Subordinates = ConvertEntitiesHelper.ConvertViewModelSubordinates(dt) ?? new List<ViewModelSubordinate>();
            }
            catch (Exception ex)
            {
                throw;
            }

            return vm;
        }
        #endregion
    }
}

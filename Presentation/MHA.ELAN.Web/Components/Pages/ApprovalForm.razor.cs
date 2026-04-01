using MHA.ELAN.Business;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;
using static MHA.ELAN.Framework.Constants.ConstantHelper.SPColumn;

namespace MHA.ELAN.Web.Components.Pages
{
    [Authorize]
    public partial class ApprovalForm : ComponentBase
    {
        #region Injection Services
        [Inject] private ApprovalFormBL ApprovalFormBL { get; set; }
        [Inject] private WorkflowBL WorkflowBL { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }
        [Inject] private NavigationHelper NavigationHelper { get; set; }
        [Inject] private TokenHelper TokenHelper { get; set; }
        [Inject] private LogHelper LogHelper { get; set; }
        [Inject] private TempMessageService MessageService { get; set; }
        [Inject] private WorkflowStateService WorkflowStateService { get; set; }
        [Inject] private IJSRuntime JS { get; set; }
        #endregion

        #region Query parameters & route parameters
        [SupplyParameterFromQuery(Name = ConstantHelper.ParameterQuery.SPHostUrl)]
        [Parameter] public string spHostUrl { get; set; }
        [SupplyParameterFromQuery(Name = ConstantHelper.ParameterQuery.RequestId)]
        public string RequestId { get; set; } = string.Empty;
        [SupplyParameterFromQuery(Name = ConstantHelper.ParameterQuery.ProcessId)]
        public string ProcessId { get; set; } = string.Empty;
        [SupplyParameterFromQuery(Name = ConstantHelper.ParameterQuery.TaskId)]
        public string TaskId { get; set; } = string.Empty;
        [Parameter] public string CurrentSection { get; set; } = ConstantHelper.RequestForm.Section.EmployeeDetails;
        #endregion

        #region View models & state
        private string accessToken = string.Empty;
        private ViewModelApproval model = new();
        private RequestFormVisibilitySettings visibilitySettings = new();
        private string? errorMessage;
        private string? requestType;
        private bool IsITDepartment;
        #endregion

        #region Resubmission model & state
        private ViewModelRequest RequestModel = new();
        private bool showErrorAlert = false;
        private CancellationTokenSource? _errorAlertCts;
        #endregion

        #region Init
        protected override async Task OnInitializedAsync()
        {
            accessToken = await TokenHelper.GetUserAccessToken();
            //await LoadData();

            //if (!string.IsNullOrEmpty(model.CurrentStage)) visibilitySettings = await ApprovalFormBL.SetVisibilitySettings(model.CurrentStage);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();

                if (!string.IsNullOrEmpty(model.CurrentStage)) visibilitySettings = await ApprovalFormBL.SetVisibilitySettings(model.CurrentStage);

                model.IsLoaded = true;

                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            try
            {
                await JS.InvokeVoidAsync("showSwalGeneralLoading", "Loading Approval Form", "Please wait while we load the approval form...");

                model = await ApprovalFormBL.InitApprovalForm(model, spHostUrl, accessToken, RequestId, TaskId);

                if (!model.IsMyTask) return;

                WorkflowStateService.WorkflowStatus = model.ViewRequestItem.WorkflowStatus;
                requestType = model.EmployeeDetails.RequestType;

                IsITDepartment = model.HardwareItems.Any(h => h.HardwareVM.DepartmentTitle == ConstantHelper.RequestForm.DepartmentName.ITDepartment);

                StateHasChanged();

                int? processId = model.ViewRequestItem.ProcessID.Value;

                if (processId == null) return;

                if (model.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ElanAdmin))
                {
                    model.PartialModelAdminWorkflowHistory = await WorkflowBL.InitAdminWFHistory(processId.Value, spHostUrl, accessToken);
                }
                else
                {
                    model.PartialModelWorkflowHistory = await WorkflowBL.InitWorkflowHistory(processId.Value, spHostUrl, accessToken);
                }

                if (WorkflowStateService.WorkflowStatus == ConstantHelper.WorkflowStatus.PENDING_ORIGINATOR_RESUBMISSION)
                {
                    if (!string.IsNullOrEmpty(RequestId))
                    {
                        RequestModel.ID = Convert.ToInt32(RequestId);
                    }

                    RequestBL requestBL = new RequestBL();
                    RequestModel = requestBL.InitNewRequestForm(RequestModel, spHostUrl, accessToken);
                    StateHasChanged();
                }

                await JS.InvokeVoidAsync("Swal.close");
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("Swal.close");
                await JS.InvokeVoidAsync("showSwalError", "Failed to load approval form.");

                LogHelper.LogMessage("ApprovalFormBL - InitApprovalForm Error: " + ex.ToString());
            }
        }

        protected override void OnParametersSet()
        {
            EnsureValidSection();
        }

        private async Task OnAdminWorkflowHistoryChanged()
        {
            await LoadData();
            StateHasChanged();
        }
        #endregion

        #region Button handlers
        private async Task HandleRequireAmendment()
        {
            if (string.IsNullOrWhiteSpace(model.EmployeeDetails.EmployeeDetailsVM.Remarks))
            {
                await JS.InvokeVoidAsync("showSwalWarning", "Remarks Required", ConstantHelper.ErrorMessage.RemarksForRequireAmendment);
                return;
            }

            await HandleApprovalAction(ConstantHelper.WorkflowActionName.RequireAmendment);
        }

        private async Task HandleApprove()
        {
            await HandleApprovalAction(ConstantHelper.WorkflowActionName.Approve);
        }

        private async Task HandleReject()
        {
            if (string.IsNullOrWhiteSpace(model.EmployeeDetails.EmployeeDetailsVM.Remarks))
            {
                await JS.InvokeVoidAsync("showSwalWarning", "Remarks Required", ConstantHelper.ErrorMessage.RemarksForReject);
                return;
            }

            await HandleApprovalAction(ConstantHelper.WorkflowActionName.Reject);
        }

        private async Task HandleSave()
        {
            await HandleApprovalAction(ConstantHelper.WorkflowActionName.Save);
        }

        private async Task HandleComplete()
        {
            if (WorkflowStateService.WorkflowStatus == ConstantHelper.WorkflowStatus.PENDING_ACKNOWLEDGEMENT &&
            visibilitySettings.ShowAcknowledgement)
            {
                bool isAcknowledged = false;

                if (IsITDepartment && (model.ViewRequestItem.RequestType != ConstantHelper.RequestForm.RequestType.TerminationRequestType))
                {
                    isAcknowledged = model.EmployeeDetails.EmployeeDetailsVM.Acknowledgement1 &&
                                     model.EmployeeDetails.EmployeeDetailsVM.Acknowledgement2;
                }
                else if (model.ViewRequestItem.RequestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType)
                {
                    isAcknowledged = model.EmployeeDetails.EmployeeDetailsVM.HardwareFinalReturnDate != null &&
                                     model.EmployeeDetails.EmployeeDetailsVM.HardwareReturnConfirmed;
                }
                else
                {
                    isAcknowledged = model.EmployeeDetails.EmployeeDetailsVM.Acknowledgement1;
                }

                if (!isAcknowledged)
                {
                    await JS.InvokeVoidAsync(
                        "showSwalWarning",
                        "Acknowledgement Required",
                        ConstantHelper.ErrorMessage.AcknowledgementReceipt
                    );
                    return;
                }
                await HandleApprovalAction(ConstantHelper.WorkflowActionName.Acknowledge);
            }
            else
            {
                await HandleApprovalAction(ConstantHelper.WorkflowActionName.Complete);
            }
        }

        private async Task HandleApprovalAction(string actionType)
        {
            try
            {
                await JS.InvokeVoidAsync("showSwalLoading");
                await RefreshTokensAsync();

                if (actionType == ConstantHelper.WorkflowActionName.Approve || actionType == ConstantHelper.WorkflowActionName.Complete || actionType == ConstantHelper.WorkflowActionName.Acknowledge)
                {
                    if (!IsReadOnlyWorkflowStatus(model.ViewRequestItem.WorkflowStatus))
                    {
                        if (!ValidateITRequirementsAndFolders(out string validationError))
                        {
                            await JS.InvokeVoidAsync("showSwalValidationWarning", "Validation Error", validationError);
                            return;
                        }
                    }
                    else if (model.ViewRequestItem.WorkflowStatus == ConstantHelper.WorkflowStatus.PENDING_DEPARTMENT_ADMIN_ACTION)
                    {
                        if (!ValidateHardware(out string validationError))
                        {
                            await JS.InvokeVoidAsync("showSwalValidationWarning", "Validation Error", validationError);
                            return;
                        }
                    }
                }

                if (!int.TryParse(RequestId, out int requestId))
                    throw new ArgumentException("Invalid RequestId");

                if (!int.TryParse(ProcessId, out int processId))
                    throw new ArgumentException("Invalid ProcessId");

                if (!int.TryParse(TaskId, out int taskId))
                    throw new ArgumentException("Invalid TaskId");

                model.RequestID = requestId;
                model.ProcessID = processId;
                model.TaskID = taskId;

                switch (actionType)
                {
                    case ConstantHelper.WorkflowActionName.RequireAmendment:
                        model = await ApprovalFormBL.RequireAmendmentRequest(model, spHostUrl, accessToken);
                        break;

                    case ConstantHelper.WorkflowActionName.Approve:
                        model = await ApprovalFormBL.ApproveRequest(model, spHostUrl, accessToken);
                        break;

                    case ConstantHelper.WorkflowActionName.Reject:
                        model = await ApprovalFormBL.RejectRequest(model, spHostUrl, accessToken);
                        break;

                    case ConstantHelper.WorkflowActionName.Save:
                        model = await ApprovalFormBL.SaveRequest(model, spHostUrl, accessToken);
                        break;

                    case ConstantHelper.WorkflowActionName.Complete:
                        model = await ApprovalFormBL.CompleteRequest(model, spHostUrl, accessToken);
                        break;

                    case ConstantHelper.WorkflowActionName.Acknowledge:
                        model = await ApprovalFormBL.AcknowledgeRequest(model, spHostUrl, accessToken);
                        break;

                    default:
                        throw new InvalidOperationException("Unknown action type");
                }

                await JS.InvokeVoidAsync("Swal.close");

                if (model.HasSuccess)
                {
                    string swalTitle = actionType switch
                    {
                        ConstantHelper.WorkflowActionName.RequireAmendment => ConstantHelper.SuccessMessage.RequireAmendmentPopup,
                        ConstantHelper.WorkflowActionName.Approve => ConstantHelper.SuccessMessage.ApprovePopup,
                        ConstantHelper.WorkflowActionName.Reject => ConstantHelper.SuccessMessage.RejectPopup,
                        ConstantHelper.WorkflowActionName.Save => ConstantHelper.SuccessMessage.SavePopup,
                        ConstantHelper.WorkflowActionName.Complete => ConstantHelper.SuccessMessage.CompletePopup,
                        ConstantHelper.WorkflowActionName.Acknowledge => ConstantHelper.SuccessMessage.AcknowledgePopup,
                        _ => "Your request has been saved"
                    };

                    await JS.InvokeVoidAsync("showSwalSuccess", swalTitle);

                    MessageService.SuccessMessage = actionType switch
                    {
                        ConstantHelper.WorkflowActionName.RequireAmendment => ConstantHelper.SuccessMessage.RequireAmendmentSuccess,
                        ConstantHelper.WorkflowActionName.Approve => ConstantHelper.SuccessMessage.ApproveRequestSuccess,
                        ConstantHelper.WorkflowActionName.Reject => ConstantHelper.SuccessMessage.RejectRequestSuccess,
                        ConstantHelper.WorkflowActionName.Save => ConstantHelper.SuccessMessage.SaveRequestSuccess,
                        ConstantHelper.WorkflowActionName.Complete => ConstantHelper.SuccessMessage.CompleteRequestSuccess,
                        ConstantHelper.WorkflowActionName.Acknowledge => ConstantHelper.SuccessMessage.AcknowledgeRequestSuccess,
                        _ => "Action completed successfully"
                    };

                    Navigation.NavigateTo(NavigationHelper.BuildUrl("/Home"));
                }
                else
                {
                    await JS.InvokeVoidAsync("showSwalError", model.ErrorMessage ?? "An unexpected error occurred.");
                    MessageService.ErrorMessage = model.ErrorMessage;
                    errorMessage = model.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogMessage($"ApprovalForm - {actionType} Error: {ex}");
                await JS.InvokeVoidAsync("showSwalError", ConstantHelper.ErrorMessage.ApprovalError);
                MessageService.ErrorMessage = ConstantHelper.ErrorMessage.ApprovalError;
            }
        }

        private void Close()
        {
            Navigation.NavigateTo(NavigationHelper.BuildUrl("/Home"));
        }
        #endregion

        #region General Helpers
        private async Task RefreshTokensAsync()
        {
            accessToken = await TokenHelper.GetUserAccessToken();
        }

        private bool IsReadOnlyWorkflowStatus(string workflowStatus)
        {
            return workflowStatus == ConstantHelper.WorkflowStatus.PENDING_REPORTING_MANAGER_APPROVAL
                || workflowStatus == ConstantHelper.WorkflowStatus.PENDING_DEPARTMENT_ADMIN_ACTION
                || workflowStatus == ConstantHelper.WorkflowStatus.PENDING_ACKNOWLEDGEMENT;
        }
        #endregion

        #region Validation Helpers
        private void EnsureValidSection()
        {
            var allowedSections = new[] {
            ConstantHelper.RequestForm.Section.EmployeeDetails,
            ConstantHelper.RequestForm.Section.ITRequirements,
            ConstantHelper.RequestForm.Section.Hardware,
            ConstantHelper.RequestForm.Section.WorkflowHistory,
            ConstantHelper.RequestForm.Section.Changes };
            if (!allowedSections.Contains(CurrentSection))
            {
                CurrentSection = "WorkflowHistory";
            }
        }

        private bool ValidateITRequirementsAndFolders(out string validationError)
        {
            var errors = new List<string>();

            // Validate Infra IT Requirements
            if (visibilitySettings.ShowInfra)
            {
                foreach (var item in model.ITRequirements.Where(x => x.ITRequirementsVM.Type == ConstantHelper.SPColumnValue.ITRequirements.ItemType.Infra))
                {
                    bool isActionChecked = item.ITRequirementsVM.IsAdded || item.ITRequirementsVM.IsRemoved;

                    if (!isActionChecked)
                    {
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorITRequirements, ConstantHelper.RequestForm.ITRequirementsType.Infra, item.ITRequirementsVM.ItemTitle));
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(item.ITRequirementsVM.Remark))
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorRequireRemarks, ConstantHelper.RequestForm.ITRequirementsType.Infra, item.ITRequirementsVM.ItemTitle));

                    if (item.ITRequirementsVM.IsAdded && item.ITRequirementsVM.DateAdded == null)
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorRequireDateAdded, ConstantHelper.RequestForm.ITRequirementsType.Infra, item.ITRequirementsVM.ItemTitle));

                    if (item.ITRequirementsVM.IsRemoved && item.ITRequirementsVM.DateRemoved == null)
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorRequireDateRemoved, ConstantHelper.RequestForm.ITRequirementsType.Infra, item.ITRequirementsVM.ItemTitle));
                }
            }

            // Validate Application IT Requirements
            if (visibilitySettings.ShowApplications)
            {
                foreach (var item in model.ITRequirements.Where(x => x.ITRequirementsVM.Type == ConstantHelper.SPColumnValue.ITRequirements.ItemType.Applications))
                {
                    bool isActionChecked = item.ITRequirementsVM.IsAdded || item.ITRequirementsVM.IsRemoved;

                    if (!isActionChecked)
                    {
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorITRequirements, ConstantHelper.RequestForm.ITRequirementsType.Applications, item.ITRequirementsVM.ItemTitle));
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(item.ITRequirementsVM.Remark))
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorRequireRemarks, ConstantHelper.RequestForm.ITRequirementsType.Applications, item.ITRequirementsVM.ItemTitle));

                    if (item.ITRequirementsVM.IsAdded && item.ITRequirementsVM.DateAdded == null)
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorRequireDateAdded, ConstantHelper.RequestForm.ITRequirementsType.Applications, item.ITRequirementsVM.ItemTitle));

                    if (item.ITRequirementsVM.IsRemoved && item.ITRequirementsVM.DateRemoved == null)
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorRequireDateRemoved, ConstantHelper.RequestForm.ITRequirementsType.Applications, item.ITRequirementsVM.ItemTitle));
                }
            }

            // Validate Folder Permissions
            if (visibilitySettings.ShowFolderPermission)
            {
                foreach (var folder in model.FolderPermissions)
                {
                    bool isActionChecked = folder.IsAdded || folder.IsRemoved;

                    if (!isActionChecked)
                    {
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorITRequirements, "Folder", folder.NameOrPath));
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(folder.Remark))
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorRequireRemarks, "Folder", folder.NameOrPath));

                    if (folder.IsAdded && folder.DateAdded == null)
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorRequireDateAdded, "Folder", folder.NameOrPath));

                    if (folder.IsRemoved && folder.DateRemoved == null)
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorRequireDateRemoved, "Folder", folder.NameOrPath));
                }
            }

            // Validate Infra Employee Name (People Picker)
            if (WorkflowStateService.WorkflowStatus == ConstantHelper.WorkflowStatus.PENDING_INFRA_TEAM_ACTION &&
                visibilitySettings.ShowEmployeePeoplePicker)
            {
                if (string.IsNullOrWhiteSpace(model.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin))
                {
                    errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorEmployeePeoplePicker, ConstantHelper.RequestForm.ITRequirementsType.Infra));
                }
            }

            if (errors.Any())
            {
                validationError = "<div style='text-align:left'><ul><li>" + string.Join("</li><li>", errors) + "</li></ul></div>";
                return false;
            }

            validationError = string.Empty;
            return true;
        }

        private bool ValidateHardware(out string validationError)
        {
            var errors = new List<string>();

            if (visibilitySettings.ShowHardwareDetails && visibilitySettings.ShowColumnsAsEditable)
            {
                foreach (var hardware in model.HardwareItems)
                {
                    if (hardware.HardwareVM.DateAssigned == null)
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorHardwareRequireFields, "Date Assigned", hardware.HardwareVM.ItemTitle));

                    if (string.IsNullOrWhiteSpace(hardware.HardwareVM.Model))
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorHardwareRequireFields, "Model", hardware.HardwareVM.ItemTitle));

                    if (string.IsNullOrWhiteSpace(hardware.HardwareVM.SerialNumber))
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorHardwareRequireFields, "Serial Number", hardware.HardwareVM.ItemTitle));

                    if (!hardware.HardwareVM.IsReturned && string.IsNullOrWhiteSpace(hardware.HardwareVM.Remarks))
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorHardwareRequireFields, "Remarks", hardware.HardwareVM.ItemTitle));

                    if (hardware.HardwareVM.PendingIsReturned && (hardware.HardwareVM.PendingDateReturned == null))
                        errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorHardwareRequireFields, "Date Returned", hardware.HardwareVM.ItemTitle));

                    if (model.ViewRequestItem.RequestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType)
                    {
                        if (hardware.HardwareVM.IsReturned && (hardware.HardwareVM.DateReturned == null))
                            errors.Add(string.Format(ConstantHelper.ErrorMessage.ValidationErrorHardwareRequireFields, "Date Returned", hardware.HardwareVM.ItemTitle));
                    }
                }
            }

            if (errors.Any())
            {
                validationError = "<div style='text-align:left'><ul><li>" + string.Join("</li><li>", errors) + "</li></ul></div>";
                return false;
            }

            validationError = string.Empty;
            return true;
        }

        #endregion

        #region Resubmission Handling
        private async Task HandleReSave()
        {
            if (!ValidateAndHandleErrors()) return;

            try
            {
                await JS.InvokeVoidAsync("showSwalLoading");
                await RefreshTokensAsync();

                RequestModel.ProcessID = Convert.ToInt32(ProcessId);
                RequestModel.TaskID = Convert.ToInt32(TaskId);

                RequestModel = await ApprovalFormBL.ReSaveRequest(RequestModel, spHostUrl, accessToken);

                await JS.InvokeVoidAsync("Swal.close");
                if (RequestModel.HasSuccess)
                {
                    await JS.InvokeVoidAsync("showSwalSuccess", "Your request has been saved");
                    MessageService.SuccessMessage = ConstantHelper.SuccessMessage.SaveDraftSuccess;
                    Navigation.NavigateTo(NavigationHelper.BuildUrl("/Home"));
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("Swal.close");
                SetError(string.Format(ConstantHelper.ErrorMessage.SaveFailed, ex.Message));
            }
        }

        private async Task HandleReSubmit()
        {
            if (!ValidateAndHandleErrors()) return;

            try
            {
                await JS.InvokeVoidAsync("showSwalLoading");
                await RefreshTokensAsync();

                RequestModel.ReferenceNo = model.ViewRequestItem.ReferenceNo;
                RequestModel.ProcessID = Convert.ToInt32(ProcessId);
                RequestModel.TaskID = Convert.ToInt32(TaskId);

                RequestModel = await ApprovalFormBL.ResubmitELANRequest(RequestModel, spHostUrl, accessToken);

                await JS.InvokeVoidAsync("Swal.close");
                if (RequestModel.HasSuccess)
                {
                    await JS.InvokeVoidAsync("showSwalSuccess", "Your request has been submitted");
                    MessageService.SuccessMessage = ConstantHelper.SuccessMessage.SubmitRequestSuccess;
                    Navigation.NavigateTo(NavigationHelper.BuildUrl("/Home"));
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("Swal.close");
                SetError(string.Format(ConstantHelper.ErrorMessage.SubmitFailed, ex.Message));
            }
        }

        private void OnEmployeeDetailsChanged(ViewModelEmployeeDetails updatedVM)
        {
            RequestModel.EmployeeDetails = updatedVM;

            RequestModel.ITRequirementsVM.Designation =
                string.IsNullOrEmpty(updatedVM.EmployeeDetailsVM.DesignationTitle)
                ? string.Empty
                : updatedVM.EmployeeDetailsVM.DesignationTitle;

            StateHasChanged();
        }

        private bool ValidateForm()
        {
            var validationContext = new ValidationContext(RequestModel.EmployeeDetails);
            var validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(RequestModel.EmployeeDetails.EmployeeDetailsVM, validationContext, validationResults, true);
        }

        private bool ValidateAndHandleErrors()
        {
            var errorMessages = new List<string>();

            // EmployeeDetails required fields
            if (!ValidateForm())
            {
                var missingFieldsMessage = RequestModel.EmployeeDetails.GetMissingRequiredFieldsMessage(RequestModel);
                if (!string.IsNullOrEmpty(missingFieldsMessage))
                {
                    errorMessages.Add(missingFieldsMessage);
                }
            }

            // IT Requirements validation
            var itErrors = GetITRequirementsValidationErrors();
            if (!string.IsNullOrEmpty(itErrors))
            {
                errorMessages.Add(itErrors);
            }

            // Hardware validation
            var hardwareErrors = GetHardwareValidationErrors();
            if (!string.IsNullOrEmpty(hardwareErrors))
            {
                errorMessages.Add(hardwareErrors);
            }

            if (errorMessages.Any())
            {
                _errorAlertCts?.Cancel();

                RequestModel.EmployeeDetails.HasError = true;
                RequestModel.EmployeeDetails.ErrorMessage = string.Join("<br/><br/>", errorMessages);
                showErrorAlert = true;

                _errorAlertCts = new CancellationTokenSource();
                var token = _errorAlertCts.Token;

                _ = AutoDismissErrorAsync(token);
                StateHasChanged();
                return false;
            }

            RequestModel.EmployeeDetails.HasError = false;
            RequestModel.EmployeeDetails.ErrorMessage = string.Empty;
            return true;
        }

        private string GetITRequirementsValidationErrors()
        {
            var errors = new List<string>();

            // Check at least one Infra selected
            bool hasInfra = RequestModel.ITRequirementsVM.InfraOptions
                .Where(opt => opt.DesignationID == null || opt.DesignationID == RequestModel.EmployeeDetails.EmployeeDetailsVM.DesignationID)
                .Any(opt => opt.IsSelected);

            if (!hasInfra)
                errors.Add(string.Format(ConstantHelper.ErrorMessage.CheckboxError, ConstantHelper.RequestForm.ITRequirementsType.Infra, ConstantHelper.RequestForm.Tab.ITRequirements));

            // Check at least one Application selected
            bool hasApp = RequestModel.ITRequirementsVM.ApplicationOptions
                .Where(opt => opt.DesignationID == null || opt.DesignationID == RequestModel.EmployeeDetails.EmployeeDetailsVM.DesignationID)
                .Any(opt => opt.IsSelected);

            if (!hasApp)
                errors.Add(string.Format(ConstantHelper.ErrorMessage.CheckboxError, ConstantHelper.RequestForm.ITRequirementsType.Applications, ConstantHelper.RequestForm.Tab.ITRequirements));

            // Check Folder Permissions: NameOrPath required
            for (int i = 0; i < RequestModel.ITRequirementsVM.FolderPermissions.Count; i++)
            {
                var fp = RequestModel.ITRequirementsVM.FolderPermissions[i];
                if (string.IsNullOrWhiteSpace(fp.NameOrPath))
                    errors.Add(string.Format(ConstantHelper.ErrorMessage.FolderPath, i + 1));
            }

            return errors.Any()
            ? string.Format(ConstantHelper.ErrorMessage.TabError, ConstantHelper.RequestForm.Tab.ITRequirements) +
              string.Join("<br/>", errors.Select(e => $"• {e}"))
            : string.Empty;
        }

        private string GetHardwareValidationErrors()
        {
            var errors = new List<string>();

            for (int i = 0; i < RequestModel.HardwareVM.AssignedItems.Count; i++)
            {
                var hw = RequestModel.HardwareVM.AssignedItems[i];
                var rowNum = i + 1;

                if (hw.DesignationID != RequestModel.EmployeeDetails.EmployeeDetailsVM.DesignationID)
                    errors.Add(string.Format(ConstantHelper.ErrorMessage.NotValidDesignation, rowNum));

                if (!hw.ItemID.HasValue)
                    errors.Add(string.Format(ConstantHelper.ErrorMessage.ItemIsRequired, rowNum));

                if (hw.Quantity <= 0)
                    errors.Add(string.Format(ConstantHelper.ErrorMessage.MinQuantity, rowNum));
            }

            return errors.Any()
            ? string.Format(ConstantHelper.ErrorMessage.TabError, ConstantHelper.RequestForm.Tab.Hardware) +
              string.Join("<br/>", errors.Select(e => $"• {e}"))
            : string.Empty;
        }

        private void SetError(string message)
        {
            _errorAlertCts?.Cancel();

            RequestModel.EmployeeDetails.HasError = true;
            RequestModel.EmployeeDetails.ErrorMessage = message;
            showErrorAlert = true;

            _errorAlertCts = new CancellationTokenSource();
            var token = _errorAlertCts.Token;

            _ = AutoDismissErrorAsync(token);
            StateHasChanged();
        }

        private async Task AutoDismissErrorAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(10000, token);
                showErrorAlert = false;
                RequestModel.EmployeeDetails.HasError = false;
                StateHasChanged();
            }
            catch (TaskCanceledException)
            {
            }
        }

        private void DismissErrorAlert()
        {
            showErrorAlert = false;
            RequestModel.EmployeeDetails.HasError = false;

            _errorAlertCts?.Cancel();
        }
        #endregion

        #region Modification Request
        private bool IsModificationRequest =>
        RequestModel != null && UpdateTypeRequests.Contains(model.ViewRequestItem.RequestType);

        private static readonly HashSet<string> UpdateTypeRequests = new()
        {
            ConstantHelper.RequestForm.RequestType.ModificationRequestType,
            ConstantHelper.RequestForm.RequestType.TransferRequestType,
            ConstantHelper.RequestForm.RequestType.PromotionRequestType
        };
        #endregion

        #region Termination Request
        private bool IsTerminationRequest =>
            model != null &&
            model.ViewRequestItem.RequestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType;
        #endregion
    }
}

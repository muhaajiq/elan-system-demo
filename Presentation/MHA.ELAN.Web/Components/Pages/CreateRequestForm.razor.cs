using MHA.ELAN.Business;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Graph;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;

namespace MHA.ELAN.Web.Components.Pages
{
    [Authorize]
    public partial class CreateRequestForm : ComponentBase
    {
        #region Injection Services
        [Inject] private NavigationManager Navigation { get; set; }
        [Inject] private NavigationHelper NavigationHelper { get; set; }
        [Inject] private TokenHelper TokenHelper { get; set; }
        [Inject] private RequestBL RequestBL { get; set; }
        [Inject] private LogHelper logHelper { get; set; }
        [Inject] private TempMessageService MessageService { get; set; }
        [Inject] private IJSRuntime JS { get; set; }
        #endregion

        #region Query parameters & route parameters
        [SupplyParameterFromQuery(Name = ConstantHelper.ParameterQuery.SPHostUrl)]
        [Parameter] public string spHostUrl { get; set; } = string.Empty;
        [SupplyParameterFromQuery(Name = ConstantHelper.ParameterQuery.RequestId)]
        public string RequestId { get; set; } = string.Empty;
        [Parameter]
        public string CurrentSection { get; set; } = "EmployeeDetails";
        #endregion

        #region View models & state
        public string accessToken = string.Empty;
        public string appAccessToken = string.Empty;
        public ViewModelRequest RequestModel { get; set; } = new();
        public ViewModelRequest OriginalRequestModel { get; set; } = new();
        private bool showErrorAlert = false;
        private CancellationTokenSource _errorAlertCts;
        #endregion

        #region Init
        protected override async Task OnInitializedAsync()
        {
            accessToken = await TokenHelper.GetUserAccessToken();
            appAccessToken = await TokenHelper.GetAccessTokenFromHybridApp(spHostUrl);
            //await LoadData();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadData();
            }

            if (showErrorAlert && RequestModel.EmployeeDetails.HasError)
            {
                await JS.InvokeVoidAsync("jsInterop.scrollToError", "errorAlertTop");
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            try
            {
                await JS.InvokeVoidAsync("showSwalGeneralLoading", "Loading Request Form", "Please wait while we load the request form...");

                if (!string.IsNullOrEmpty(RequestId))
                {
                    RequestModel.ID = Convert.ToInt32(RequestId);
                }

                RequestModel = RequestBL.InitNewRequestForm(RequestModel, spHostUrl, accessToken);
                OriginalRequestModel = CloneObject(RequestModel);

                StateHasChanged();

                await JS.InvokeVoidAsync("Swal.close");
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("Swal.close");
                await JS.InvokeVoidAsync("showSwalError", "Failed to load approval form.");

                logHelper.LogMessage("EmployeeDetailsTab - InitNewRequestForm: " + ex.ToString());
                RequestModel.HasError = true;
                RequestModel.ErrorMessage = string.Format(ConstantHelper.ErrorMessage.UnexpectedErrorOccur, ex.Message);
                StateHasChanged();
            }
        }

        protected override void OnParametersSet()
        {
            EnsureValidSection();
        }

        private async Task LoadEmployeeDetailsByLogin(string employeeLogin)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(employeeLogin))
                {
                    ResetAllSections("Employee login is empty. Please select an employee to continue.");
                    return;
                }

                await JS.InvokeVoidAsync("showSwalLoading", "Loading Employee", ConstantHelper.InfoMessage.LoadEmployeeDataLoading);
                RequestModel = await RequestBL.InitExistingRequestFormByLogin(RequestModel, spHostUrl, accessToken);
                OriginalRequestModel = CloneObject(RequestModel);

                if (RequestModel.EmployeeDetails.HasError)
                {
                    ResetAllSections(RequestModel.EmployeeDetails.ErrorMessage);
                }

                await JS.InvokeVoidAsync("Swal.close");
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("Swal.close");
                logHelper.LogMessage("RequestBL - InitExistingRequestFormByLogin Error: " + ex.ToString());
                ResetAllSections(ConstantHelper.ErrorMessage.LoadEmployeeFailed);
                StateHasChanged();
            }
        }
        #endregion

        #region Button handlers
        private async Task HandleSave()
        {
            if (!ValidateAndHandleErrors()) return;

            try
            {
                await JS.InvokeVoidAsync("showSwalLoading");
                await RefreshTokensAsync();
                var changeLog = RequestBL.CompareAndCreateOrUpdateChangesLog(OriginalRequestModel, RequestModel);
                RequestModel.Changes = changeLog;

                RequestModel = await RequestBL.SaveELANRequest(RequestModel, spHostUrl, accessToken, appAccessToken);

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

        private async Task HandleSubmit()
        {
            if (!ValidateAndHandleErrors()) return;

            try
            {
                await JS.InvokeVoidAsync("showSwalLoading");
                await RefreshTokensAsync();
                var changeLog = RequestBL.CompareAndCreateOrUpdateChangesLog(OriginalRequestModel, RequestModel);
                RequestModel.Changes = changeLog;
                RequestModel = await RequestBL.SubmitELANRequest(RequestModel, spHostUrl, accessToken, appAccessToken);

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

        private async Task OnRequestTypeChanged(string newType)
        {
            RequestModel.RequestType = newType;
            CurrentSection = ConstantHelper.RequestForm.Section.EmployeeDetails;

            if (!UpdateTypeRequests.Contains(RequestModel.RequestType))
            {
                RequestModel.ModifyEmployeeDetails = false;
                RequestModel.ModifyITRequirements = false;
                RequestModel.ModifyHardware = false;

                // If user switches back to "New Request", reload defaults
                if (RequestModel.RequestType == ConstantHelper.RequestForm.RequestType.NewRequestType)
                {
                    await LoadData();
                }
            }

            StateHasChanged();
        }
        #endregion

        #region Validation Helpers
        private void EnsureValidSection()
        {
            var allowedSections = new[] {
            ConstantHelper.RequestForm.Section.EmployeeDetails,
            ConstantHelper.RequestForm.Section.ITRequirements,
            ConstantHelper.RequestForm.Section.Hardware };
            if (!allowedSections.Contains(CurrentSection))
            {
                CurrentSection = ConstantHelper.RequestForm.Section.EmployeeDetails;
            }
        }

        private async Task RefreshTokensAsync()
        {
            accessToken = await TokenHelper.GetUserAccessToken();
            appAccessToken = await TokenHelper.GetAccessTokenFromHybridApp(spHostUrl);
        }

        private bool ValidateAndHandleErrors()
        {
            var errorMessages = new List<string>();

            // EmployeeDetails required fields
            var missingFieldsMessage = RequestModel.EmployeeDetails.GetMissingRequiredFieldsMessage(RequestModel);
            if (!string.IsNullOrEmpty(missingFieldsMessage))
            {
                errorMessages.Add(missingFieldsMessage);
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

            if (RequestModel.RequestType?.Equals(ConstantHelper.RequestForm.RequestType.ModificationRequestType, StringComparison.OrdinalIgnoreCase) == true && !RequestModel.ModifyEmployeeDetails)
            {
                var missingFields = new List<string>();

                if (RequestModel.EmployeeDetails.EmployeeDetailsVM.CompanyID <= 0)
                    missingFields.Add("Company");

                if (RequestModel.EmployeeDetails.EmployeeDetailsVM.Employee == null ||
                    string.IsNullOrWhiteSpace(RequestModel.EmployeeDetails.EmployeeDetailsVM.Employee.Login))
                    missingFields.Add("Employee Name");

                if (missingFields.Count > 0)
                {
                    errorMessages.Add("Please fill in the following required fields before saving:<br /><br />" +
                    string.Join("<br />", missingFields.Select(f => $"• {f}")));
                }
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

        //private bool ValidateForm()
        //{
        //    var validationContext = new ValidationContext(RequestModel.EmployeeDetails.EmployeeDetailsVM);
        //    var validationResults = new List<ValidationResult>();
        //    return Validator.TryValidateObject(RequestModel.EmployeeDetails.EmployeeDetailsVM, validationContext, validationResults, true);
        //}

        private string GetITRequirementsValidationErrors()
        {
            if (RequestModel.RequestType?.Equals(ConstantHelper.RequestForm.RequestType.TerminationRequestType, StringComparison.OrdinalIgnoreCase) == true)
            {
                return string.Empty;
            }

            if (RequestModel.RequestType?.Equals(ConstantHelper.RequestForm.RequestType.ModificationRequestType, StringComparison.OrdinalIgnoreCase) == true && !RequestModel.ModifyITRequirements)
            {
                return string.Empty;
            }

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
            if (RequestModel.RequestType?.Equals(ConstantHelper.RequestForm.RequestType.TerminationRequestType, StringComparison.OrdinalIgnoreCase) == true)
            {
                return string.Empty;
            }

            if (RequestModel.RequestType?.Equals(ConstantHelper.RequestForm.RequestType.ModificationRequestType, StringComparison.OrdinalIgnoreCase) == true && !RequestModel.ModifyHardware)
            {
                return string.Empty;
            }

            var errors = new List<string>();
            var assignedItems = RequestModel.HardwareVM.AssignedItems
                        .Where(hw => !hw.IsTempRemoved)
                        .ToList();

            for (int i = 0; i < assignedItems.Count; i++)
            {
                var hw = assignedItems[i];
                var rowNum = i + 1;

                if (hw.DesignationID != RequestModel.EmployeeDetails.EmployeeDetailsVM.DesignationID)
                    errors.Add(string.Format(ConstantHelper.ErrorMessage.NotValidDesignation, rowNum));

                if (!hw.ItemID.HasValue)
                    errors.Add(string.Format(ConstantHelper.ErrorMessage.ItemIsRequired, rowNum));

                if (hw.Quantity <= 0)
                    errors.Add(string.Format(ConstantHelper.ErrorMessage.MinQuantity, rowNum));
            }

            // Check for duplicates
            var duplicateGroups = assignedItems
                .Where(hw => hw.ItemID.HasValue)
                .GroupBy(hw => hw.ItemID.Value)
                .Where(g => g.Count() > 1)
                .ToList();

            foreach (var dup in duplicateGroups)
            {
                var itemName = RequestModel.HardwareVM.HardwareDefinitions
                                .FirstOrDefault(hd => hd.ItemID == dup.Key)?.ItemTitle ?? $"Item ID {dup.Key}";

                var dupRows = assignedItems
                    .Select((hw, idx) => new { hw, idx })
                    .Where(x => x.hw.ItemID == dup.Key)
                    .Select(x => x.idx + 1)
                    .ToList();

                errors.Add(string.Format(ConstantHelper.ErrorMessage.DuplicateHardwareItem, itemName, string.Join(", ", dupRows)));
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
        #endregion

        #region General Helpers
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

        private void ResetAllSections(string message = "")
        {
            RequestModel.EmployeeDetails = new ViewModelEmployeeDetails
            {
                EmployeeDetailsVM = new EmployeeDetails(),
                HasError = !string.IsNullOrEmpty(message),
                ErrorMessage = message
            };

            RequestModel.ITRequirementsVM = new ViewModelITRequirements
            {
                InfraOptions = new List<ITOption>(),
                ApplicationOptions = new List<ITOption>(),
                FolderPermissions = new List<FolderPermission>()
            };

            RequestModel.HardwareVM = new ViewModelHardware
            {
                HardwareDefinitions = new List<HardwareDefinition>(),
                HardwareItemList = new List<DropDownListItem>(),
                AssignedItems = new List<HardwareItemVM>()
            };

            showErrorAlert = !string.IsNullOrEmpty(message);
            StateHasChanged();
        }

        private T CloneObject<T>(T source)
        {
            var serialized = System.Text.Json.JsonSerializer.Serialize(source);
            return System.Text.Json.JsonSerializer.Deserialize<T>(serialized);
        }

        private static readonly HashSet<string> UpdateTypeRequests = new()
        {
            ConstantHelper.RequestForm.RequestType.ModificationRequestType,
            ConstantHelper.RequestForm.RequestType.TransferRequestType,
            ConstantHelper.RequestForm.RequestType.PromotionRequestType
        };

        private string PageHeaderTitle
        {
            get
            {
                if (UpdateTypeRequests.Contains(RequestModel.RequestType))
                {
                    return $"{RequestModel.RequestType.Replace("RequestType", string.Empty)} Form";
                }
                else if (RequestModel.RequestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType)
                {
                    return "Termination Request Form";
                }
                else
                {
                    return "New Request Form";
                }
            }
        }
        #endregion

        #region Modification Request
        private bool IsModificationRequest =>
            RequestModel != null && UpdateTypeRequests.Contains(RequestModel.RequestType);
        #endregion

        #region Termination Request
        private bool IsTerminationRequest =>
            RequestModel != null &&
            RequestModel.RequestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType;
        #endregion
    }
}

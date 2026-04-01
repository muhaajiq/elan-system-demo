using Microsoft.Graph;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Framework.Constants
{
    public partial class ConstantHelper
    {
        #region Delimit
        public class Delimit
        {
            public static char[] SemiColonDelimit = { ';' };
        }

        #endregion

        #region Email
        public class Email
        {
            public class ExceptionMessage
            {
                public const string ConnectedPartyNotProperlyRespond = "A connection attempt failed because the connected party did not properly respond after a period of time";
                public const string OperationTimedOut = "The operation has timed out";
                public const string MailboxServerIsTooBusy = "mailbox server is too busy";
                public const string MapiExceptionRpcServerTooBusy = "MapiExceptionRpcServerTooBusy";
                public const string SenderThreadLimitExceeded = "sender thread limit exceeded";
                public const string ConcurrentConnectionLimitExceed = "Concurrent connections limit exceeded";
                public const string TemporaryServerError = "Temporary server error";
                public const string ConnectionForcibltClosed = "An existing connection was forcibly closed by the remote host";
                public const string UnableReadData = "Unable to read data from the transport connection";

                public const string SubmissionQuotaExceededException = "SubmissionQuotaExceededException";
                public const string SenderSubmissionExceeded = "sender's submission quota was exceeded";
            }

        }
        public class EmailTemplateKeyTitle
        {
            public const string WorkflowTaskReassignedNotification = "Workflow Task Reassigned Notification";
            public const string WorkflowTaskDelegatedNotification = "Workflow Task Delegated Notification";

            public const string EmployeeContractExpirationReminder = "Employee Contract Expiration Reminder";
        }
        #endregion

        #region Execption
        public class ExceptionType
        {
            public const string SaveConflict = "save conflict";
            public const string VersionConflict = "version conflict";
            public const string OperationTimedOut = "the operation has timed out";
            public const string HResult = "hresult: 0x80131904";
            public const string UnderlineClosed = "underlying connection was closed";
        }
        #endregion

        #region General
        public class DateFormat
        {
            public const string JoinedDateFormat = "yyyy-MM-dd";
            public const string DefaultDateFormat = "dd-MMM-yyyy";
            public const string GridDateFormat = "{0:dd-MM-yyyy}";
            public const string GridDateFormat2 = "{0:dd-MMM-yyyy}";
        }

        public class ItemStatus
        {
            public const string Active = "Active";
            public const string Inactive = "Inactive";
        }

        public class Sorting
        {
            public const string GridAscending = "Ascending";
            public const string GridDescending = "Descending";

            public const string SQLAscending = "ASC";
            public const string SQLDescending = "DESC";
        }

        public class ParameterQuery
        {
            public const string SPHostUrl = "SPHostUrl";
            public const string RequestId = "RequestId";
            public const string ProcessId = "ProcessId";
            public const string TaskId = "TaskId";
            public const string EmployeeId = "EmployeeId";
        }

        public class TableColumnWidth
        {
            public const string General = "200px";

            public class ITRequirements
            {
                public const string Item = "300px";
                public const string Remarks = "400px";
            }
        }
        #endregion

        #region Limits and row counts
        public static int MyPagingSelectionShowLimit = 7;
        #endregion

        #region Message
        public class ErrorMessage
        {
            public const string NoAuthorized = "Sorry, You do not have permission to {0}.";
            public const string UnexpectedErrorOccur = "An unexpected error has occurred. Please try again or contact responsible person if the issue persists.";

            //New Request Form
            public const string SaveError = "We couldn't save your request. Please try again or contact responsible person if the issue persists.";
            public const string SubmitError = "We couldn't submit your request. Please try again or contact responsible person if the issue persists.";
            public const string SaveFailed = "Failed to save: {0}";
            public const string SubmitFailed = "Failed to submit: {0}";
            public const string NoSelectedDesignation = "Please select a Designation from Employee Details.";
            public const string AllItemsAdded = "All active hardware items have already been added.";
            public const string LoadEmployeeFailed = "Failed to load employee details.";

            //Fields Validation
            public const string NotValidDesignation = "Row {0}: not valid for the selected designation.";
            public const string ItemIsRequired = "Row {0}: Item is required.";
            public const string MinQuantity = "Row {0}: Quantity must be at least 1.";
            public const string TabError = "Please fix the following in {0} tab before saving:<br /><br />";
            public const string CheckboxError = "Please select at least one {0} item in the {1} tab.";
            public const string FolderPath = "Folder Permissions Row {0}: Folder Name/Path is required.";
            public const string RemarksForReject = "Please provide remarks before rejecting the request.";
            public const string RemarksForRequireAmendment = "Please provide remarks before requesting amendment.";
            public const string AcknowledgementReceipt = "Please tick the checkbox(es) to confirm you have acknowledged the receipt and terms.";
            public const string DuplicateHardwareItem = "Duplicate item '{0}' selected in rows: {1}";

            //Workflow
            public const string MissingWorkflowMatrix = "System encounter error when getting {0} user. Please try again later.";
            public const string MissingWFDueDate = "System encounter error when getting {0} due days. Please try again later.";

            //Approval Form
            public const string NonActiveTask = "This task is no longer available. It's either you have completed the task or the task has been removed.";
            public const string ApprovalError = "An error occurred during approval. Please try again later.";
            public const string TaskInProgressErrorMsg = "This task is currently being completed in another tab/browser or by another actioner. Your current changes cannot be saved. Please refresh the page to see the latest version";

            //ITRequirements
            public const string ValidationErrorITRequirements = "{0}: {1} must have either 'Added' or 'Removed' selected.";
            public const string ValidationErrorRequireRemarks = "{0}: {1} requires a remark.";
            public const string ValidationErrorRequireDateAdded = "{0}: {1} requires a 'Date Added'.";
            public const string ValidationErrorRequireDateRemoved = "{0}: {1} requires a 'Date Removed'.";
            public const string ValidationErrorEmployeePeoplePicker = "{0}: Employee Name is required.";

            //Hardware
            public const string ValidationErrorHardwareRequireFields = "Please provide a {0} for hardware item '{1}'.";
        }

        public class SuccessMessage
        {
            public const string SaveDraftSuccess = "Saved successfully and currently saved as draft.";
            public const string SubmitRequestSuccess = "Submitted successfully and start workflow.";
            public const string ApproveRequestSuccess = "The task has been approved successfully.";
            public const string RejectRequestSuccess = "The task has been rejected successfully.";
            public const string RequireAmendmentSuccess = "The task is required for amendment.";
            public const string SaveRequestSuccess = "The task has been saved successfully.";
            public const string CompleteRequestSuccess = "The task has been completed successfully.";
            public const string AcknowledgeRequestSuccess = "Acknowledged successfully.";

            //Popup
            public const string RequireAmendmentPopup = "Amendment required successfully.";
            public const string ApprovePopup = "Request approved successfully.";
            public const string RejectPopup = "Request rejected successfully.";
            public const string SavePopup = "Request saved successfully.";
            public const string CompletePopup = "Request compeleted successfully.";
            public const string AcknowledgePopup = "Request acknowledged successfully.";
        }

        public class InfoMessage
        {
            public const string EmailSentStatusInfo = "{0} {1} sent out at {2}.";
            public const string RejectRemarks = "Rejected reason: {0}";
            public const string CompareHardwareChangesRemoved = "Hardware: '{0}' removed (Qty: {1}, Remarks: '{2}')";
            public const string CompareHardwareChangesAdded = "Hardware: '{0}' added (Qty: {1}, Remarks: '{2}')";
            public const string CompareHardwareChangesQuantity = "Hardware: '{0}' quantity changed from '{1}' to '{2}'";
            public const string CompareHardwareChangesRemarks = "Hardware: '{0}' remarks changed from '{1}' to '{2}'";
            public const string CompareITChangesRemoved = "{0} option '{1}' has been removed";
            public const string CompareITChangesAdded = "{0} option '{1}' has been added";
            public const string CompareFolderChangesRemoved = "Folder '{0}' has been removed";
            public const string CompareFolderChangesAdded = "Folder '{0}' has been added";
            public const string CompareFolderChangesPermissionRead = "Folder '{0}' read permission changed from '{1}' to '{2}'";
            public const string CompareFolderChangesPermissionWrite = "Folder '{0}' write permission changed from '{1}' to '{2}'";
            public const string CompareFolderChangesPermissionDelete = "Folder '{0}' delete permission changed from '{1}' to '{2}'";
            public const string LoadEmployeeDataLoading = "Please wait while we load the selected employee data...";
            public const string LoadWait = "Please wait...";
        }
        #endregion

        #region Module
        public class Module
        {
            public const string ELAN = "ELAN";
        }
        #endregion

        #region Running Number Format
        public class RunningNumberFormatType
        {
            public const string ELANRequestReferenceNumber = "ELAN Request Reference Number";
        }

        public class RunningNumberFormatInstance
        {
            public const string RunningNo = "{RunningNo}";
            public const string RequestYear = "{RequestYear}";
        }
        #endregion

        #region Request Form

        public class RequestForm
        {
            public class RequestType
            {
                public const string NewRequestType = "New Request";
                public const string ModificationRequestType = "Modification Request";
                public const string TerminationRequestType = "Termination Request";
                public const string TransferRequestType = "Transfer Request";
                public const string PromotionRequestType = "Promotion Request";
            }

            public class ContractType
            {
                public const string Temporary = "Contract / Temporary Staff";
                public const string Permanent = "Permanent Staff";
            }

            public class WorkflowNewRequest
            {
                public const string ReferenceNoEmpty = "Not Available";
                public const string WorkflowStatusEmpty = "Not Available";
                public const string ReferenceNoDraft = "-";
                public const string WorkflowStatusDraft = "Draft";
            }

            public class ITRequirementsType
            {
                public const string Infra = "Infra";
                public const string Applications = "Applications";
            }

            public class Tab
            {
                public const string EmployeeDetails = "Employee Details";
                public const string ITRequirements = "IT Requirements";
                public const string Hardware = "Hardware";
            }

            public class Section
            {
                public const string EmployeeDetails = "EmployeeDetails";
                public const string ITRequirements = "ITRequirements";
                public const string Hardware = "Hardware";
                public const string WorkflowHistory = "WorkflowHistory";
                public const string Changes = "Changes";
            }

            public class DepartmentName
            {
                public const string ITDepartment = "IT Department";
            }
        }

        #endregion

        #region SharePoint

        #region URL Template
        public class URLTemplate
        {
            public const string SiteContents = "{0}/_layouts/15/viewlsts.aspx?view=14";
            public const string SiteSettings = "{0}/_layouts/15/settings.aspx";
            public const string ReportURLTemplate = "{0}/Lists/Report%20URL";
            public const string RunningNumberFormatUrlTemplate = "{0}/Lists/RunningNumberFormat";
            public const string RunningNumberUrlTemplate = "{0}/Lists/RunningNumber";
            public const string CompanyUrlTemplate = "{0}/Lists/Company";
            public const string LocationUrlTemplate = "{0}/Lists/Location";
            public const string GradeUrlTemplate = "{0}/Lists/Grade";
            public const string DepartmentUrlTemplate = "{0}/Lists/Department";
            public const string DesignationUrlTemplate = "{0}/Lists/Designation";
            public const string ITRequirementsUrlTemplate = "{0}/Lists/ITRequirements";
            public const string HardwareUrlTemplate = "{0}/Lists/Hardware";

            public const string HomePageUrlTemplate = "{0}/Home?SPHostUrl={1}";
            public const string EmployeeDisplayFormUrlTemplate = "{0}/ViewEmployeeForm?EmployeeId={1}&SPHostUrl={2}";
            public const string RequestDisplayFormUrlTemplate = "{0}/ViewRequestForm?RequestId={1}&SPHostUrl={2}";
            public const string NewDelegationUrlTemplate = "{0}/NewDelegate?SPHostUrl={1}";
            public const string RemoveDelegationUrlTemplate = "{0}/RemoveDelegate?SPHostUrl={1}";
        }
        #endregion

        #endregion

        #region SharePoint List
        public class SPList
        {
            //Maintenance list
            public const string ReportUrl = "Report URL";
            public const string Company = "Company";
            public const string Designation = "Designation";
            public const string Grade = "Grade";
            public const string Department = "Department";
            public const string Location = "Location";
            public const string ITRequirements = "ITRequirements";
            public const string Hardware = "Hardware";
            public const string Hardware_Designation = "Hardware_Designation";
            public const string DueDateForWorkflow = "Due Date For Workflow";

            public const string EmailTemplate = "Email Templates";

            //Running Number
            public const string RunningNumber = "Running Number";
            public const string RunningNumberFormat = "Running Number Format";
        }
        #endregion

        #region SharePoint Column
        public class SPColumn
        {
            public const string CreatedBy = "Author";
            public const string ModifiedBy = "Editor";
            public const string Created = "Created";
            public const string Modified = "Modified";
            public const string Status = "Status";
            public const string Title = "Title";

            public class ReportURL
            {
                public const string Title = "Title";
                public const string URL = "URL";
            }

            #region Running Number Setting
            public class RunningNumberFormat
            {
                public const string ID = "ID";
                public const string Title = "Title";
                public const string Format = "Format";
                public const string Prefix = "Prefix";
                public const string Autonumber = "Autonumber";
            }

            public class RunningNumber
            {
                public const string ID = "ID";
                public const string Title = "Title";
                public const string Format = "Format";
                public const string Prefix = "Prefix";
                public const string Number = "Number";
            }
            #endregion

            //Company
            public class Company
            {
                public const string Title = "Title";
            }

            //Designation
            public class Designation
            {
                public const string ID = "ID";
                public const string Title = "Title";
                public const string Status = "Status";
            }

            //Grade
            public class Grade
            {
                public const string Title = "Title";
            }

            //Department
            public class Department
            {
                public const string ID = "ID";
                public const string Title = "Title";
                public const string Admin = "Admin";
                public const string Status = "Status";
            }

            //Location
            public class Location
            {
                public const string Title = "Title";
            }


            //ITRequirements
            public class ITRequirements
            {
                public const string Title = "Title";
                public const string ItemType = "ItemType";
                public const string Designation = "Designation";
                public const string Sequence = "Sequence";
            }

            //Hardware
            public class Hardware
            {
                public const string Title = "Title";
                public const string Department = "Department";
                public const string Status = "Status";
                public const string Sequence = "Sequence";
                public const string Designation = "Designation";
            }


            public class Hardware_Designation
            {
                public const string Title = "Title";
                public const string Designation = "Designation";
                public const string Item = "Item";
                public const string Sequence = "Sequence";
            }

            //Workflow Due Date
            public class DueDateForWorkflow
            {
                public const string Title = "Title";
                public const string DurationDays = "DurationDays";
            }

            public class EmailTemplates
            {
                public const string Title = "Title";
                public const string Subject = "Subject";
                public const string Body = "Body";
                public const string ProcessName = "ProcessName";
                public const string InternalStepName = "InternalStepName";
                //public const string ClientOrThirdParty = "ClientOrThirdParty";
                //public const string CorrespondenceType = "CorrespondenceType";
            }
        }
        #endregion

        #region SharePoint Column Value
        public class SPColumnValue
        {
            public class Department
            {
                public class Title
                {
                    public const string HRDepartment = "HR Department";
                }
                public class Status
                {
                    public const string Active = "Active";
                    public const string Inactive = "Inactive";
                }
            }

            public class Designation
            {
                public class Status
                {
                    public const string Active = "Active";
                    public const string Inactive = "Inactive";
                }
            }

            public class Grade
            {
                public class Status
                {
                    public const string Active = "Active";
                    public const string Inactive = "Inactive";
                }
            }

            public class Location
            {
                public class Status
                {
                    public const string Active = "Active";
                    public const string Inactive = "Inactive";
                }
            }

            public class DueDateDaysForWorkflow
            {
                public const string PendingOriginatorResubmission = "Pending Originator Resubmission";
                public const string PendingReportingManagerApproval = "Pending Reporting Manager Approval";
                public const string PendingInfraTeamAction = "Pending Infra Team Action";
                public const string PendingApplicationTeamAction = "Pending Application Team Action";
                public const string PendingITManagerApproval = "Pending IT Manager Approval";
                public const string PendingDepartmentAdminAction = "Pending Department Admin Action";
                public const string PendingAcknowledgement = "Pending Acknowledgement";
            }

            public class ITRequirements
            {
                public class ItemType
                {
                    public const string Infra = "Infra";
                    public const string Applications = "Applications";
                }
            }
        }

        #endregion

        #region SharePoint Group & Permission
        public class SPSecurityGroup
        {
            public const string ElanAdmin = "Elan Admin";
            public const string ElanMember = "Elan Member";
            public const string DepartmentAdmin = "Department Admin";
            public const string ITManager = "IT Manager";
            public const string ITInfra = "IT Infra";
            public const string ITApplications = "IT Applications";
        }
        #endregion

        #region Workflow

        public class WorkflowName
        {
            public const string ELANWorkflow = "ELAN Workflow";

        }

        public class WorkflowStatus
        {
            public const string PENDING_ORIGINATOR_RESUBMISSION = "Pending Originator Resubmission";
            public const string PENDING_REPORTING_MANAGER_APPROVAL = "Pending Reporting Manager Approval";
            public const string PENDING_INFRA_TEAM_ACTION = "Pending Infra Team Action";
            public const string PENDING_APPLICATION_TEAM_ACTION = "Pending Application Team Action";
            public const string PENDING_IT_MANAGER_APPROVAL = "Pending IT Manager Approval";
            public const string PENDING_DEPARTMENT_ADMIN_ACTION = "Pending Department Admin Action";
            public const string PENDING_ACKNOWLEDGEMENT = "Pending Acknowledgement";

            public const string COMPLETED = "Completed";
            public const string APPROVED = "Approved";
            public const string REJECTED = "Rejected";
            public const string TERMINATED = "Terminated";
            public const string DRAFT = "Draft";
            public const string ERROR = "Error";
        }

        public class TaskStatus
        {
            public const string Completed = "Completed";
            public const string IN_Progress = "In Progress";
            public const string Reassigned = "Reassigned";
            public const string Removed = "Removed";
        }

        public class WorkflowStepName
        {
            public class ELANWorkflow
            {
                public const string PendingOriginatorResubmission = "PendingOriginatorResubmission";
                public const string PendingReportingManagerApproval = "PendingReportingManagerApproval";
                public const string PendingInfraTeamAction = "PendingInfraTeamAction";
                public const string PendingApplicationTeamAction = "PendingApplicationTeamAction";
                public const string PendingITManagerApproval = "PendingITManagerApproval";
                public const string PendingDepartmentAdminAction = "PendingDepartmentAdminAction";
                public const string PendingAcknowledgement = "PendingAcknowledgement";

                public const string Completed = "Completed";
                public const string Approved = "Approved";
                public const string Rejected = "Rejected";
                public const string Terminated = "Terminated";
                public const string Draft = "Draft";
            }
        }

        public class WorkflowTemplateId
        {
            public const int PULZApprovalWorkflow = 1;
        }

        public class WorkflowActionName
        {
            public const string Complete = "Complete";
            public const string Approve = "Approve";
            public const string Reject = "Reject";
            public const string Default = "Default";
            public const string RequireAmendment = "Require Amendment";
            public const string Resubmit = "Resubmit";
            public const string Acknowledge = "Acknowledge";

            //Approval Form Save Draft Action (Not a workflow action)
            public const string Save = "Save";
        }

        public class WorkflowManagementAction
        {
            public const string AddNew = "add new";
            public const string Reassign = "reassign";
            public const string Remove = "remove";
            public const string RemoveAll = "remove all";
        }

        public class WorkflowDTColumn
        {
            public class WorkflowHistory
            {
                public const string ProcessID = "ProcessID";
                public const string ProcessName = "ProcessName";
                public const string ReferenceKey = "ReferenceKey";
                public const string InternalStepName = "InternalStepName";
                public const string EncryptParam = "EncryptParam";
                public const string StepName = "StepName";
                public const string AssigneeLogin = "AssigneeLogin";
                public const string AssigneeName = "AssigneeName";
                public const string AssigneeEmail = "AssigneeEmail";
                public const string AssignedDate = "AssignedDate";
                public const string DueDate = "DueDate";
                public const string Status = "Status";
                public const string ActionedDate = "ActionedDate";
                public const string ActionedBy = "ActionedBy";
                public const string ActionedByName = "ActionedByName";
                public const string ActionName = "ActionName";
                public const string Comments = "Comments";
                public const string TaskURL = "TaskURL";
                public const string TaskID = "TaskID";
                public const string ExtendedDays = "ExtendedDays";
            }

            public class MyPendingTask
            {
                public const string ProcessID = "ProcessID";
                public const string ProcessName = "ProcessName";
                public const string OriginatorLogin = "OriginatorLogin";
                public const string ReferenceKey = "ReferenceKey";
                public const string OriginatorName = "OriginatorName";
                public const string ProcessStartDate = "ProcessStartDate";
                public const string KeywordsXML = "KeywordsXML";
                public const string EncryptParam = "EncryptParam";
                public const string ApplicationName = "ApplicationName";
                public const string StepName = "StepName";
                public const string TaskURL = "TaskURL";
                public const string TaskID = "TaskID";
                public const string AssigneeLogin = "AssigneeLogin";
                public const string AssigneeEmail = "AssigneeEmail";
                public const string AssigneeName = "AssigneeName";
                public const string AssignedDate = "AssignedDate";
                public const string DueDate = "DueDate";
                public const string ExtendedDays = "ExtendedDays";
                public const string IsDelegatedTask = "IsDelegatedTask";
                public const string ActionerLogin = "ActionerLogin";
                // public const string ActionerEmail = "ActionerEmail";
                public const string ActionerName = "ActionerName";
                // public const string ActionedDate = "ActionedDate"; 
                public const string TaskStatus = "Status";
                // Note: maps to "Status" in Task
                // public const string WorkflowStartDate = "StartDate"; 
                // public const string StepTemplateID = "StepTemplateID";
                // public const string InternalStepName = "InternalStepName";
                // public const string CompletionDate = "CompletionDate"; 
                // public const string WorkflowStage = "WorkflowStage";
                // public const string TaskAction = "TaskAction";
                // public const string DelegateTo = "DelegateTo"; 
                // public const string DelegateToName = "DelegationToName"; 
            }

            public class MyActiveRequest
            {
                public const string RequestID = "RequestID";
                public const string ReferenceNo = "ReferenceNo";
                public const string EmployeeID = "EmployeeID";
                public const string EmployeeName = "EmployeeName";
                public const string RequestType = "RequestType";
                public const string WorkflowStatus = "WorkflowStatus";
                public const string Submitted = "Submitted";
            }

            public class ManagerTask
            {
                public const string ProcessID = "ProcessID";
                public const string ProcessName = "ProcessName";
                public const string InternalStepName = "InternalStepName";
                public const string StepName = "StepName";
                public const string TaskID = "TaskID";
                public const string TaskStatus = "TaskStatus";
                public const string AssigneeLogin = "AssigneeLogin";
                public const string AssigneeName = "AssigneeName";
                public const string AssigneeEmail = "AssigneeEmail";
                public const string AssignedDate = "AssignedDate";
                public const string RequestID = "RequestID";
                public const string ManagerLogin = "ManagerLogin";
                public const string ManagerName = "ManagerName";
                public const string ManagerEmail = "ManagerEmail";
            }
        }

        public class WorkflowKeywords
        {
            public class Common
            {
                public const string SPHostURL = "{SPHostUrl}";
                public const string SPWebURL = "{SPWebURL}";
                public const string WorkflowName = "{WorkflowName}";
                public const string WorkflowDueDate = "{WorkflowDueDate}";
                public const string WorkflowCycleDueDays = "{WorkflowCycleDueDays}";
                public const string AG_TimeZoneName = "{AG_TimeZoneName}";
                public const string AppAccessToken = "{AppAccessToken}";

                public const string RequestType = "{RequestType}";
                public const string RequestID = "{RequestID}";
                public const string RequestRefNo = "{RequestRefNo}";
                public const string EmployeeId = "{EmployeeId}";
                public const string EmployeeName = "{EmployeeName}";
                public const string ViewEmployeeLink = "{ViewEmployeeLink}";
                public const string ViewRequestLink = "{ViewRequestLink}";
                public const string Remarks = "{Remarks}";

                public const string Originator = "{Originator}";
                public const string OriginatorSubmittedByName = "{OriginatorSubmittedByName}";
                public const string OriginatorSubmittedByLogin = "{OriginatorSubmittedByLogin}";
                public const string OriginatorSubmittedByEmail = "{OriginatorSubmittedByEmail}";
            }


            public class TaskURL
            {
                public const string ProcessID = "{ProcessID}";
                public const string TaskID = "{TaskID}";
            }
        }

        public class WorkflowMatrix
        {
            public const string ReportingManager = "Reporting Manager";
            public const string InfraTeam = "IT Infra";
            public const string ApplicationTeam = "IT Application";
            public const string ITManager = "IT Manager";
            public const string DepartmentAdmin = "Department Admin";
            public const string Recipient = "Recipient";
        }

        #region Workflow On Going Tasks
        public class WFOnGoingTasks
        {
            public const string TableName = "WFOngoingTasks";
            public class ColumnName
            {
                public const string ID = "ID";
                public const string TaskID = "TaskID";
                public const string IsComplete = "IsComplete";
            }
        }
        #endregion

        #endregion

        #region Generic WF SQL
        public class WFSQLTableFields
        {
            public class d_tblStep
            {
                public const string StepName = "StepName";
                public const string InternalStepName = "InternalStepName";
                public const string StepOrder = "StepOrder";
                public const string DueDateDay = "DueDateDay";
                public const string EmailNotification = "EmailNotification";
                public const string EmailToAssignee = "EmailToAssignee";
                public const string EmailToOriginator = "EmailToOriginator";
                public const string EmailCCOriginator = "EmailCCOriginator";
                public const string EmailNotificationSubject = "EmailNotificationSubject";
                public const string EmailNotificationBody = "EmailNotificationBody";

                public const string TaskURL = "TaskURL";
                public const string LastStep = "LastStep";
                public const string EmailOnlyStep = "EmailOnlyStep";
                public const string CodeOnlyStep = "CodeOnlyStep";
                public const string AssemblyName = "AssemblyName";
                public const string ClassName = "ClassName";
                public const string MethodName = "MethodName";
                public const string ProcessID = "ProcessID";
                public const string StepID = "StepID";
            }

            public class d_tblProcess
            {
                public const string ProcessID = "ProcessID";
                public const string ProcessName = "ProcessName";
                public const string EncryptParam = "EncryptParam";
            }

            public class i_tblTask
            {
                public const string ProcessID = "ProcessID";
                public const string TaskID = "TaskID";
                public const string ActionID = "ActionID";
                public const string Status = "Status";
                public const string Comments = "Comments";
                public const string ActionedDate = "ActionedDate";
                public const string AssignedDate = "AssignedDate";
                public const string DueDate = "DueDate";
                public const string AssigneeEmail = "AssigneeEmail";
                public const string AssigneeLogin = "AssigneeLogin";
                public const string ActionedBy = "ActionedBy";
                public const string StepTemplateID = "StepTemplateID";
            }
        }
        #endregion

        #region SQL DataTable

        public class SQLDataTable
        {
            public class Table
            {
                public const string EmployeeDetails = "EmployeeDetails";
                public const string FinalEmployeeDetails = "Final_EmployeeDetails";
                public const string Request = "Request";
                public const string ITRequirements = "ITRequirements";
                public const string FolderPermission = "FolderPermission";
                public const string Hardware = "Hardware";
                public const string FinalHardware = "Final_Hardware";
                public const string FinalITRequirements = "Final_ITRequirements";
                public const string FinalFolderPermission = "Final_FolderPermission";
                public const string d_tblStep = "d_tblStep";

                public class FinalEmployeeDetailsColumns
                {
                    public const string ID = "ID";
                    public const string CompanyID = "CompanyID";
                    public const string CompanyTitle = "CompanyTitle";
                    public const string EmployeeName = "EmployeeName";
                    public const string EmployeeEmail = "EmployeeEmail";
                    public const string EmployeeLogin = "EmployeeLogin";
                    public const string EmployeeID = "EmployeeID";
                    public const string DesignationID = "DesignationID";
                    public const string DesignationTitle = "DesignationTitle";
                    public const string GradeID = "GradeID";
                    public const string GradeTitle = "GradeTitle";
                    public const string DepartmentID = "DepartmentID";
                    public const string DepartmentTitle = "DepartmentTitle";
                    public const string ReportingManagerEmail = "ReportingManagerEmail";
                    public const string ReportingManagerLogin = "ReportingManagerLogin";
                    public const string ReportingManagerName = "ReportingManagerName";
                    public const string LocationID = "LocationID";
                    public const string LocationTitle = "LocationTitle";
                    public const string MobileNo = "MobileNo";
                    public const string ContractOrTemporaryStaff = "ContractOrTemporaryStaff";
                    public const string JoinDate = "JoinDate";
                    public const string EndDate = "EndDate";
                    public const string Description1 = "Description1";
                    public const string Description2 = "Description2";
                    public const string Description3 = "Description3";
                    public const string Description4 = "Description4";
                    public const string Description5 = "Description5";
                    public const string Created = "Created";
                    public const string CreatedBy = "CreatedBy";
                    public const string CreatedByLogin = "CreatedByLogin";
                    public const string Modified = "Modified";
                    public const string ModifiedBy = "ModifiedBy";
                    public const string ModifiedByLogin = "ModifiedByLogin";
                    public const string EmployeeStatus = "EmployeeStatus";
                    public const string IsReminderEmailSent = "IsReminderEmailSent";
                    public const string NewReportingManagerName = "NewReportingManagerName";
                    public const string NewReportingManagerEmail = "NewReportingManagerEmail";
                    public const string NewReportingManagerLogin = "NewReportingManagerLogin";
                }

                public class EmployeeDetailsColumns
                {
                    public const string ID = "ID";
                    public const string RequestID = "RequestID";
                    public const string CompanyID = "CompanyID";
                    public const string CompanyTitle = "CompanyTitle";
                    public const string EmployeeName = "EmployeeName";
                    public const string EmployeeEmail = "EmployeeEmail";
                    public const string EmployeeLogin = "EmployeeLogin";
                    public const string EmployeeStatus = "EmployeeStatus";
                    public const string EmployeeID = "EmployeeID";
                    public const string DesignationID = "DesignationID";
                    public const string DesignationTitle = "DesignationTitle";
                    public const string GradeID = "GradeID";
                    public const string GradeTitle = "GradeTitle";
                    public const string DepartmentID = "DepartmentID";
                    public const string DepartmentTitle = "DepartmentTitle";
                    public const string ReportingManagerEmail = "ReportingManagerEmail";
                    public const string ReportingManagerLogin = "ReportingManagerLogin";
                    public const string ReportingManagerName = "ReportingManagerName";
                    public const string LocationID = "LocationID";
                    public const string LocationTitle = "LocationTitle";
                    public const string MobileNo = "MobileNo";
                    public const string ContractOrTemporaryStaff = "ContractOrTemporaryStaff";
                    public const string JoinDate = "JoinDate";
                    public const string EndDate = "EndDate";
                    public const string Description1 = "Description1";
                    public const string Description2 = "Description2";
                    public const string Description3 = "Description3";
                    public const string Description4 = "Description4";
                    public const string Description5 = "Description5";
                    public const string Created = "Created";
                    public const string CreatedBy = "CreatedBy";
                    public const string CreatedByLogin = "CreatedByLogin";
                    public const string Modified = "Modified";
                    public const string ModifiedBy = "ModifiedBy";
                    public const string ModifiedByLogin = "ModifiedByLogin";
                    public const string Remarks = "Remarks";
                    public const string NewReportingManagerName = "NewReportingManagerName";
                    public const string NewReportingManagerEmail = "NewReportingManagerEmail";
                    public const string NewReportingManagerLogin = "NewReportingManagerLogin";
                }

                public class RequestColumns
                {
                    public const string ID = "ID";
                    public const string FinalEmployeeDetailsID = "FinalEmployeeDetailsID";
                    public const string ReferenceNo = "ReferenceNo";
                    public const string WorkflowStatus = "WorkflowStatus";
                    public const string ProcessID = "ProcessID";
                    public const string Changes = "Changes";
                    public const string Created = "Created";
                    public const string CreatedBy = "CreatedBy";
                    public const string CreatedByLogin = "CreatedByLogin";
                    public const string Modified = "Modified";
                    public const string ModifiedBy = "ModifiedBy";
                    public const string ModifiedByLogin = "ModifiedByLogin";
                    public const string Submitted = "Submitted";
                    public const string SubmittedBy = "SubmittedBy";
                    public const string SubmittedByLogin = "SubmittedByLogin";
                    public const string RequestType = "RequestType";
                    public const string RMEscalation = "RMEscalation";
                    public const string ModifyEmployeeDetails = "ModifyEmployeeDetails";
                    public const string ModifyITRequirements = "ModifyITRequirements";
                    public const string ModifyHardware = "ModifyHardware";
                }

                public class ITRequirementsColumns
                {
                    public const string ID = "ID";
                    public const string RequestID = "RequestID";
                    public const string ItemID = "ItemID";
                    public const string ItemTitle = "ItemTitle";
                    public const string Type = "Type";
                    public const string IsAdded = "IsAdded";
                    public const string IsRemoved = "IsRemoved";
                    public const string DateAdded = "DateAdded";
                    public const string DateRemoved = "DateRemoved";
                    public const string Remark = "Remark";
                    public const string Created = "Created";
                    public const string CreatedBy = "CreatedBy";
                    public const string CreatedByLogin = "CreatedByLogin";
                    public const string Modified = "Modified";
                    public const string ModifiedBy = "ModifiedBy";
                    public const string ModifiedByLogin = "ModifiedByLogin";
                }

                public class FolderPermissionColumns
                {
                    public const string ID = "ID";
                    public const string NameOrPath = "NameOrPath";
                    public const string IsRead = "IsRead";
                    public const string IsWrite = "IsWrite";
                    public const string IsDelete = "IsDelete";
                    public const string Status = "Status";
                    public const string IsAdded = "IsAdded";
                    public const string IsRemoved = "IsRemoved";
                    public const string DateAdded = "DateAdded";
                    public const string DateRemoved = "DateRemoved";
                    public const string Remark = "Remark";
                    public const string RequestID = "RequestID";
                    public const string Created = "Created";
                    public const string CreatedBy = "CreatedBy";
                    public const string CreatedByLogin = "CreatedByLogin";
                    public const string Modified = "Modified";
                    public const string ModifiedBy = "ModifiedBy";
                    public const string ModifiedByLogin = "ModifiedByLogin";
                }

                public class HardwareColumns
                {
                    public const string ID = "ID";
                    public const string RequestID = "RequestID";
                    public const string DepartmentID = "DepartmentID";
                    public const string DepartmentTitle = "DepartmentTitle";
                    public const string ItemID = "ItemID";
                    public const string ItemTitle = "ItemTitle";
                    public const string Quantity = "Quantity";
                    public const string RemarkHistory = "RemarkHistory";
                    public const string Model = "Model";
                    public const string SerialNumber = "SerialNumber";
                    public const string DateAssigned = "DateAssigned";
                    public const string IsReturned = "IsReturned";
                    public const string DateReturned = "DateReturned";
                    public const string IsReceived = "IsReceived";
                    public const string DateReceived = "DateReceived";
                    public const string Created = "Created";
                    public const string CreatedBy = "CreatedBy";
                    public const string CreatedByLogin = "CreatedByLogin";
                    public const string Modified = "Modified";
                    public const string ModifiedBy = "ModifiedBy";
                    public const string ModifiedByLogin = "ModifiedByLogin";
                }

                public class FinalHardwareColumns
                {
                    public const string ID = "ID";
                    public const string FinalEmployeeDetailsID = "FinalEmployeeDetailsID";
                    public const string DepartmentID = "DepartmentID";
                    public const string DepartmentTitle = "DepartmentTitle";
                    public const string ItemID = "ItemID";
                    public const string ItemTitle = "ItemTitle";
                    public const string Quantity = "Quantity";
                    public const string RemarkHistory = "RemarkHistory";
                    public const string Model = "Model";
                    public const string SerialNumber = "SerialNumber";
                    public const string DateAssigned = "DateAssigned";
                    public const string IsReturned = "IsReturned";
                    public const string DateReturned = "DateReturned";
                    public const string IsReceived = "IsReceived";
                    public const string DateReceived = "DateReceived";
                    public const string Created = "Created";
                    public const string CreatedBy = "CreatedBy";
                    public const string CreatedByLogin = "CreatedByLogin";
                    public const string Modified = "Modified";
                    public const string ModifiedBy = "ModifiedBy";
                    public const string ModifiedByLogin = "ModifiedByLogin";
                }

                public class FinalITRequirementsColumns
                {
                    public const string ID = "ID";
                    public const string FinalEmployeeDetailsID = "FinalEmployeeDetailsID";
                    public const string ItemID = "ItemID";
                    public const string ItemTitle = "ItemTitle";
                    public const string Type = "Type";
                    public const string IsAdded = "IsAdded";
                    public const string IsRemoved = "IsRemoved";
                    public const string DateAdded = "DateAdded";
                    public const string DateRemoved = "DateRemoved";
                    public const string Remark = "Remark";
                    public const string Created = "Created";
                    public const string CreatedBy = "CreatedBy";
                    public const string CreatedByLogin = "CreatedByLogin";
                    public const string Modified = "Modified";
                    public const string ModifiedBy = "ModifiedBy";
                    public const string ModifiedByLogin = "ModifiedByLogin";
                }

                public class FinalFolderPermissionColumns
                {
                    public const string ID = "ID";
                    public const string FinalEmployeeDetailsID = "FinalEmployeeDetailsID";
                    public const string NameOrPath = "NameOrPath";
                    public const string IsRead = "IsRead";
                    public const string IsWrite = "IsWrite";
                    public const string IsDelete = "IsDelete";
                    public const string Status = "Status";
                    public const string IsAdded = "IsAdded";
                    public const string IsRemoved = "IsRemoved";
                    public const string DateAdded = "DateAdded";
                    public const string DateRemoved = "DateRemoved";
                    public const string Remark = "Remark";
                    public const string Created = "Created";
                    public const string CreatedBy = "CreatedBy";
                    public const string CreatedByLogin = "CreatedByLogin";
                    public const string Modified = "Modified";
                    public const string ModifiedBy = "ModifiedBy";
                    public const string ModifiedByLogin = "ModifiedByLogin";
                }
            }
        }

        public class SQLTableColumnValue
        {
            public class FinalEmployeeDetails
            {
                public class EmployeeStatus
                {
                    public const string Active = "Active";
                    public const string Resigned = "Resigned";
                }

                public class ContractOrTemporaryStaff
                {
                    public const string Contract = "Contract";
                    public const string Permanent = "Permanent";
                }
            }

            public class Request
            {
                public class Changes
                {
                    public const string NoChanges = "No changes";
                }
            }
        }
        #endregion

        #region Store Procedure
        public class StoreProcedureName
        {
            public const string GetMyPendingAndIncomingTasksByBatchByProcessNameApplicationName = "ELan_GetMyPendingAndIncomingTasksByBatchByProcessNameApplicationName";
            public const string GetMyPendingAndIncomingTasksCountByBatchByProcessNameApplicationName = "ELan_GetMyPendingAndIncomingTasksCountByBatchByProcessNameApplicationName";
            public const string GetPendingReportingManagerApprovalTasksByProcessName = "ELan_GetPendingReportingManagerApprovalTasksByProcessName";
            public const string GetActiveActionerByProcessIDStepTemplateID = "ELan_GetActiveActionerByProcessIDStepTemplateID";

            public const string GetMyActiveRequest = "ELan_GetMyActiveRequest";
            public const string GetMyActiveRequestCount = "ELan_GetMyActiveRequestCount";

            public const string SearchRequestListing = "ELan_SearchRequestListing";
            public const string GetSearchRequestListingCount = "ELan_GetSearchRequestListingCount";

            public const string SearchEmployeeListing = "ELan_SearchEmployeeListing";
            public const string GetSearchEmployeeListingCount = "ELan_GetSearchEmployeeListingCount";

            public const string GetWorkflowTaskHistoryByProcessID = "ELan_GetWorkflowTaskHistoryByProcessID";

            public const string GetExpiringContractEmployees = "ELan_GetExpiringContractEmployees";
        }

        public class StoreProcedureParameter
        {
            public const string ActionerLogin = "ActionerLogin";
            public const string ProcessName = "ProcessName";
            public const string ApplicationName = "ApplicationName";
            public const string PageNumber = "PageNumber";
            public const string RowsPerPage = "RowsPerPage";
            public const string ProcessTemplateID = "ProcessTemplateID";
            public const string WorkflowStatus = "WorkflowStatus";

            public class RequestListing
            {
                public const string PageNumber = "PageNumber";
                public const string RowsPerPage = "RowsPerPage";
                public const string ReferenceNo = "ReferenceNo";
                public const string EmployeeName = "EmployeeName";
                public const string EmployeeID = "EmployeeID";
                public const string SubmittedBy = "SubmittedBy";
                public const string Designation = "Designation";
                public const string Department = "Department";
                public const string CreatedStartDate = "CreatedStartDate";
                public const string CreatedEndDate = "CreatedEndDate";
                public const string SubmittedStartDate = "SubmittedStartDate";
                public const string SubmittedEndDate = "SubmittedEndDate";
                public const string WorkflowStatus = "WorkflowStatus";
                public const string RequestType = "RequestType";
                public const string SortField = "SortField";
                public const string SortFieldTable = "SortFieldTable";
                public const string SortDirection = "SortDirection";
                public const string ManagerInChargeDepartments = "ManagerInChargeDepartments";
                public const string MemberLogin = "MemberLogin";
                public const string CreatedByLogin = "CreatedByLogin";
            }

            public class EmployeeListing
            {
                public const string PageNumber = "PageNumber";
                public const string RowsPerPage = "RowsPerPage";
                public const string EmployeeName = "EmployeeName";
                public const string EmployeeID = "EmployeeID";
                public const string Designation = "Designation";
                public const string Grade = "Grade";
                public const string Department = "Department";
                public const string ReportingManager = "ReportingManager";
                public const string Location = "Location";
                public const string JoinedStartDate = "JoinedStartDate";
                public const string JoinedEndDate = "JoinedEndDate";
                public const string ContractOrPermanent = "ContractOrPermanent";
                public const string EmployeeStatus = "EmployeeStatus";
                public const string SortField = "SortField";
                public const string SortFieldTable = "SortFieldTable";
                public const string SortDirection = "SortDirection";
                public const string ManagerInChargeDepartments = "ManagerInChargeDepartments";
                public const string MemberLogin = "MemberLogin";
            }

            public class ReminderEmail
            {
                public const string RemainingMonths = "@RemainingMonths";
            }
        }

        public class StoreProcedureColumn
        {
            public class RequestListing
            {
                public const string RequestID = "RequestID";
                public const string ReferenceNo = "ReferenceNo";
                public const string EmployeeName = "EmployeeName";
                public const string EmployeeID = "EmployeeID";
                public const string SubmittedBy = "SubmittedBy";
                public const string Designation = "Designation";
                public const string Department = "Department";
                public const string CreatedDate = "CreatedDate";
                public const string SubmittedDate = "SubmittedDate";
                public const string WorkflowStatus = "WorkflowStatus";
                public const string RequestType = "RequestType";
                public const string CreatedByLogin = "CreatedByLogin";
            }

            public class EmployeeListing
            {
                public const string ID = "ID";
                public const string EmployeeName = "EmployeeName";
                public const string EmployeeID = "EmployeeID";
                public const string Designation = "Designation";
                public const string Grade = "Grade";
                public const string Department = "Department";
                public const string ReportingManager = "ReportingManager";
                public const string Location = "Location";
                public const string JoinedDate = "JoinedDate";
                public const string ContractOrTemporaryStaff = "ContractOrTemporaryStaff";
                public const string EmployeeStatus = "EmployeeStatus";
            }
        }
        #endregion

        #region Permission Config Function
        public class PermissionConfigFunction
        {
            public class Request
            {
                public const string CreateNewRequest = "Create New Request";
                public const string CreateModificationRequest = "Create Modification Request";
            }
        }
        #endregion
    }
}
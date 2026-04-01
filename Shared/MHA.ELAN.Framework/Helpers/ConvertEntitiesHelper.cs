using ProcessKeywords = MHA.Framework.Core.SP;
using MHA.Framework.Core.Workflow.BO;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.JSONConstants;
using Microsoft.SharePoint.Client;
using System.Data;
using FieldHelper = MHA.Framework.Core.SP;
using FieldHelperGeneral = MHA.Framework.Core.General;

namespace MHA.ELAN.Framework.Helpers
{
    public static class ConvertEntitiesHelper
    {
        private static readonly JSONAppSettings appSettings;

        static ConvertEntitiesHelper()
        {
            appSettings = ConfigurationManager.GetAppSetting();
        }

        #region Running Number
        public static RunningNumber ConvertRunningNumberObject(ListItem li)
        {
            RunningNumber rnObj = new RunningNumber();

            rnObj.Format = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.RunningNumber.Format);
            rnObj.ID = FieldHelper.GetFieldValueAsNumber(li, ConstantHelper.SPColumn.RunningNumber.ID);
            rnObj.Number = FieldHelper.GetFieldValueAsNumber(li, ConstantHelper.SPColumn.RunningNumber.Number);
            rnObj.Prefix = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.RunningNumber.Prefix);
            rnObj.Title = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.RunningNumber.Title);

            return rnObj;
        }

        public static RunningNumberFormat ConvertRunningNumberFormatObject(ListItem li)
        {
            RunningNumberFormat rnfObj = new RunningNumberFormat();

            rnfObj.Autonumber = FieldHelper.GetFieldValueAsBoolean(li, ConstantHelper.SPColumn.RunningNumberFormat.Autonumber);
            rnfObj.Format = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.RunningNumberFormat.Format);
            rnfObj.ID = FieldHelper.GetFieldValueAsNumber(li, ConstantHelper.SPColumn.RunningNumberFormat.ID);
            rnfObj.Prefix = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.RunningNumberFormat.Prefix);
            rnfObj.Title = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.RunningNumberFormat.Title);

            return rnfObj;
        }
        #endregion

        #region Pending Task
        public static MyPendingTask ConvertMyPendingTaskObj(DataRow dr, ClientContext context)
        {
            string keywordsXML = dr[ConstantHelper.WorkflowDTColumn.MyPendingTask.KeywordsXML] + "";
            ProcessKeywords keywords = new ProcessKeywords(keywordsXML);
            MyPendingTask myTaskObj = new MyPendingTask();

            DateTime? dueDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.WorkflowDTColumn.MyPendingTask.DueDate);
            if (dueDate != DateTime.MinValue)
            {
                myTaskObj.DueDate = dueDate;
                bool useSPTimeZone = false;
                bool.TryParse(appSettings.AG_UseSPTimeZoneforDBDateTimeColumn, out useSPTimeZone);
                myTaskObj.DueDateDisplay = FieldHelper.GetFieldValueAsDateTimeStringWithSPTimeZone(dr, ConstantHelper.WorkflowDTColumn.MyPendingTask.DueDate, useSPTimeZone, context, appSettings.DefaultDateFormat);
            }

            myTaskObj.StepName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.MyPendingTask.StepName);
            myTaskObj.TaskID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.WorkflowDTColumn.MyPendingTask.TaskID);
            myTaskObj.ProcessID = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WFSQLTableFields.i_tblTask.ProcessID);

            // Additional fields extracted from keywords XML
            myTaskObj.WorkflowName = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.WorkflowName);
            myTaskObj.RequestID = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.RequestID);
            myTaskObj.RequestReferenceNo = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.RequestRefNo);
            myTaskObj.RequestType = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.RequestType);
            myTaskObj.EmployeeId = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.EmployeeId);
            myTaskObj.EmployeeName = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.EmployeeName);

            // If is incoming task, URL set to view request form
            string taskUrl = dr[ConstantHelper.WFSQLTableFields.d_tblStep.TaskURL].ToString();
            taskUrl = taskUrl
                .Replace(ConstantHelper.WorkflowKeywords.TaskURL.ProcessID, dr[ConstantHelper.WFSQLTableFields.i_tblTask.ProcessID].ToString())
                .Replace(ConstantHelper.WorkflowKeywords.TaskURL.TaskID, dr[ConstantHelper.WFSQLTableFields.i_tblTask.TaskID].ToString());
            dr[ConstantHelper.WFSQLTableFields.d_tblStep.TaskURL] = taskUrl;

            //TODO
            if (myTaskObj.TaskID <= 0)
            {
                // Set Task URL
                myTaskObj.TaskURL = string.Format(
                        ConstantHelper.URLTemplate.RequestDisplayFormUrlTemplate,
                        appSettings.RemoteAppURL.TrimEnd('/'),
                        myTaskObj.RequestID,
                        context.Web.Url);

                myTaskObj.IsUpcomingTask = true;
            }
            else
            {
                myTaskObj.TaskURL = appSettings.RemoteAppURL.TrimEnd('/') + FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.MyPendingTask.TaskURL);
            }

            //string actionerName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.MyPendingTask.AssigneeName);
            //if (string.IsNullOrEmpty(actionerName))
            //{
            //    actionerName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.MyPendingTask.ActionerName);
            //}
            //myTaskObj.ActionerName = actionerName;
            //myTaskObj.ModifiedDate = DateTimeHelper.GetCurrentDateTime();

            myTaskObj.WorkflowDueDate = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.WorkflowDueDate);

            return myTaskObj;
        }
        #endregion

        #region SharePoint Maintenance List
        public static SPDepartment ConvertDepartmentObject(ListItem li)
        {
            SPDepartment obj = new SPDepartment();

            obj.ID = FieldHelper.GetFieldValueAsNumber(li, ConstantHelper.SPColumn.Department.ID);
            obj.Title = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.Department.Title);
            obj.Status = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.Department.Status);

            return obj;
        }

        public static SPDesignation ConvertDesignationObject(ListItem li)
        {
            SPDesignation obj = new SPDesignation();

            obj.ID = FieldHelper.GetFieldValueAsNumber(li, ConstantHelper.SPColumn.Designation.ID);
            obj.Title = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.Designation.Title);
            obj.Status = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.Designation.Status);

            return obj;
        }

        public static SPGrade ConvertGradeObject(ListItem li)
        {
            SPGrade obj = new SPGrade();

            obj.ID = FieldHelper.GetFieldValueAsNumber(li, ConstantHelper.SPColumn.Designation.ID);
            obj.Title = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.Designation.Title);
            obj.Status = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.Designation.Status);

            return obj;
        }

        public static SPLocation ConvertLocationObject(ListItem li)
        {
            SPLocation obj = new SPLocation();

            obj.ID = FieldHelper.GetFieldValueAsNumber(li, ConstantHelper.SPColumn.Designation.ID);
            obj.Title = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.Designation.Title);
            obj.Status = FieldHelper.GetFieldValueAsString(li, ConstantHelper.SPColumn.Designation.Status);

            return obj;
        }
        #endregion

        #region My Active Request
        public static MyActiveRequestData ConvertMyActiveRequestObj(DataRow dr)
        {
            MyActiveRequestData obj = new();
            obj.RequestID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.WorkflowDTColumn.MyActiveRequest.RequestID);
            obj.ReferenceNo = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.MyActiveRequest.ReferenceNo);
            obj.EmployeeID = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.MyActiveRequest.EmployeeID);
            obj.EmployeeName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.MyActiveRequest.EmployeeName);
            obj.RequestType = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.MyActiveRequest.RequestType);
            obj.WorkflowStatus = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.MyActiveRequest.WorkflowStatus);
            obj.Submitted = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.WorkflowDTColumn.MyActiveRequest.Submitted);

            return obj;
        }
        #endregion

        #region Request Listing
        public static RequestListingData ConvertRequestListingObj(DataRow dr)
        {
            RequestListingData obj = new();
            obj.RequestID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.StoreProcedureColumn.RequestListing.RequestID);
            obj.ReferenceNo = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.RequestListing.ReferenceNo);
            obj.EmployeeName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.RequestListing.EmployeeName);
            obj.EmployeeID = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.RequestListing.EmployeeID);
            obj.SubmittedBy = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.RequestListing.SubmittedBy);
            obj.Designation = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.RequestListing.Designation);
            obj.Department = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.RequestListing.Department);
            DateTime createdDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.StoreProcedureColumn.RequestListing.CreatedDate);
            obj.CreatedDate = createdDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(createdDate) : createdDate;
            DateTime submittedDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.StoreProcedureColumn.RequestListing.SubmittedDate);
            obj.SubmittedDate = submittedDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(submittedDate) : submittedDate;
            obj.WorkflowStatus = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.RequestListing.WorkflowStatus);
            obj.RequestType = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.RequestListing.RequestType);
            obj.CreatedByLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.RequestListing.CreatedByLogin);

            return obj;
        }
        #endregion

        #region Employee Listing
        public static EmployeeListingData ConvertEmployeeListingObj(DataRow dr)
        {
            EmployeeListingData obj = new();
            obj.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.StoreProcedureColumn.EmployeeListing.ID);
            obj.EmployeeName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.EmployeeListing.EmployeeName);
            obj.EmployeeID = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.EmployeeListing.EmployeeID);
            obj.Designation = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.EmployeeListing.Designation);
            obj.Grade = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.EmployeeListing.Grade);
            obj.Department = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.EmployeeListing.Department);
            obj.ReportingManager = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.EmployeeListing.ReportingManager);
            obj.Location = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.EmployeeListing.Location);
            DateTime joinedDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.StoreProcedureColumn.EmployeeListing.JoinedDate);
            obj.JoinedDate = joinedDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(joinedDate) : joinedDate;
            obj.ContractOrPermanent = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.EmployeeListing.ContractOrTemporaryStaff);
            obj.EmployeeStatus = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.StoreProcedureColumn.EmployeeListing.EmployeeStatus);

            return obj;
        }
        #endregion

        #region View Request Form
        public static ViewRequestItem ConvertViewRequestItemObj(DataTable dt)
        {
            ViewRequestItem obj = new ViewRequestItem();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                obj.RequestID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.ID);
                obj.FinalEmpID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.FinalEmployeeDetailsID);
                obj.ProcessID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.ProcessID);
                obj.ReferenceNo = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.ReferenceNo);
                obj.WorkflowStatus = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.WorkflowStatus);
                obj.RequestType = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.RequestType);

                obj.ModifyEmployeeDetails = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyEmployeeDetails);
                obj.ModifyITRequirements = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyITRequirements);
                obj.ModifyHardware = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyHardware);

                DateTime createdDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.Created);
                obj.CreatedDate = createdDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(createdDate).ToString(appSettings.DefaultDateFormat) : string.Empty;
            }

            return obj;
        }

        public static ViewModelEmployeeDetails ConvertViewEmployeeDetailsItemObj(DataTable dt)
        {
            ViewModelEmployeeDetails obj = new ViewModelEmployeeDetails();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                obj.RequestID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.RequestID);
                obj.EmployeeDetailsVM.CompanyTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.CompanyTitle);
                obj.EmployeeDetailsVM.EmployeeName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeName);
                obj.EmployeeDetailsVM.EmployeeLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeLogin);
                obj.EmployeeDetailsVM.EmployeeEmail = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeEmail);
                obj.EmployeeDetailsVM.EmployeeID = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeID);
                obj.EmployeeDetailsVM.DesignationTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DesignationTitle);
                obj.EmployeeDetailsVM.GradeTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.GradeTitle);
                obj.EmployeeDetailsVM.DepartmentID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DepartmentID);
                obj.EmployeeDetailsVM.DepartmentTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DepartmentTitle);
                obj.EmployeeDetailsVM.ReportingManagerName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerName);
                obj.EmployeeDetailsVM.ReportingManagerLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerLogin);
                obj.EmployeeDetailsVM.ReportingManagerEmail = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerEmail);
                obj.EmployeeDetailsVM.LocationTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.LocationTitle);
                obj.EmployeeDetailsVM.MobileNo = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.MobileNo);
                obj.EmployeeDetailsVM.ContractTemporaryStaff = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ContractOrTemporaryStaff);

                DateTime joinedDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.JoinDate);
                obj.EmployeeDetailsVM.JoinedDate = joinedDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(joinedDate) : joinedDate;

                DateTime endDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EndDate);
                obj.EmployeeDetailsVM.EndDate = endDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(endDate) : endDate;

                obj.EmployeeDetailsVM.Description1 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description1);
                obj.EmployeeDetailsVM.Description2 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description2);
                obj.EmployeeDetailsVM.Description3 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description3);
                obj.EmployeeDetailsVM.Description4 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description4);
                obj.EmployeeDetailsVM.Description5 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description5);
                obj.EmployeeDetailsVM.Remarks = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Remarks);
                obj.EmployeeDetailsVM.EmployeeStatus = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeStatus);
                //Termination Request
                obj.EmployeeDetailsVM.NewReportingManagerName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerName);
                obj.EmployeeDetailsVM.NewReportingManagerLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerLogin);
                obj.EmployeeDetailsVM.NewReportingManagerEmail = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerEmail);
            }

            return obj;
        }

        public static List<ViewModelHardware> ConvertViewHardwareItemObj(DataTable dt)
        {
            List<ViewModelHardware> list = new List<ViewModelHardware>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewModelHardware obj = new ViewModelHardware();

                    obj.HardwareVM.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.ID);
                    obj.HardwareVM.RequestID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.RequestID);
                    obj.HardwareVM.DepartmentTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.DepartmentTitle);
                    obj.HardwareVM.ItemID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.ItemID);
                    obj.HardwareVM.ItemTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.ItemTitle);
                    obj.HardwareVM.Quantity = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.Quantity);

                    DateTime dateAssigned = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.DateAssigned);
                    obj.HardwareVM.DateAssigned = dateAssigned != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateAssigned) : dateAssigned;

                    obj.HardwareVM.Model = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.Model);
                    obj.HardwareVM.SerialNumber = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.SerialNumber);
                    obj.HardwareVM.RemarkHistory = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.RemarkHistory);
                    obj.HardwareVM.IsReturned = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.IsReturned);
                    obj.HardwareVM.DateReturned = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.HardwareColumns.DateReturned);

                    if (obj.HardwareVM.DateAssigned == DateTime.MinValue)
                    {
                        obj.HardwareVM.DateAssigned = null;
                    }

                    if (obj.HardwareVM.DateReturned == DateTime.MinValue)
                    {
                        obj.HardwareVM.DateReturned = null;
                    }

                    list.Add(obj);
                }
            }

            return list;
        }

        public static List<ViewModelITRequirements> ConvertViewITRequirementsItemObj(DataTable dt)
        {
            List<ViewModelITRequirements> list = new List<ViewModelITRequirements>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewModelITRequirements obj = new ViewModelITRequirements();

                    obj.ITRequirementsVM.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ID);
                    obj.ITRequirementsVM.RequestID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.RequestID);
                    obj.ITRequirementsVM.ItemTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemTitle);
                    obj.ITRequirementsVM.Type = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Type);
                    obj.ITRequirementsVM.IsAdded = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsAdded);
                    obj.ITRequirementsVM.IsRemoved = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsRemoved);

                    DateTime dateAdded = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateAdded);
                    obj.ITRequirementsVM.DateAdded = dateAdded != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateAdded) : dateAdded;

                    DateTime dateRemoved = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateRemoved);
                    obj.ITRequirementsVM.DateRemoved = dateRemoved != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateRemoved) : dateRemoved;

                    obj.ITRequirementsVM.Remark = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Remark);

                    list.Add(obj);
                }
            }

            return list;
        }

        public static List<ViewModelFolderPermission> ConvertViewFolderPermissionObj(DataTable dt)
        {
            List<ViewModelFolderPermission> list = new List<ViewModelFolderPermission>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewModelFolderPermission obj = new ViewModelFolderPermission();

                    obj.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ID);
                    obj.RequestID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.RequestID);
                    obj.NameOrPath = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.NameOrPath);
                    obj.IsRead = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRead);
                    obj.IsWrite = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsWrite);
                    obj.IsDelete = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsDelete);
                    obj.Status = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Status);
                    obj.IsAdded = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsAdded);
                    obj.IsRemoved = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRemoved);

                    DateTime dateAdded = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateAdded);
                    obj.DateAdded = dateAdded != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateAdded) : dateAdded;

                    DateTime dateRemoved = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateRemoved);
                    obj.DateRemoved = dateRemoved != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateRemoved) : dateRemoved;

                    obj.Remark = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Remark);

                    list.Add(obj);
                }
            }

            return list;
        }

        public static List<WorkflowHistory> ConvertWorkflowHistoryObj(DataTable dt)
        {
            List<WorkflowHistory> list = new List<WorkflowHistory>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    WorkflowHistory obj = new WorkflowHistory();

                    obj.Action = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.ActionName);
                    obj.ActionedBy = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.ActionedBy);
                    obj.ActionedByName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.ActionedByName);

                    obj.AssigneeLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.AssigneeLogin);
                    obj.AssigneeName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.AssigneeName);
                    obj.AssigneeEmail = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.AssigneeEmail);

                    obj.Comments = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.Comments);
                    obj.Status = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.Status);
                    obj.StepName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.StepName);
                    obj.TaskID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.TaskID);
                    obj.TaskURL = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.TaskURL);
                    obj.ProcessName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.ProcessName);

                    DateTime actionDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.ActionedDate);
                    obj.ActionedDate = actionDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(actionDate).ToString(appSettings.DefaultDateFormat) : string.Empty;

                    DateTime assignedDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.AssignedDate);
                    obj.AssignedDate = assignedDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(assignedDate).ToString(appSettings.DefaultDateFormat) : string.Empty;

                    DateTime dueDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.DueDate);
                    obj.DueDate = dueDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dueDate).ToString(appSettings.DefaultDateFormat) : string.Empty;

                    list.Add(obj);
                }
            }

            return list;
        }

        public static List<ViewChangesItem> ConvertChangesObj(DataTable dt)
        {
            List<ViewChangesItem> list = new List<ViewChangesItem>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string changes = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.Changes);

                    if (string.IsNullOrEmpty(changes)) continue;

                    string[] parts = changes.Split(ConstantHelper.Delimit.SemiColonDelimit, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string part in parts)
                    {
                        ViewChangesItem obj = new ViewChangesItem();

                        obj.Changes = part;

                        list.Add(obj);
                    }
                }
            }

            return list;
        }

        #endregion


        #region View Employee Form
        public static List<ViewRequestItem> ConvertRequestItemList(DataTable dt)
        {
            List<ViewRequestItem> list = new List<ViewRequestItem>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewRequestItem obj = new ViewRequestItem();

                    obj.RequestID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.ID);
                    obj.FinalEmpID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.FinalEmployeeDetailsID);
                    obj.ProcessID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.ProcessID);
                    obj.ReferenceNo = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.ReferenceNo);
                    obj.WorkflowStatus = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.WorkflowStatus);
                    obj.RequestType = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.RequestType);

                    DateTime createdDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.RequestColumns.Created);
                    obj.CreatedDate = createdDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(createdDate).ToString(appSettings.DefaultDateFormat) : string.Empty;

                    list.Add(obj);
                }
            }

            return list;
        }

        public static ViewFinalEmployeeDetailsItem ConvertViewFinalEmployeeDetailsItemObj(DataTable dt)
        {
            ViewFinalEmployeeDetailsItem obj = new ViewFinalEmployeeDetailsItem();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                obj.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ID);
                obj.CompanyTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.CompanyTitle);
                obj.EmployeeName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeName);
                obj.EmployeeID = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeID);
                obj.DesignationTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DesignationTitle);
                obj.GradeTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.GradeTitle);
                obj.DepartmentID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DepartmentID);
                obj.DepartmentTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DepartmentTitle);
                obj.ReportingManagerName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerName);
                obj.LocationTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.LocationTitle);
                obj.MobileNo = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.MobileNo);
                obj.ContractOrTemporaryStaff = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ContractOrTemporaryStaff);
                DateTime joinDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.JoinDate);
                obj.JoinDate = joinDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(joinDate) : joinDate;
                DateTime endDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EndDate);
                obj.EndDate = endDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(endDate) : endDate;
                obj.Description1 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description1);
                obj.Description2 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description2);
                obj.Description3 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description3);
                obj.Description4 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description4);
                obj.Description5 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description5);
            }

            return obj;
        }

        public static List<ViewFinalHardwareItem> ConvertViewFinalHardwareItemObj(DataTable dt)
        {
            List<ViewFinalHardwareItem> list = new List<ViewFinalHardwareItem>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewFinalHardwareItem obj = new ViewFinalHardwareItem();

                    obj.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ID);
                    obj.FinalEmployeeDetailsID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.FinalEmployeeDetailsID);
                    obj.DepartmentTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DepartmentTitle);
                    obj.ItemID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ItemID);
                    obj.ItemTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ItemTitle);
                    obj.Quantity = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.Quantity);
                    obj.Remarks = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.RemarkHistory);

                    list.Add(obj);
                }
            }

            return list;
        }

        public static List<ViewFinalITRequirementsItem> ConvertViewFinalITRequirementsItemObj(DataTable dt)
        {
            List<ViewFinalITRequirementsItem> list = new List<ViewFinalITRequirementsItem>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewFinalITRequirementsItem obj = new ViewFinalITRequirementsItem();

                    obj.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ID);
                    obj.FinalEmployeeDetailsID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.FinalEmployeeDetailsID);
                    obj.ItemTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ItemTitle);
                    obj.Type = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.Type);
                    obj.IsAdded = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.IsAdded);
                    obj.IsRemoved = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.IsRemoved);
                    DateTime dateAdded = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.DateAdded);
                    obj.DateAdded = dateAdded != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateAdded) : dateAdded;
                    DateTime dateRemoved = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.DateRemoved);
                    obj.DateRemoved = dateRemoved != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateRemoved) : dateRemoved;
                    obj.Remark = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.Remark);

                    list.Add(obj);
                }
            }

            return list;
        }

        public static List<ViewFinalFolderPermission> ConvertViewFinalFolderPermissionObj(DataTable dt)
        {
            List<ViewFinalFolderPermission> list = new List<ViewFinalFolderPermission>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewFinalFolderPermission obj = new ViewFinalFolderPermission();

                    obj.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.ID);
                    obj.FinalEmployeeDetailsID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.FinalEmployeeDetailsID);
                    obj.NameOrPath = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.NameOrPath);
                    obj.IsRead = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsRead);
                    obj.IsWrite = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsWrite);
                    obj.IsDelete = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsDelete);
                    obj.Status = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.Status);
                    obj.IsAdded = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsAdded);
                    obj.IsRemoved = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsRemoved);
                    DateTime dateAdded = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.DateAdded);
                    obj.DateAdded = dateAdded != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateAdded) : dateAdded;
                    DateTime dateRemoved = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.DateRemoved);
                    obj.DateRemoved = dateRemoved != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateRemoved) : dateRemoved;
                    obj.Remark = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.Remark);

                    list.Add(obj);
                }
            }

            return list;
        }
        #endregion

        #region Manager
        public static List<ManagerTask> ConvertManagerTaskObj(DataTable dt)
        {
            List<ManagerTask> list = new List<ManagerTask>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ManagerTask obj = new ManagerTask();

                    obj.ProcessId = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.ProcessID);
                    obj.ProcessName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.ProcessName);
                    obj.StepName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.StepName);
                    obj.StepName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.InternalStepName);
                    obj.TaskStatus = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.TaskStatus);
                    obj.TaskID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.TaskID);
                    obj.AssigneeLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.AssigneeLogin);
                    obj.AssigneeName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.AssigneeName);
                    obj.AssigneeEmail = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.AssigneeEmail);

                    DateTime assignedDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.WorkflowDTColumn.WorkflowHistory.AssignedDate);
                    obj.AssignedDate = assignedDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(assignedDate).ToString(appSettings.DefaultDateFormat) : string.Empty;

                    obj.RequestID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.RequestID);
                    obj.ManagerLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.ManagerLogin);
                    obj.ManagerName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.ManagerName);
                    obj.ManagerEmail = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.WorkflowDTColumn.ManagerTask.ManagerEmail);

                    list.Add(obj);
                }
            }

            return list;
        }
        #endregion

        #region People Picker

        public static PeoplePickerUser ConvertPeoplePickerUser(User user)
        {
            if (user != null)
            {
                PeoplePickerUser ppUser = new PeoplePickerUser();
                ppUser.Name = user.Title;
                ppUser.Login = user.LoginName;
                ppUser.Email = user.Email;
                ppUser.LookupId = user.Id;

                return ppUser;
            }
            else
            {
                return null;
            }
        }

        public static PeoplePickerUser ConvertPeoplePickerUser(User user, ClientContext clientContext)
        {
            if (user != null)
            {
                clientContext.Load(user);
                clientContext.ExecuteQuery();

                PeoplePickerUser ppUser = new PeoplePickerUser();
                ppUser.Name = user.Title;
                ppUser.Login = user.LoginName;
                ppUser.Email = user.Email;
                ppUser.LookupId = user.Id;

                return ppUser;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Final Table Conversion
        public static ViewModelFinalEmployeeDetails ConvertViewModelFinalEmployeeDetailsObj(DataTable dt)
        {
            ViewModelFinalEmployeeDetails obj = new ViewModelFinalEmployeeDetails();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                obj.Final_EmployeeDetails.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ID);
                obj.Final_EmployeeDetails.CompanyID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.CompanyID);
                obj.Final_EmployeeDetails.CompanyTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.CompanyTitle);
                obj.Final_EmployeeDetails.EmployeeName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeName);
                obj.Final_EmployeeDetails.EmployeeEmail = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeEmail);
                obj.Final_EmployeeDetails.EmployeeLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeLogin);
                obj.Final_EmployeeDetails.EmployeeID = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeID);
                obj.Final_EmployeeDetails.DesignationID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DesignationID);
                obj.Final_EmployeeDetails.DesignationTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DesignationTitle);
                obj.Final_EmployeeDetails.GradeID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.GradeID);
                obj.Final_EmployeeDetails.GradeTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.GradeTitle);
                obj.Final_EmployeeDetails.DepartmentID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DepartmentID);
                obj.Final_EmployeeDetails.DepartmentTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DepartmentTitle);
                obj.Final_EmployeeDetails.ReportingManagerEmail = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerEmail);
                obj.Final_EmployeeDetails.ReportingManagerLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerLogin);
                obj.Final_EmployeeDetails.ReportingManagerName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerName);
                obj.Final_EmployeeDetails.LocationID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.LocationID);
                obj.Final_EmployeeDetails.LocationTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.LocationTitle);
                obj.Final_EmployeeDetails.MobileNo = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.MobileNo);
                obj.Final_EmployeeDetails.ContractOrTemporaryStaff = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ContractOrTemporaryStaff);

                DateTime joinDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.JoinDate);
                obj.Final_EmployeeDetails.JoinDate = joinDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(joinDate) : joinDate;

                DateTime endDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EndDate);
                obj.Final_EmployeeDetails.EndDate = endDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(endDate) : (DateTime?)null;

                obj.Final_EmployeeDetails.Description1 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description1);
                obj.Final_EmployeeDetails.Description2 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description2);
                obj.Final_EmployeeDetails.Description3 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description3);
                obj.Final_EmployeeDetails.Description4 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description4);
                obj.Final_EmployeeDetails.Description5 = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description5);

                DateTime createdDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Created);
                obj.Final_EmployeeDetails.Created = createdDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(createdDate) : createdDate;

                obj.Final_EmployeeDetails.CreatedBy = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.CreatedBy);
                obj.Final_EmployeeDetails.CreatedByLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.CreatedByLogin);

                DateTime modifiedDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Modified);
                obj.Final_EmployeeDetails.Modified = modifiedDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(modifiedDate) : modifiedDate;

                obj.Final_EmployeeDetails.ModifiedBy = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ModifiedBy);
                obj.Final_EmployeeDetails.ModifiedByLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ModifiedByLogin);

                //termination request
                obj.Final_EmployeeDetails.NewReportingManagerLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.NewReportingManagerLogin);
                obj.Final_EmployeeDetails.NewReportingManagerName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.NewReportingManagerName);
                obj.Final_EmployeeDetails.NewReportingManagerEmail = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.NewReportingManagerEmail);

                if (obj.Final_EmployeeDetails.EndDate == DateTime.MinValue)
                {
                    obj.Final_EmployeeDetails.EndDate = null;
                }
            }

            return obj;
        }

        public static List<ViewModelFinalITRequirements> ConvertViewModelFinalITRequirementsObj(DataTable dt)
        {
            List<ViewModelFinalITRequirements> list = new List<ViewModelFinalITRequirements>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewModelFinalITRequirements obj = new ViewModelFinalITRequirements();

                    obj.Final_ITRequirements.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ID);
                    obj.Final_ITRequirements.ItemID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ItemID);
                    obj.Final_ITRequirements.ItemTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ItemTitle);
                    obj.Final_ITRequirements.Type = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.Type);
                    obj.Final_ITRequirements.IsAdded = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.IsAdded);
                    obj.Final_ITRequirements.IsRemoved = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.IsRemoved);

                    DateTime dateAdded = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.DateAdded);
                    obj.Final_ITRequirements.DateAdded = dateAdded != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateAdded) : dateAdded;

                    DateTime dateRemoved = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.DateRemoved);
                    obj.Final_ITRequirements.DateRemoved = dateRemoved != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateRemoved) : dateRemoved;

                    obj.Final_ITRequirements.Remark = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.Remark);

                    DateTime createdDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.Created);
                    obj.Final_ITRequirements.Created = createdDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(createdDate) : createdDate;

                    obj.Final_ITRequirements.CreatedBy = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.CreatedBy);
                    obj.Final_ITRequirements.CreatedByLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.CreatedByLogin);

                    DateTime modifiedDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.Modified);
                    obj.Final_ITRequirements.Modified = modifiedDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(modifiedDate) : modifiedDate;

                    obj.Final_ITRequirements.ModifiedBy = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ModifiedBy);
                    obj.Final_ITRequirements.ModifiedByLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ModifiedByLogin);

                    list.Add(obj);
                }
            }

            return list;
        }

        public static List<ViewModelFinalFolderPermission> ConvertViewModelFinalFolderPermission(DataTable dt)
        {
            List<ViewModelFinalFolderPermission> list = new List<ViewModelFinalFolderPermission>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewModelFinalFolderPermission obj = new ViewModelFinalFolderPermission();

                    obj.Final_FolderPermission.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.ID);
                    obj.Final_FolderPermission.NameOrPath = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.NameOrPath);
                    obj.Final_FolderPermission.IsRead = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsRead);
                    obj.Final_FolderPermission.IsWrite = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsWrite);
                    obj.Final_FolderPermission.IsDelete = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsDelete);
                    obj.Final_FolderPermission.Status = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.Status);
                    obj.Final_FolderPermission.IsAdded = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsAdded);
                    obj.Final_FolderPermission.IsRemoved = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsRemoved);

                    DateTime dateAdded = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.DateAdded);
                    obj.Final_FolderPermission.DateAdded = dateAdded != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateAdded) : dateAdded;

                    DateTime dateRemoved = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.DateRemoved);
                    obj.Final_FolderPermission.DateRemoved = dateRemoved != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateRemoved) : dateRemoved;

                    obj.Final_FolderPermission.Remark = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.Remark);

                    DateTime createdDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.Created);
                    obj.Final_FolderPermission.Created = createdDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(createdDate) : createdDate;

                    obj.Final_FolderPermission.CreatedBy = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.CreatedBy);
                    obj.Final_FolderPermission.CreatedByLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.CreatedByLogin);

                    DateTime modifiedDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.Modified);
                    obj.Final_FolderPermission.Modified = modifiedDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(modifiedDate) : modifiedDate;

                    obj.Final_FolderPermission.ModifiedBy = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.ModifiedBy);
                    obj.Final_FolderPermission.ModifiedByLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.ModifiedByLogin);

                    list.Add(obj);
                }
            }

            return list;
        }

        public static List<ViewModelFinalHardware> ConvertViewModelFinalHardwareObj(DataTable dt)
        {
            List<ViewModelFinalHardware> list = new List<ViewModelFinalHardware>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ViewModelFinalHardware obj = new ViewModelFinalHardware();

                    obj.Final_Hardware.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ID);
                    obj.Final_Hardware.DepartmentID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DepartmentID);
                    obj.Final_Hardware.DepartmentTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DepartmentTitle);
                    obj.Final_Hardware.ItemID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ItemID);
                    obj.Final_Hardware.ItemTitle = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ItemTitle);
                    obj.Final_Hardware.Quantity = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.Quantity);
                    obj.Final_Hardware.RemarkHistory = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.RemarkHistory);
                    obj.Final_Hardware.Model = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.Model);
                    obj.Final_Hardware.SerialNumber = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.SerialNumber);

                    DateTime dateAssigned = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DateAssigned);
                    obj.Final_Hardware.DateAssigned = dateAssigned != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateAssigned) : dateAssigned;

                    obj.Final_Hardware.IsReturned = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.IsReturned);

                    DateTime dateReturned = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DateReturned);
                    obj.Final_Hardware.DateReturned = dateReturned != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateReturned) : dateReturned;

                    obj.Final_Hardware.IsReceived = FieldHelperGeneral.GetFieldValueAsBoolean(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.IsReceived);

                    DateTime dateReceived = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DateReceived);
                    obj.Final_Hardware.DateReceived = dateReceived != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(dateReceived) : dateReceived;

                    DateTime createdDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.Created);
                    obj.Final_Hardware.Created = createdDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(createdDate) : createdDate;

                    DateTime modifiedDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.Modified);
                    obj.Final_Hardware.Modified = modifiedDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(modifiedDate) : modifiedDate;

                    obj.Final_Hardware.CreatedBy = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.CreatedBy);
                    obj.Final_Hardware.CreatedByLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.CreatedByLogin);
                    obj.Final_Hardware.ModifiedBy = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ModifiedBy);
                    obj.Final_Hardware.ModifiedByLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ModifiedByLogin);

                    list.Add(obj);
                }
            }

            return list;
        }
        #endregion

        #region HR Reminder Email
        public static List<EmailReminderEmployeeDetails> ConvertReminderEmailObject(DataTable dt)
        {
            List<EmailReminderEmployeeDetails> list = new();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    EmailReminderEmployeeDetails obj = new();

                    obj.ID = FieldHelperGeneral.GetFieldValueAsNumber(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ID);
                    obj.EmployeeID = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeID);
                    obj.EmployeeName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeName);
                    obj.Designation = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DesignationTitle);
                    obj.Department = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DepartmentTitle);

                    DateTime endDate = FieldHelperGeneral.GetFieldValueAsDateTime(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EndDate);
                    if (endDate != DateTime.MinValue)
                    {
                        endDate = DateTimeHelper.ConvertToLocalDateTime(endDate);
                        obj.ContractEndDate = endDate.ToString(appSettings.DefaultDateFormat);
                    }

                    list.Add(obj);
                }
            }

            return list;
        }
        #endregion

        #region Request Form

        public static List<ViewModelSubordinate> ConvertViewModelSubordinates(DataTable dt)
        {
            List<ViewModelSubordinate> obj = new List<ViewModelSubordinate>();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    obj.Add(new ViewModelSubordinate
                    {
                        Name = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeName),
                        Email = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeEmail),
                        Status = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeStatus),
                        ReportingManagerName = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerName),
                        ReportingManagerEmail = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerEmail),
                        ReportingManagerLogin = FieldHelperGeneral.GetFieldValueAsString(dr, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerLogin)
                    });
                }
            }

            return obj;
        }

        #endregion
    }
}

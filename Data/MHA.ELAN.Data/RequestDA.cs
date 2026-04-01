using MHA.Framework.Core.General;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using Microsoft.SharePoint.News.DataModel;
using System.Data;
using System.Data.SqlTypes;

namespace MHA.ELAN.Data
{
    public class RequestDA
    {
        private readonly SQLHelper sqlHelper;

        public RequestDA()
        {
            sqlHelper = new SQLHelper(ConnectionStringHelper.GetGenericWFConnString());
        }

        #region Insert

        public int InitCreateRequestID(ViewModelRequest vm)
        {
            List<string> parameterNames = new List<string>
            {
                ConstantHelper.SQLDataTable.Table.RequestColumns.ReferenceNo,
                ConstantHelper.SQLDataTable.Table.RequestColumns.WorkflowStatus,
                ConstantHelper.SQLDataTable.Table.RequestColumns.Changes,
                ConstantHelper.SQLDataTable.Table.RequestColumns.Created,
                ConstantHelper.SQLDataTable.Table.RequestColumns.CreatedBy,
                ConstantHelper.SQLDataTable.Table.RequestColumns.CreatedByLogin,
                ConstantHelper.SQLDataTable.Table.RequestColumns.Modified,
                ConstantHelper.SQLDataTable.Table.RequestColumns.ModifiedBy,
                ConstantHelper.SQLDataTable.Table.RequestColumns.ModifiedByLogin,
                ConstantHelper.SQLDataTable.Table.RequestColumns.RequestType,
                ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyEmployeeDetails,
                ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyITRequirements,
                ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyHardware,
                ConstantHelper.SQLDataTable.Table.RequestColumns.FinalEmployeeDetailsID
            };

            List<object> parameterValues = new List<object>
            {
                vm.ReferenceNo ?? string.Empty,
                vm.WorkflowStatus ?? string.Empty,
                vm.Changes ?? string.Empty,
                vm.Created != null && vm.Created != DateTime.MinValue? DateTimeHelper.ConvertToUTCDateTime( vm.Created.Value) : DateTimeHelper.GetCurrentUtcDateTime(),
                vm.CreatedBy ?? string.Empty,
                vm.CreatedByLogin ?? string.Empty,
                vm.Modified !=null && vm.Modified != DateTime.MinValue? DateTimeHelper.ConvertToUTCDateTime( vm.Modified.Value) : DateTimeHelper.GetCurrentUtcDateTime(),
                vm.ModifiedBy ?? string.Empty,
                vm.ModifiedByLogin ?? string.Empty,
                vm.RequestType ?? string.Empty,
                vm.ModifyEmployeeDetails,
                vm.ModifyITRequirements,
                vm.ModifyHardware,
                vm.EmployeeDetails.EmployeeDetailsVM.TempFinalEmployeeDetailsID ?? (object)DBNull.Value
            };

            int requestID = sqlHelper.Create(ConstantHelper.SQLDataTable.Table.Request, parameterNames, parameterValues);
            return requestID;
        }

        public int InitCreateFinalEmployeeID(ViewModelApproval vm)
        {
            List<string> parameterNames = new List<string>
            {
                ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Created,
                ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.CreatedBy,
                ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.CreatedByLogin,
                ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Modified,
                ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ModifiedBy,
                ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ModifiedByLogin,
            };

            List<object> parameterValues = new List<object>
            {
                vm.FinalEmployeeDetails.Final_EmployeeDetails.Created != DateTime.MinValue? DateTimeHelper.ConvertToUTCDateTime(vm.FinalEmployeeDetails.Final_EmployeeDetails.Created.Value) : DateTimeHelper.GetCurrentUtcDateTime(),
                vm.FinalEmployeeDetails.Final_EmployeeDetails.CreatedBy ?? string.Empty,
                vm.FinalEmployeeDetails.Final_EmployeeDetails.CreatedByLogin ?? string.Empty,
                vm.FinalEmployeeDetails.Final_EmployeeDetails.Modified != DateTime.MinValue? DateTimeHelper.ConvertToUTCDateTime(vm.FinalEmployeeDetails.Final_EmployeeDetails.Modified.Value) : DateTimeHelper.GetCurrentUtcDateTime(),
                vm.FinalEmployeeDetails.Final_EmployeeDetails.ModifiedBy ?? string.Empty,
                vm.FinalEmployeeDetails.Final_EmployeeDetails.ModifiedByLogin ?? string.Empty
            };

            int finalEmpID = sqlHelper.Create(ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, parameterNames, parameterValues);
            return finalEmpID;
        }

        public ViewModelRequest CreateNewRequest(ViewModelRequest vm, string spHostURL)
        {
            try
            {
                int createNewRequestID = 0;
                vm.EmployeeDetails.EmployeeDetailsVM.EmployeeStatus = "Active"; //TODO: Temporary here
                RequestDA reqDA = new RequestDA();
                int? finalEmpID = 0;
                if (vm.EmployeeDetails.EmployeeDetailsVM.TempFinalEmployeeDetailsID > 0)
                {
                    finalEmpID = vm.EmployeeDetails.EmployeeDetailsVM.TempFinalEmployeeDetailsID;
                }

                int requestID = InitCreateRequestID(vm);
                vm.ID = requestID;

                // Retrieve title for dropdowns
                vm.EmployeeDetails.EmployeeDetailsVM.CompanyTitle = vm.EmployeeDetails.EmployeeDetailsVM.CompanyList.FirstOrDefault(x => x.Value == vm.EmployeeDetails.EmployeeDetailsVM.CompanyID?.ToString())?.Text ?? "";
                vm.EmployeeDetails.EmployeeDetailsVM.DesignationTitle = vm.EmployeeDetails.EmployeeDetailsVM.DesignationList.FirstOrDefault(x => x.Value == vm.EmployeeDetails.EmployeeDetailsVM.DesignationID?.ToString())?.Text ?? "";
                vm.EmployeeDetails.EmployeeDetailsVM.DepartmentTitle = vm.EmployeeDetails.EmployeeDetailsVM.DepartmentList.FirstOrDefault(x => x.Value == vm.EmployeeDetails.EmployeeDetailsVM.DepartmentID?.ToString())?.Text ?? "";
                vm.EmployeeDetails.EmployeeDetailsVM.LocationTitle = vm.EmployeeDetails.EmployeeDetailsVM.LocationList.FirstOrDefault(x => x.Value == vm.EmployeeDetails.EmployeeDetailsVM.LocationID?.ToString())?.Text ?? "";
                vm.EmployeeDetails.EmployeeDetailsVM.GradeTitle = vm.EmployeeDetails.EmployeeDetailsVM.GradeList.FirstOrDefault(x => x.Value == vm.EmployeeDetails.EmployeeDetailsVM.GradeID?.ToString())?.Text ?? "";

                #region Employee Details
                List<string> parameterNames = new List<string> {
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.RequestID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.CompanyID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.CompanyTitle,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeName,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeEmail,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeLogin,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeStatus,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DesignationID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DesignationTitle,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.GradeID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.GradeTitle,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DepartmentID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DepartmentTitle,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerEmail,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerLogin,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerName,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.LocationID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.LocationTitle,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.MobileNo,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ContractOrTemporaryStaff,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.JoinDate,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EndDate,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description1,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description2,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description3,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description4,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description5,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Created,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.CreatedBy,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.CreatedByLogin,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Modified,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ModifiedBy,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ModifiedByLogin,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Remarks
                };

                List<object> parameterValues = new List<object> {
                    vm.ID ?? (object)DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.CompanyID ?? (object)DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.CompanyTitle ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.EmployeeName ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.EmployeeEmail ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.EmployeeStatus ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.EmployeeID ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.DesignationID ?? (object)DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.DesignationTitle ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.GradeID ?? (object)DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.GradeTitle ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.DepartmentID ?? (object)DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.DepartmentTitle ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerEmail ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerLogin ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerName ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.LocationID ?? (object)DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.LocationTitle ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.MobileNo ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.ContractTemporaryStaff ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.JoinedDate != null && vm.EmployeeDetails.EmployeeDetailsVM.JoinedDate >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime( vm.EmployeeDetails.EmployeeDetailsVM.JoinedDate.Value) : DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.EndDate != null && vm.EmployeeDetails.EmployeeDetailsVM.EndDate >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.EmployeeDetails.EmployeeDetailsVM.EndDate.Value) : DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.Description1 ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Description2 ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Description3 ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Description4 ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Description5 ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Created != null && vm.EmployeeDetails.EmployeeDetailsVM.Created >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.EmployeeDetails.EmployeeDetailsVM.Created.Value) : DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.CreatedBy ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.CreatedByLogin ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Modified != null && vm.EmployeeDetails.EmployeeDetailsVM.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.EmployeeDetails.EmployeeDetailsVM.Modified.Value) : DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.ModifiedBy ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.ModifiedByLogin ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Remarks ?? string.Empty
                };

                // Reaasign to new manager - Termination Request
                if (vm.RequestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType && vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManager != null)
                {
                    parameterNames.Add(ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerName);
                    parameterNames.Add(ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerEmail);
                    parameterNames.Add(ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerLogin);

                    parameterValues.Add(vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManager.Name ?? string.Empty);
                    parameterValues.Add(vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManager.Email ?? string.Empty);
                    parameterValues.Add(vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManager.Login ?? string.Empty);
                }

                #endregion

                #region IT Requirements
                var itRequirementsInsertList = new List<(List<string> parameterNamesIT, List<object> parameterValuesIT)>();
                var folderPermissionsInsertList = new List<(List<string> parameterNames, List<object> parameterValues)>();

                foreach (var itReq in vm.ITRequirementsVM.SelectedITRequirements)
                {
                    var parameterNamesIT = new List<string>
                    {
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.RequestID,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemID,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemTitle,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Type,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsAdded,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsRemoved,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateAdded,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateRemoved,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Remark,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Created,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.CreatedBy,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.CreatedByLogin,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Modified,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ModifiedBy,
                        ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ModifiedByLogin,
                    };

                    var parameterValuesIT = new List<object>
                    {
                        vm.ID ?? (object)DBNull.Value,
                        itReq.ItemID ?? (object)DBNull.Value,
                        itReq.ItemTitle ?? string.Empty,
                        itReq.Type ?? string.Empty,
                        itReq.IsAdded,
                        itReq.IsRemoved,
                        itReq.DateAdded != null && itReq.DateAdded >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.DateAdded.Value) : DBNull.Value,
                        itReq.DateRemoved != null && itReq.DateRemoved >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.DateRemoved.Value) : DBNull.Value,
                        itReq.Remark ?? string.Empty,
                        itReq.Created != null && itReq.Created >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.Created.Value) : DBNull.Value,
                        itReq.CreatedBy ?? string.Empty,
                        itReq.CreatedByLogin ?? string.Empty,
                        itReq.Modified != null && itReq.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.Modified.Value) : DBNull.Value,
                        itReq.ModifiedBy ?? string.Empty,
                        itReq.ModifiedByLogin ?? string.Empty
                    };

                    itRequirementsInsertList.Add((parameterNamesIT, parameterValuesIT));
                }

                #endregion

                #region IT Requirements - Folder Permission
                foreach (var folder in vm.ITRequirementsVM.FolderPermissions)
                {
                    if (folder.IsBeingRemoved)
                    {
                        if (folder.ID > 0) // Only delete if it existed during init and being removed
                        {
                            reqDA.DeleteByID(ConstantHelper.SQLDataTable.Table.FolderPermission, folder.ID);
                        }
                        continue;
                    }

                    var parameterNamesFolder = new List<string>
                    {
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.RequestID,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.NameOrPath,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRead,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsWrite,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsDelete,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Status,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsAdded,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRemoved,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateAdded,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateRemoved,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Remark,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Created,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.CreatedBy,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.CreatedByLogin,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Modified,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedBy,
                        ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedByLogin
                    };

                    var parameterValuesFolder = new List<object>
                    {
                        vm.ID ?? (object)DBNull.Value,
                        folder.NameOrPath ?? string.Empty,
                        folder.IsRead,
                        folder.IsWrite,
                        folder.IsDelete,
                        folder.Status ?? string.Empty,
                        folder.IsAdded,
                        folder.IsRemoved,
                        folder.DateAdded != null && folder.DateAdded >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.DateAdded.Value) : DBNull.Value,
                        folder.DateRemoved != null && folder.DateRemoved >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.DateRemoved.Value) : DBNull.Value,
                        folder.Remark ?? string.Empty,
                        folder.Created != null && folder.Created >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.Created.Value) : DBNull.Value,
                        folder.CreatedBy ?? string.Empty,
                        folder.CreatedByLogin ?? string.Empty,
                        folder.Modified != null && folder.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.Modified.Value) : DBNull.Value,
                        folder.ModifiedBy ?? string.Empty,
                        folder.ModifiedByLogin ?? string.Empty
                    };

                    folderPermissionsInsertList.Add((parameterNamesFolder, parameterValuesFolder));
                }
                #endregion

                #region Hardware

                var hardwareInsertList = new List<(List<string> paramNames, List<object> paramValues)>();

                foreach (var hw in vm.HardwareVM.SelectedHardwareItems)
                {
                    bool existsInFinal = ProjectHelper.Exists(
                            ConstantHelper.SQLDataTable.Table.FinalHardware,
                            new List<string>
                            {
                                ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ItemID,
                                ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.FinalEmployeeDetailsID
                            },
                            new List<object> { hw.ItemID, finalEmpID }
                        );

                    if (hw.IsTempRemoved)
                    {
                        if (existsInFinal)
                        {
                            // Item exists in FinalHardware but temp removed, still create and mark as returned - Need clarification
                            hw.IsReturned = true;
                            hw.DateReturned = DateTime.Now;
                        }
                        else // Only delete if it existed during init and being removed
                        {
                            if (hw.ID > 0)
                            {
                                reqDA.DeleteByID(ConstantHelper.SQLDataTable.Table.Hardware, (int)hw.ID);
                            }
                            continue;
                        }
                    }

                    var deptText = vm.HardwareVM.DepartmentList.FirstOrDefault(d => int.TryParse(d.Value, out var v)
                                            && hw.DepartmentID.HasValue
                                            && v == hw.DepartmentID.Value)?.Text ?? string.Empty;

                    var parameterNamesHardware = new List<string>
                    {
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.RequestID,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.DepartmentID,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.DepartmentTitle,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.ItemID,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.ItemTitle,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.Quantity,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.RemarkHistory,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.Model,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.SerialNumber,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.DateAssigned,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.IsReturned,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.DateReturned,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.IsReceived,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.DateReceived,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.Created,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.CreatedBy,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.CreatedByLogin,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.Modified,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.ModifiedBy,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.ModifiedByLogin
                    };

                    var parameterValuesHardware = new List<object>
                    {
                        vm.ID ?? (object)DBNull.Value,
                        hw.DepartmentID ?? (object)DBNull.Value,
                        deptText,
                        hw.ItemID,
                        hw.ItemTitle ?? string.Empty,
                        hw.Quantity,
                        hw.RemarkHistory ?? string.Empty,
                        hw.Model ?? string.Empty,
                        hw.SerialNumber ?? string.Empty,
                        hw.DateAssigned != null && hw.DateAssigned >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.DateAssigned.Value) : DBNull.Value,
                        hw.IsReturned,
                        hw.DateReturned != null && hw.DateReturned >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.DateReturned.Value) : DBNull.Value,
                        hw.IsReceived,
                        hw.DateReceived != null && hw.DateReceived >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.DateReceived.Value) : DBNull.Value,
                        hw.Created != null && hw.Created >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.Created.Value) : DBNull.Value,
                        hw.CreatedBy ?? string.Empty,
                        hw.CreatedByLogin ?? string.Empty,
                        hw.Modified != null && hw.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.Modified.Value) : DBNull.Value,
                        hw.ModifiedBy ?? string.Empty,
                        hw.ModifiedByLogin ?? string.Empty
                    };

                    hardwareInsertList.Add((parameterNamesHardware, parameterValuesHardware));
                }

                #endregion

                #region Execute insertion

                //Employee Details
                createNewRequestID = sqlHelper.Create(ConstantHelper.SQLDataTable.Table.EmployeeDetails, parameterNames, parameterValues);
                vm.EmployeeDetails.EmployeeDetailsVM.ID = createNewRequestID;

                //IT Requirements
                foreach (var (paramNames, paramValues) in itRequirementsInsertList)
                {
                    sqlHelper.Create(ConstantHelper.SQLDataTable.Table.ITRequirements, paramNames, paramValues);
                }

                //Folder Permissions
                foreach (var (paramNames, paramValues) in folderPermissionsInsertList)
                {
                    sqlHelper.Create(ConstantHelper.SQLDataTable.Table.FolderPermission, paramNames, paramValues);
                }

                //Hardware
                foreach (var (paramNames, paramValues) in hardwareInsertList)
                {
                    sqlHelper.Create(ConstantHelper.SQLDataTable.Table.Hardware, paramNames, paramValues);
                }

                vm.HasSuccess = true;
                #endregion

                return vm;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ViewModelApproval CreateFinalEmployeeInstance(ViewModelApproval vm)
        {
            try
            {
                if (vm.ViewRequestItem.RequestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType)
                {
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeStatus = "Resigned";
                }
                else
                {
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeStatus = "Active";
                }

                int? finalEmpID = null;
                RequestDA da = new RequestDA();

                if (vm.ViewRequestItem.FinalEmpID > 0)
                {
                    finalEmpID = vm.ViewRequestItem.FinalEmpID;
                }
                else
                {
                    // New request
                    finalEmpID = InitCreateFinalEmployeeID(vm);

                    // Update Request with new FinalEmployeeDetailsID
                    if (vm.ViewRequestItem.RequestID.HasValue && finalEmpID.HasValue)
                    {
                        sqlHelper.Update(
                            ConstantHelper.SQLDataTable.Table.Request,
                            new List<string> { ConstantHelper.SQLDataTable.Table.RequestColumns.FinalEmployeeDetailsID },
                            new List<object> { finalEmpID.Value },
                            ConstantHelper.SQLDataTable.Table.RequestColumns.ID,
                            vm.ViewRequestItem.RequestID.Value
                        );
                    }
                }

                #region Employee Details
                List<string> parameterNames = new List<string> {
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.CompanyID,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.CompanyTitle,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeName,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeEmail,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeLogin,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeStatus,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeID,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DesignationID,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DesignationTitle,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.GradeID,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.GradeTitle,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DepartmentID,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.DepartmentTitle,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerEmail,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerLogin,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerName,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.LocationID,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.LocationTitle,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.MobileNo,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ContractOrTemporaryStaff,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.JoinDate,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EndDate,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description1,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description2,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description3,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description4,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Description5,

                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.NewReportingManagerLogin,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.NewReportingManagerName,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.NewReportingManagerEmail
                };

                List<object> parameterValues = new List<object> {
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.CompanyID ?? (object)DBNull.Value,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.CompanyTitle ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeName ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeEmail ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeLogin ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeStatus ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeID ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.DesignationID ?? (object)DBNull.Value,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.DesignationTitle ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.GradeID ?? (object)DBNull.Value,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.GradeTitle ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.DepartmentID ?? (object)DBNull.Value,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.DepartmentTitle ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.ReportingManagerEmail ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.ReportingManagerLogin ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.ReportingManagerName ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.LocationID ?? (object)DBNull.Value,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.LocationTitle ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.MobileNo ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.ContractOrTemporaryStaff ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.JoinDate != null && vm.FinalEmployeeDetails.Final_EmployeeDetails.JoinDate >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.FinalEmployeeDetails.Final_EmployeeDetails.JoinDate.Value) : DBNull.Value,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.EndDate != null && vm.FinalEmployeeDetails.Final_EmployeeDetails.EndDate >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.FinalEmployeeDetails.Final_EmployeeDetails.EndDate.Value) : DBNull.Value,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.Description1 ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.Description2 ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.Description3 ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.Description4 ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.Description5 ?? string.Empty,

                    vm.FinalEmployeeDetails.Final_EmployeeDetails.NewReportingManagerLogin ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.NewReportingManagerName ?? string.Empty,
                    vm.FinalEmployeeDetails.Final_EmployeeDetails.NewReportingManagerEmail ?? string.Empty
                };

                #endregion

                #region IT Requirements
                var itRequirementsInsertList = new List<(int itemId, List<string> parameterNamesIT, List<object> parameterValuesIT)>();

                foreach (var itReq in vm.FinalITRequirements)
                {
                    var parameterNamesIT = new List<string>
                    {
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.FinalEmployeeDetailsID,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ItemID,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ItemTitle,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.Type,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.IsAdded,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.IsRemoved,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.DateAdded,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.DateRemoved,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.Remark,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.Created,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.CreatedBy,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.CreatedByLogin,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.Modified,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ModifiedBy,
                        ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ModifiedByLogin,
                    };

                    var parameterValuesIT = new List<object>
                    {
                        finalEmpID ?? (object)DBNull.Value,
                        itReq.Final_ITRequirements.ItemID ?? (object)DBNull.Value,
                        itReq.Final_ITRequirements.ItemTitle ?? string.Empty,
                        itReq.Final_ITRequirements.Type ?? string.Empty,
                        itReq.Final_ITRequirements.IsAdded,
                        itReq.Final_ITRequirements.IsRemoved,
                        itReq.Final_ITRequirements.DateAdded != null && itReq.Final_ITRequirements.DateAdded >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.Final_ITRequirements.DateAdded.Value) : DBNull.Value,
                        itReq.Final_ITRequirements.DateRemoved != null && itReq.Final_ITRequirements.DateRemoved >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.Final_ITRequirements.DateRemoved.Value) : DBNull.Value,
                        itReq.Final_ITRequirements.Remark ?? string.Empty,
                        itReq.Final_ITRequirements.Created != null && itReq.Final_ITRequirements.Created >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.Final_ITRequirements.Created.Value) : DBNull.Value,
                        itReq.Final_ITRequirements.CreatedBy ?? string.Empty,
                        itReq.Final_ITRequirements.CreatedByLogin ?? string.Empty,
                        itReq.Final_ITRequirements.Modified != null && itReq.Final_ITRequirements.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.Final_ITRequirements.Modified.Value) : DBNull.Value,
                        itReq.Final_ITRequirements.ModifiedBy ?? string.Empty,
                        itReq.Final_ITRequirements.ModifiedByLogin ?? string.Empty
                    };

                    itRequirementsInsertList.Add((itReq.Final_ITRequirements.ItemID.Value, parameterNamesIT, parameterValuesIT));
                }

                #endregion

                #region IT Requirements - Folder Permission
                var folderPermissionsInsertList = new List<(List<string> parameterNames, List<object> parameterValues)>();

                sqlHelper.Delete(
                   ConstantHelper.SQLDataTable.Table.FinalFolderPermission,
                   new List<string> { ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.FinalEmployeeDetailsID },
                   new List<object> { finalEmpID }
                );

                foreach (var folder in vm.FinalFolderPermissions)
                {
                    var parameterNamesFolder = new List<string>
                    {
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.FinalEmployeeDetailsID,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.NameOrPath,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsRead,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsWrite,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsDelete,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.Status,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsAdded,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.IsRemoved,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.DateAdded,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.DateRemoved,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.Remark,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.Created,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.CreatedBy,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.CreatedByLogin,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.Modified,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.ModifiedBy,
                        ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.ModifiedByLogin
                    };

                    var parameterValuesFolder = new List<object>
                    {
                        finalEmpID ?? (object)DBNull.Value,
                        folder.Final_FolderPermission.NameOrPath ?? string.Empty,
                        folder.Final_FolderPermission.IsRead,
                        folder.Final_FolderPermission.IsWrite,
                        folder.Final_FolderPermission.IsDelete,
                        folder.Final_FolderPermission.Status ?? string.Empty,
                        folder.Final_FolderPermission.IsAdded,
                        folder.Final_FolderPermission.IsRemoved,
                        folder.Final_FolderPermission.DateAdded != null && folder.Final_FolderPermission.DateAdded >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.Final_FolderPermission.DateAdded.Value) : DBNull.Value,
                        folder.Final_FolderPermission.DateRemoved != null && folder.Final_FolderPermission.DateRemoved >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.Final_FolderPermission.DateRemoved.Value) : DBNull.Value,
                        folder.Final_FolderPermission.Remark ?? string.Empty,
                        folder.Final_FolderPermission.Created != null && folder.Final_FolderPermission.Created >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.Final_FolderPermission.Created.Value) : DBNull.Value,
                        folder.Final_FolderPermission.CreatedBy ?? string.Empty,
                        folder.Final_FolderPermission.CreatedByLogin ?? string.Empty,
                        folder.Final_FolderPermission.Modified != null && folder.Final_FolderPermission.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.Final_FolderPermission.Modified.Value) : DBNull.Value,
                        folder.Final_FolderPermission.ModifiedBy ?? string.Empty,
                        folder.Final_FolderPermission.ModifiedByLogin ?? string.Empty
                    };

                    folderPermissionsInsertList.Add((parameterNamesFolder, parameterValuesFolder));
                }
                #endregion

                #region Hardware

                var hardwareInsertList = new List<(int itemId, List<string> paramNames, List<object> paramValues)>();

                foreach (var hw in vm.FinalHardwareItems)
                {
                    var parameterNamesHardware = new List<string>
                    {
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.FinalEmployeeDetailsID,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DepartmentID,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DepartmentTitle,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ItemID,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ItemTitle,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.Quantity,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.RemarkHistory,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.Model,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.SerialNumber,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DateAssigned,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.IsReturned,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DateReturned,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.IsReceived,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DateReceived,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.Created,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.CreatedBy,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.CreatedByLogin,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.Modified,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ModifiedBy,
                        ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ModifiedByLogin
                    };

                    var parameterValuesHardware = new List<object>
                    {
                        finalEmpID ?? (object)DBNull.Value,
                        hw.Final_Hardware.DepartmentID ?? (object)DBNull.Value,
                        hw.Final_Hardware.DepartmentTitle ?? string.Empty,
                        hw.Final_Hardware.ItemID,
                        hw.Final_Hardware.ItemTitle ?? string.Empty,
                        hw.Final_Hardware.Quantity,
                        hw.Final_Hardware.RemarkHistory ?? string.Empty,
                        hw.Final_Hardware.Model ?? string.Empty,
                        hw.Final_Hardware.SerialNumber ?? string.Empty,
                        hw.Final_Hardware.DateAssigned != null && hw.Final_Hardware.DateAssigned >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.Final_Hardware.DateAssigned.Value) : DBNull.Value,
                        hw.Final_Hardware.IsReturned,
                        hw.Final_Hardware.DateReturned != null && hw.Final_Hardware.DateReturned >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.Final_Hardware.DateReturned.Value) : DBNull.Value,
                        hw.Final_Hardware.IsReceived,
                        hw.Final_Hardware.DateReceived != null && hw.Final_Hardware.DateReceived >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.Final_Hardware.DateReceived.Value) : DBNull.Value,
                        hw.Final_Hardware.Created != null && hw.Final_Hardware.Created >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.Final_Hardware.Created.Value) : DBNull.Value,
                        hw.Final_Hardware.CreatedBy ?? string.Empty,
                        hw.Final_Hardware.CreatedByLogin ?? string.Empty,
                        hw.Final_Hardware.Modified != null && hw.Final_Hardware.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.Final_Hardware.Modified.Value) : DBNull.Value,
                        hw.Final_Hardware.ModifiedBy ?? string.Empty,
                        hw.Final_Hardware.ModifiedByLogin ?? string.Empty
                    };

                    hardwareInsertList.Add((hw.Final_Hardware.ItemID, parameterNamesHardware, parameterValuesHardware));
                }

                #endregion

                #region Execute insertion

                //Employee Details
                sqlHelper.Update(
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails,
                    parameterNames,
                    parameterValues,
                    ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ID,
                    finalEmpID
                );

                //IT Requirements
                foreach (var (itemId, paramNames, paramValues) in itRequirementsInsertList)
                {
                    if (vm.ViewRequestItem.RequestType != ConstantHelper.RequestForm.RequestType.NewRequestType)
                    {
                        //TODO: use sqlHelper
                        bool exists = ProjectHelper.Exists(
                            ConstantHelper.SQLDataTable.Table.FinalITRequirements,
                            new List<string>
                            {
                                ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ItemID,
                                ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.FinalEmployeeDetailsID
                            },
                            new List<object> { itemId, finalEmpID }
                        );

                        if (exists)
                        {
                            ProjectHelper.UpdateWithFilters(
                                ConstantHelper.SQLDataTable.Table.FinalITRequirements,
                                paramNames,
                                paramValues,
                                new List<string>
                                {
                                    ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.ItemID,
                                    ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.FinalEmployeeDetailsID
                                },
                                new List<object> { itemId, finalEmpID }
                            );
                        }
                        else
                        {
                            sqlHelper.Create(
                                ConstantHelper.SQLDataTable.Table.FinalITRequirements,
                                paramNames,
                                paramValues
                            );
                        }
                    }
                    else
                    {
                        sqlHelper.Create(
                            ConstantHelper.SQLDataTable.Table.FinalITRequirements,
                            paramNames,
                            paramValues
                        );
                    }
                }

                //Folder Permissions
                foreach (var (paramNames, paramValues) in folderPermissionsInsertList)
                {
                    sqlHelper.Create(ConstantHelper.SQLDataTable.Table.FinalFolderPermission, paramNames, paramValues);
                }

                //Hardware
                foreach (var (itemId, paramNames, paramValues) in hardwareInsertList)
                {
                    if (vm.ViewRequestItem.RequestType != ConstantHelper.RequestForm.RequestType.NewRequestType)
                    {
                        //TODO: use sqlHelper
                        bool exists = ProjectHelper.Exists(
                            ConstantHelper.SQLDataTable.Table.FinalHardware,
                            new List<string>
                            {
                                ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ItemID,
                                ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.FinalEmployeeDetailsID
                            },
                            new List<object> { itemId, finalEmpID }
                        );

                        if (exists)
                        {
                            ProjectHelper.UpdateWithFilters(
                                ConstantHelper.SQLDataTable.Table.FinalHardware,
                                paramNames,
                                paramValues,
                                new List<string> { ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ItemID, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.FinalEmployeeDetailsID },
                                new List<object> { itemId, finalEmpID }
                            );
                        }
                        else
                        {
                            sqlHelper.Create(
                                ConstantHelper.SQLDataTable.Table.FinalHardware,
                                paramNames,
                                paramValues
                            );
                        }
                    }
                    else
                    {
                        sqlHelper.Create(
                            ConstantHelper.SQLDataTable.Table.FinalHardware,
                            paramNames,
                            paramValues
                        );
                    }
                }

                vm.HasSuccess = true;
                #endregion

                return vm;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Update
        public bool UpdateRequest(ViewModelRequest vm, bool fromSubmission)
        {
            bool success = false;
            int? finalEmpID = 0;
            if (vm.EmployeeDetails.EmployeeDetailsVM.TempFinalEmployeeDetailsID > 0)
            {
                finalEmpID = vm.EmployeeDetails.EmployeeDetailsVM.TempFinalEmployeeDetailsID;
            }

            try
            {
                if (vm.ID == null)
                    throw new ArgumentException("Request ID cannot be null for update.");

                #region Update Employee Details

                vm.EmployeeDetails.EmployeeDetailsVM.EmployeeStatus = "Active"; //TODO: Temporary here
                List<string> parameterNames = new List<string> {
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.CompanyID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.CompanyTitle,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeName,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeEmail,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeLogin,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeStatus,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DesignationID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DesignationTitle,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.GradeID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.GradeTitle,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DepartmentID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DepartmentTitle,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerEmail,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerLogin,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerName,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.LocationID,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.LocationTitle,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.MobileNo,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ContractOrTemporaryStaff,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.JoinDate,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EndDate,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description1,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description2,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description3,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description4,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description5,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Modified,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ModifiedBy,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ModifiedByLogin,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Remarks
                };

                List<object> parameterValues = new List<object> {
                    vm.EmployeeDetails.EmployeeDetailsVM.CompanyID ?? (object)DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.CompanyTitle ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.EmployeeName ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.EmployeeEmail ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.EmployeeStatus ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.EmployeeID ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.DesignationID ?? (object)DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.DesignationTitle ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.GradeID ?? (object)DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.GradeTitle ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.DepartmentID ?? (object)DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.DepartmentTitle ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerEmail ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerLogin ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerName ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.LocationID ?? (object)DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.LocationTitle ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.MobileNo ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.ContractTemporaryStaff ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.JoinedDate != null && vm.EmployeeDetails.EmployeeDetailsVM.JoinedDate >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.EmployeeDetails.EmployeeDetailsVM.JoinedDate.Value) : DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.EndDate != null && vm.EmployeeDetails.EmployeeDetailsVM.EndDate >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.EmployeeDetails.EmployeeDetailsVM.EndDate.Value) : DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.Description1 ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Description2 ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Description3 ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Description4 ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Description5 ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Modified != null && vm.EmployeeDetails.EmployeeDetailsVM.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.EmployeeDetails.EmployeeDetailsVM.Modified.Value) : DBNull.Value,
                    vm.EmployeeDetails.EmployeeDetailsVM.ModifiedBy ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.ModifiedByLogin ?? string.Empty,
                    vm.EmployeeDetails.EmployeeDetailsVM.Remarks ?? string.Empty,
                };

                // Reaasign to new manager - Termination Request
                if (vm.RequestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType && vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManager != null)
                {
                    parameterNames.Add(ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerName);
                    parameterNames.Add(ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerEmail);
                    parameterNames.Add(ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerLogin);

                    parameterValues.Add(vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManager.Name ?? string.Empty);
                    parameterValues.Add(vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManager.Email ?? string.Empty);
                    parameterValues.Add(vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManager.Login ?? string.Empty);
                }

                // Execute update
                sqlHelper.Update(
                    ConstantHelper.SQLDataTable.Table.EmployeeDetails,
                    parameterNames,
                    parameterValues,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.RequestID,
                    vm.ID.Value
                );

                #endregion

                #region IT Requirements
                RequestDA reqDA = new RequestDA();
                var existingItems = reqDA.RetrieveITRequirementsByRequestID(vm.ID.Value);
                var selectedItems = vm.ITRequirementsVM.SelectedITRequirements;
                var selectedIds = vm.ITRequirementsVM.SelectedITRequirements.Select(x => x.ItemID).ToHashSet();

                foreach (var itReq in selectedItems)
                {
                    //Insert New
                    if (!existingItems.AsEnumerable().Any(e => e.Field<int?>(ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemID) == itReq.ItemID))
                    {
                        var paramNames = new List<string>
                        {
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.RequestID,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemID,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemTitle,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Type,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsAdded,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsRemoved,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateAdded,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateRemoved,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Remark,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Created,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.CreatedBy,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.CreatedByLogin,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Modified,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ModifiedBy,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ModifiedByLogin,
                        };

                        var paramValues = new List<object>
                        {
                            vm.ID ?? (object)DBNull.Value,
                            itReq.ItemID ?? (object)DBNull.Value,
                            itReq.ItemTitle ?? string.Empty,
                            itReq.Type ?? string.Empty,
                            itReq.IsAdded,
                            itReq.IsRemoved,
                            itReq.DateAdded != null && itReq.DateAdded >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.DateAdded.Value) : DBNull.Value,
                            itReq.DateRemoved != null && itReq.DateRemoved >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.DateRemoved.Value) : DBNull.Value,
                            itReq.Remark ?? string.Empty,
                            itReq.Created != null && itReq.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.Modified.Value) : DBNull.Value,
                            itReq.CreatedBy ?? string.Empty,
                            itReq.CreatedByLogin ?? string.Empty,
                            itReq.Modified != null && itReq.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.Modified.Value) : DBNull.Value,
                            itReq.ModifiedBy ?? string.Empty,
                            itReq.ModifiedByLogin ?? string.Empty
                        };

                        sqlHelper.Create(ConstantHelper.SQLDataTable.Table.ITRequirements, paramNames, paramValues);
                    }
                }

                // Update or delete existing
                foreach (DataRow existing in existingItems.Rows)
                {
                    int id = existing[ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ID] != DBNull.Value ? Convert.ToInt32(existing[ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ID]) : 0;
                    int itemId = existing[ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemID] != DBNull.Value ? Convert.ToInt32(existing[ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemID]) : 0;

                    var selected = selectedItems.FirstOrDefault(x => x.ItemID == itemId);

                    if (selected != null)
                    {
                        if (selected.IsRemoved)
                        {
                            // Soft delete = update existing row
                            sqlHelper.Update(
                                ConstantHelper.SQLDataTable.Table.ITRequirements,
                                new List<string>
                                {
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsRemoved,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateRemoved,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsAdded,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateAdded,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Modified,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ModifiedBy,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ModifiedByLogin
                                },
                                new List<object>
                                {
                                    true,
                                    selected.DateRemoved != null && selected.DateRemoved >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(selected.DateRemoved.Value) : DBNull.Value,
                                    false,
                                    DBNull.Value,
                                    selected.Modified!=null && selected.Modified!=DateTime.MinValue ? DateTimeHelper.ConvertToUTCDateTime(selected.Modified.Value) : DateTimeHelper.GetCurrentUtcDateTime(),
                                    selected.ModifiedBy ?? string.Empty,
                                    selected.ModifiedByLogin ?? string.Empty
                                },
                                ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ID,
                                id
                            );
                        }
                        else
                        {
                            // Reactivated (Undo Remove)
                            sqlHelper.Update(
                                ConstantHelper.SQLDataTable.Table.ITRequirements,
                                new List<string>
                                {
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsRemoved,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsAdded,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateAdded,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateRemoved,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Modified,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ModifiedBy,
                                    ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ModifiedByLogin
                                },
                                new List<object>
                                {
                                    false,                     // IsRemoved
                                    true,                      // IsAdded
                                    selected.DateAdded != null && selected.DateAdded >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(selected.DateAdded.Value) : DBNull.Value,
                                    DBNull.Value,              // clear DateRemoved
                                    selected.Modified!=null && selected.Modified!=DateTime.MinValue ? DateTimeHelper.ConvertToUTCDateTime(selected.Modified.Value) : DateTimeHelper.GetCurrentUtcDateTime(),
                                    selected.ModifiedBy ?? string.Empty,
                                    selected.ModifiedByLogin ?? string.Empty
                                },
                                ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ID,
                                id
                            );
                        }
                    }
                    else
                    {
                        reqDA.DeleteByID(ConstantHelper.SQLDataTable.Table.ITRequirements, id);
                    }
                }
                #endregion

                #region Folder Permissions
                foreach (var folder in vm.ITRequirementsVM.FolderPermissions)
                {
                    if (folder.ID > 0) // Existing record
                    {
                        if (folder.IsBeingRemoved) // Only delete if it existed during init and being removed
                        {
                            reqDA.DeleteByID(ConstantHelper.SQLDataTable.Table.FolderPermission, folder.ID);
                        }
                        else if (folder.IsRemoved)
                        {
                            // Soft delete (update only)
                            sqlHelper.Update(
                                ConstantHelper.SQLDataTable.Table.FolderPermission,
                                new List<string>
                                {
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRemoved,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateRemoved,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsAdded,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateAdded,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Modified,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedBy,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedByLogin
                                },
                                new List<object>
                                {
                                    true,
                                    folder.DateRemoved != null && folder.DateRemoved >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.DateRemoved.Value) : DBNull.Value,
                                    false,
                                    DBNull.Value,
                                    folder.Modified != null && folder.Modified != DateTime.MinValue ? DateTimeHelper.ConvertToUTCDateTime(folder.Modified.Value) : DateTimeHelper.GetCurrentUtcDateTime(),
                                    folder.ModifiedBy ?? string.Empty,
                                    folder.ModifiedByLogin ?? string.Empty
                                },
                                ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ID,
                                folder.ID
                            );
                        }
                        else
                        {
                            // Reactivate (Undo Remove)
                            sqlHelper.Update(
                                ConstantHelper.SQLDataTable.Table.FolderPermission,
                                new List<string>
                                {
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRemoved,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsAdded,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateAdded,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateRemoved,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRead,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsWrite,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsDelete,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Modified,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedBy,
                                    ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedByLogin
                                },
                                new List<object>
                                {
                                    false,                      // IsRemoved
                                    true,                       // IsAdded
                                    folder.DateAdded != null && folder.DateAdded >= SqlDateTime.MinValue.Value ?(object) DateTimeHelper.ConvertToUTCDateTime(folder.DateAdded.Value) : DBNull.Value,
                                    DBNull.Value,               // clear DateRemoved
                                    folder.IsRead,
                                    folder.IsWrite,
                                    folder.IsDelete,
                                    folder.Modified!=null && folder.Modified!=DateTime.MinValue ? DateTimeHelper.ConvertToUTCDateTime(folder.Modified.Value) : DateTimeHelper.GetCurrentUtcDateTime(),
                                    folder.ModifiedBy ?? string.Empty,
                                    folder.ModifiedByLogin ?? string.Empty
                                },
                                ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ID,
                                folder.ID
                            );
                        }
                    }
                    else
                    {
                        // New insert
                        var paramNames = new List<string>
                        {
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.RequestID,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.NameOrPath,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRead,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsWrite,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsDelete,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Status,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsAdded,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRemoved,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateAdded,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateRemoved,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Remark,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Created,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.CreatedBy,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.CreatedByLogin,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Modified,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedBy,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedByLogin
                        };

                        var paramValues = new List<object>
                        {
                            vm.ID ?? (object)DBNull.Value,
                            folder.NameOrPath ?? string.Empty,
                            folder.IsRead,
                            folder.IsWrite,
                            folder.IsDelete,
                            folder.Status ?? string.Empty,
                            folder.IsAdded,
                            folder.IsRemoved,
                            folder.DateAdded != null && folder.DateAdded >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.DateAdded.Value) : DBNull.Value,
                            folder.DateRemoved != null && folder.DateRemoved >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.DateRemoved.Value) : DBNull.Value,
                            folder.Remark ?? string.Empty,
                            folder.Created != null && folder.Created >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.Created.Value) : DBNull.Value,
                            folder.CreatedBy ?? string.Empty,
                            folder.CreatedByLogin ?? string.Empty,
                            folder.Modified != null && folder.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.Modified.Value) : DBNull.Value,
                            folder.ModifiedBy ?? string.Empty,
                            folder.ModifiedByLogin ?? string.Empty
                        };

                        sqlHelper.Create(ConstantHelper.SQLDataTable.Table.FolderPermission, paramNames, paramValues);
                    }
                }
                #endregion

                #region Hardware
                foreach (var hw in vm.HardwareVM.SelectedHardwareItems)
                {
                    bool existsInFinal = ProjectHelper.Exists(
                            ConstantHelper.SQLDataTable.Table.FinalHardware,
                            new List<string>
                            {
                                ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ItemID,
                                ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.FinalEmployeeDetailsID
                            },
                            new List<object> { hw.ItemID, finalEmpID }
                        );

                    if (hw.IsTempRemoved)
                    {
                        if (existsInFinal)
                        {
                            // Item exists in FinalHardware but temp removed, still create and mark as returned - Need clarification
                            hw.IsReturned = true;
                            hw.DateReturned = DateTime.Now;
                        }
                        else // Only delete if it existed during init and being removed
                        {
                            if (hw.ID > 0)
                            {
                                reqDA.DeleteByID(ConstantHelper.SQLDataTable.Table.Hardware, (int)hw.ID);
                            }
                            continue;
                        }
                    }

                    var deptText = vm.HardwareVM.DepartmentList.FirstOrDefault(d => int.TryParse(d.Value, out var v)
                                                && hw.DepartmentID.HasValue
                                                && v == hw.DepartmentID.Value)?.Text ?? string.Empty;

                    var paramNames = new List<string>
                    {
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.RequestID,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.DepartmentID,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.DepartmentTitle,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.ItemID,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.ItemTitle,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.Quantity,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.RemarkHistory,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.Model,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.SerialNumber,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.DateAssigned,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.IsReturned,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.DateReturned,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.IsReceived,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.DateReceived,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.Modified,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.ModifiedBy,
                        ConstantHelper.SQLDataTable.Table.HardwareColumns.ModifiedByLogin
                    };

                    var paramValues = new List<object>
                    {
                        vm.ID ?? (object)DBNull.Value,
                        hw.DepartmentID ?? (object)DBNull.Value,
                        deptText,
                        hw.ItemID,
                        hw.ItemTitle ?? string.Empty,
                        hw.Quantity,
                        hw.RemarkHistory ?? string.Empty,
                        hw.Model ?? string.Empty,
                        hw.SerialNumber ?? string.Empty,
                        hw.DateAssigned != null && hw.DateAssigned >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.DateAssigned.Value) : DBNull.Value,
                        hw.IsReturned,
                        hw.DateReturned != null && hw.DateReturned >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.DateReturned.Value) : DBNull.Value,
                        hw.IsReceived,
                        hw.DateReceived != null && hw.DateReceived >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.DateReceived.Value) : DBNull.Value,
                        hw.Modified != null && hw.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(hw.Modified.Value) : DBNull.Value,
                        hw.ModifiedBy ?? string.Empty,
                        hw.ModifiedByLogin ?? string.Empty
                    };

                    if (hw.ID > 0)
                    {
                        sqlHelper.Update(ConstantHelper.SQLDataTable.Table.Hardware, paramNames, paramValues, ConstantHelper.SQLDataTable.Table.HardwareColumns.ID, hw.ID);
                    }
                    else
                    {
                        sqlHelper.Create(ConstantHelper.SQLDataTable.Table.Hardware, paramNames, paramValues);
                    }
                }
                #endregion

                if (fromSubmission)
                {
                    var requestParamNames = new List<string>
                    {
                        ConstantHelper.SQLDataTable.Table.RequestColumns.ReferenceNo,
                        //ConstantHelper.SQLDataTable.Table.RequestColumns.WorkflowStatus,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.ProcessID,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.SubmittedByLogin,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.SubmittedBy,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.Submitted,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyEmployeeDetails,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyITRequirements,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyHardware
                    };

                    var requestParamValues = new List<object>
                    {
                        vm.ReferenceNo ?? string.Empty,
                        //vm.WorkflowStatus ?? string.Empty,
                        vm.ProcessID ?? (object)DBNull.Value,
                        vm.SubmittedByLogin ?? string.Empty,
                        vm.SubmittedBy ?? string.Empty,
                        vm.Submitted != null && vm.Submitted != DateTime.MinValue ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.Submitted) : DBNull.Value,
                        vm.ModifyEmployeeDetails,
                        vm.ModifyITRequirements,
                        vm.ModifyHardware
                    };

                    sqlHelper.Update(
                        ConstantHelper.SQLDataTable.Table.Request,
                        requestParamNames,
                        requestParamValues,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.ID,
                        vm.ID.Value
                    );
                }
                else
                {
                    var requestParamNames = new List<string>
                    {
                        ConstantHelper.SQLDataTable.Table.RequestColumns.WorkflowStatus,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.Changes,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyEmployeeDetails,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyITRequirements,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyHardware
                    };

                    var requestParamValues = new List<object>
                    {
                        vm.WorkflowStatus ?? string.Empty,
                        vm.Changes ?? string.Empty,
                        vm.ModifyEmployeeDetails,
                        vm.ModifyITRequirements,
                        vm.ModifyHardware
                    };

                    sqlHelper.Update(
                        ConstantHelper.SQLDataTable.Table.Request,
                        requestParamNames,
                        requestParamValues,
                        ConstantHelper.SQLDataTable.Table.RequestColumns.ID,
                        vm.ID.Value
                    );
                }

                success = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"UpdateRequest error: {ex.Message}", ex);
            }

            return success;
        }

        public void UpdateRequestRefNo(ViewModelRequest vm)
        {
            var requestParamNames = new List<string>
                    {
                        ConstantHelper.SQLDataTable.Table.RequestColumns.ReferenceNo
                    };

            var requestParamValues = new List<object>
                    {
                        vm.ReferenceNo ?? string.Empty,
                    };

            sqlHelper.Update(
                ConstantHelper.SQLDataTable.Table.Request,
                requestParamNames,
                requestParamValues,
                ConstantHelper.SQLDataTable.Table.RequestColumns.ID,
                vm.ID.Value
            );
        }

        public void UpdateRequestFinalEmployeeID(int finalEmployeeDetailsID, int requestID)
        {
            var requestParamNames = new List<string>
                    {
                        ConstantHelper.SQLDataTable.Table.RequestColumns.FinalEmployeeDetailsID
                    };

            var requestParamValues = new List<object>
                    {
                        finalEmployeeDetailsID
                    };

            sqlHelper.Update(
                ConstantHelper.SQLDataTable.Table.Request,
                requestParamNames,
                requestParamValues,
                ConstantHelper.SQLDataTable.Table.RequestColumns.ID,
                requestID
            );
        }

        //public void UpdateRequestWF(string refNo, List<string> parmNamesList, List<object> parmObjectsList)
        //{
        //    string whereParmName = ConstantHelper.SQLDataTable.Table.RequestColumns.ReferenceNo;
        //    sqlHelper.Update(ConstantHelper.SQLDataTable.Table.Request, parmNamesList, parmObjectsList, whereParmName, refNo);
        //}

        public bool UpdateRequestWF(string curStep, int reqID)
        {
            bool result = false;

            DateTime curDate = DateTimeHelper.GetCurrentUtcDateTime();

            List<string> parameterNames = new List<string>
            {
                ConstantHelper.SQLDataTable.Table.RequestColumns.WorkflowStatus,
                ConstantHelper.SQLDataTable.Table.RequestColumns.Modified
            };

            List<object> parameterValues = new List<object>
            {
                curStep,
                curDate
            };

            result = sqlHelper.Update(ConstantHelper.SQLDataTable.Table.Request, parameterNames, parameterValues, ConstantHelper.SQLDataTable.Table.RequestColumns.ID, reqID);

            return result;
        }

        public bool UpdateApprovalRequest(ViewModelApproval vm, bool isRemarksOnly = false)
        {
            bool success = false;
            try
            {
                if (vm.RequestID == null)
                    throw new ArgumentException("Request ID cannot be null for update.");

                #region Update Employee Details

                List<string> parameterNames = new List<string> {
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Remarks,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Modified,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ModifiedBy,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ModifiedByLogin,
                };

                List<object> parameterValues = new List<object> {
                    vm.EmployeeDetails.EmployeeDetailsVM.Remarks ?? string.Empty,
                    vm.Modified != null && vm.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.Modified.Value) : DBNull.Value,
                    vm.ModifiedBy ?? string.Empty,
                    vm.ModifiedByLogin ?? string.Empty
                };

                if (vm.ViewRequestItem.WorkflowStatus == ConstantHelper.WorkflowStatus.PENDING_INFRA_TEAM_ACTION)
                {
                    parameterNames.Add(ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeName);
                    parameterNames.Add(ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeEmail);
                    parameterNames.Add(ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeLogin);

                    parameterValues.Add(vm.EmployeeDetails.EmployeeDetailsVM.EmployeeName ?? string.Empty);
                    parameterValues.Add(vm.EmployeeDetails.EmployeeDetailsVM.EmployeeEmail ?? string.Empty);
                    parameterValues.Add(vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin ?? string.Empty);
                }

                sqlHelper.Update(
                    ConstantHelper.SQLDataTable.Table.EmployeeDetails,
                    parameterNames,
                    parameterValues,
                    ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.RequestID,
                    vm.RequestID
                );

                #endregion

                if (!isRemarksOnly)
                {
                    #region IT Requirements
                    foreach (var itReq in vm.ITRequirements)
                    {
                        var paramNames = new List<string>
                        {
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsAdded,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsRemoved,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateAdded,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateRemoved,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Remark,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Modified,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ModifiedBy,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ModifiedByLogin,
                        };

                        var paramValues = new List<object>
                        {
                            itReq.ITRequirementsVM.IsAdded,
                            itReq.ITRequirementsVM.IsRemoved,
                            itReq.ITRequirementsVM.DateAdded != null && itReq.ITRequirementsVM.DateAdded >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.ITRequirementsVM.DateAdded.Value) : DBNull.Value,
                            itReq.ITRequirementsVM.DateRemoved != null && itReq.ITRequirementsVM.DateRemoved >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(itReq.ITRequirementsVM.DateRemoved.Value) : DBNull.Value,
                            itReq.ITRequirementsVM.Remark ?? string.Empty,
                            vm.Modified != null && vm.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.Modified.Value) : DBNull.Value,
                            vm.ModifiedBy ?? string.Empty,
                            vm.ModifiedByLogin ?? string.Empty
                        };

                        sqlHelper.Update(
                            ConstantHelper.SQLDataTable.Table.ITRequirements,
                            paramNames, paramValues,
                            ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ID,
                            itReq.ITRequirementsVM.ID);
                    }
                    #endregion

                    #region Folder Permissions

                    foreach (var folder in vm.FolderPermissions)
                    {
                        var paramNamesFolder = new List<string>
                        {
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsAdded,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRemoved,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateAdded,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateRemoved,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Remark,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Modified,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedBy,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedByLogin
                        };

                        var paramValuesFolder = new List<object>
                        {
                            folder.IsAdded,
                            folder.IsRemoved,
                            folder.DateAdded != null && folder.DateAdded >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.DateAdded.Value) : DBNull.Value,
                            folder.DateRemoved != null && folder.DateRemoved >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(folder.DateRemoved.Value) : DBNull.Value,
                            folder.Remark ?? string.Empty,
                            vm.Modified != null && vm.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.Modified.Value) : DBNull.Value,
                            vm.ModifiedBy ?? string.Empty,
                            vm.ModifiedByLogin ?? string.Empty
                        };

                        sqlHelper.Update(
                            ConstantHelper.SQLDataTable.Table.FolderPermission,
                            paramNamesFolder, paramValuesFolder,
                            ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ID,
                            folder.ID);
                    }
                    #endregion

                    if (vm.ViewRequestItem.WorkflowStatus == ConstantHelper.WorkflowStatus.PENDING_DEPARTMENT_ADMIN_ACTION)
                    {
                        #region Hardware
                        foreach (var hw in vm.HardwareItems)
                        {
                            if(!hw.HardwareVM.IsReturned)
                            {
                                var paramNamesHardware = new List<string>
                            {
                                ConstantHelper.SQLDataTable.Table.HardwareColumns.DateAssigned,
                                ConstantHelper.SQLDataTable.Table.HardwareColumns.IsReturned,
                                ConstantHelper.SQLDataTable.Table.HardwareColumns.DateReturned,
                                ConstantHelper.SQLDataTable.Table.HardwareColumns.Model,
                                ConstantHelper.SQLDataTable.Table.HardwareColumns.SerialNumber,
                                ConstantHelper.SQLDataTable.Table.HardwareColumns.RemarkHistory,
                                ConstantHelper.SQLDataTable.Table.HardwareColumns.Modified,
                                ConstantHelper.SQLDataTable.Table.HardwareColumns.ModifiedBy,
                                ConstantHelper.SQLDataTable.Table.HardwareColumns.ModifiedByLogin
                            };

                                var paramValuesHardware = new List<object>
                            {
                                hw.HardwareVM.DateAssigned != null && vm.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.Modified.Value) : DBNull.Value,
                                hw.HardwareVM.PendingIsReturned,
                                hw.HardwareVM.PendingDateReturned != null && hw.HardwareVM.PendingDateReturned >= SqlDateTime.MinValue.Value ? (object)hw.HardwareVM.PendingDateReturned : DBNull.Value,
                                hw.HardwareVM.Model ?? string.Empty,
                                hw.HardwareVM.SerialNumber ?? string.Empty,
                                hw.HardwareVM.Remarks ?? string.Empty,
                                vm.Modified != null && vm.Modified >= SqlDateTime.MinValue.Value ? (object)DateTimeHelper.ConvertToUTCDateTime(vm.Modified.Value) : DBNull.Value,
                                vm.ModifiedBy ?? string.Empty,
                                vm.ModifiedByLogin ?? string.Empty
                            };

                                sqlHelper.Update(
                                    ConstantHelper.SQLDataTable.Table.Hardware,
                                    paramNamesHardware, paramValuesHardware,
                                    ConstantHelper.SQLDataTable.Table.HardwareColumns.ID,
                                    hw.HardwareVM.ID);
                            }
                        }
                        #endregion
                    }
                }

                success = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"UpdateRequest error: {ex.Message}", ex);
            }

            return success;
        }

        public void UpdateRMEscalationFlagForRequest(int reqID, bool isEscalated)
        {
            var requestParamNames = new List<string>
            {
                ConstantHelper.SQLDataTable.Table.RequestColumns.RMEscalation
            };

            var requestParamValues = new List<object>
            {
                isEscalated
            };

            sqlHelper.Update(
                ConstantHelper.SQLDataTable.Table.Request,
                requestParamNames,
                requestParamValues,
                ConstantHelper.SQLDataTable.Table.RequestColumns.ID,
                reqID
            );
        }

        public bool UpdateNewReportingManager(ViewModelApproval vm, DataTable subordinatesDt)
        {
            bool success = false;

            try
            {
                if (subordinatesDt != null && subordinatesDt.Rows.Count > 0)
                {
                    foreach (DataRow row in subordinatesDt.Rows)
                    {
                        string employeeLogin = vm.FinalEmployeeDetails.Final_EmployeeDetails.EmployeeLogin;

                        DateTime curDateTime = DateTimeHelper.GetCurrentUtcDateTime();

                        List<string> updateFields = new List<string>
                        {
                            ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerLogin,
                            ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerName,
                            ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerEmail,
                            ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.Modified,
                            ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ModifiedBy,
                            ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ModifiedByLogin
                        };

                        List<object> updateValues = new List<object>
                        {
                            vm.FinalEmployeeDetails.Final_EmployeeDetails.NewReportingManagerLogin,
                            vm.FinalEmployeeDetails.Final_EmployeeDetails.NewReportingManagerName,
                            vm.FinalEmployeeDetails.Final_EmployeeDetails.NewReportingManagerEmail,
                            vm.FinalEmployeeDetails.Final_EmployeeDetails.Modified ?? curDateTime,
                            vm.FinalEmployeeDetails.Final_EmployeeDetails.ModifiedBy ?? string.Empty,
                            vm.FinalEmployeeDetails.Final_EmployeeDetails.ModifiedByLogin ?? string.Empty
                        };

                        sqlHelper.Update(
                            ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails,
                            updateFields,
                            updateValues,
                            ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerLogin,
                            employeeLogin
                        );
                    }
                }
                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"UpdateNewReportingManager error: {ex.Message}", ex);
            }
        }
        #endregion

        #region Retrieve
        public DataTable RetrieveRequestByID(int requestID)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.RequestColumns.ID };
            List<object> paramValues = new List<object>() { requestID };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.Request, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveRequestByRequestID(int requestID)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.RequestColumns.ID };
            List<object> paramValues = new List<object>() { requestID };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.Request, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveEmployeeDetailsByRequestID(int requestID)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.RequestID };
            List<object> paramValues = new List<object>() { requestID };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.EmployeeDetails, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveHardwareByRequestID(int requestID)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.HardwareColumns.RequestID };
            List<object> paramValues = new List<object>() { requestID };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.Hardware, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveITRequirementsByRequestID(int requestID)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.RequestID };
            List<object> paramValues = new List<object>() { requestID };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.ITRequirements, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveFolderPermissionByRequestID(int requestID)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.RequestID };
            List<object> paramValues = new List<object>() { requestID };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.FolderPermission, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveChangesByRequestID(int requestID)
        {
            List<string> columnNames = new List<string>() { ConstantHelper.SQLDataTable.Table.RequestColumns.Changes };
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.RequestColumns.ID };
            List<object> paramValues = new List<object>() { requestID };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.Request, columnNames, paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveFinalEmployeeDetailsByLogin(string employeeLogin)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.EmployeeLogin };
            List<object> paramValues = new List<object>() { employeeLogin };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveFinalITRequirementsByFinalEmpID(string finalEmployeeID)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.FinalEmployeeDetailsID };
            List<object> paramValues = new List<object>() { finalEmployeeID };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.FinalITRequirements, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveFinalFolderPermissionByFinalEmpID(string finalEmployeeID)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.FinalEmployeeDetailsID };
            List<object> paramValues = new List<object>() { finalEmployeeID };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.FinalFolderPermission, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveFinalHardwareByFinalEmpID(string finalEmployeeID)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.FinalEmployeeDetailsID };
            List<object> paramValues = new List<object>() { finalEmployeeID };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.FinalHardware, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveRequestByFinalEmpID(string finalEmployeeID)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.RequestColumns.FinalEmployeeDetailsID };
            List<object> paramValues = new List<object>() { finalEmployeeID };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.Request, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveFinalEmployeeDetailsByRepManagerLogin(string employeeLogin)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ReportingManagerLogin };
            List<object> paramValues = new List<object>() { employeeLogin };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, new List<string>(), paramNames, paramValues);

            return dt;
        }
        #endregion

        #region Delete
        public bool DeleteByRequestID(string tableName, int requestID)
        {
            List<string> fieldFilters = new List<string> { "RequestID" };
            List<object> fieldValues = new List<object> { requestID };

            return sqlHelper.Delete(tableName, fieldFilters, fieldValues);
        }

        public bool DeleteByID(string tableName, int ID)
        {
            List<string> fieldFilters = new List<string> { "ID" };
            List<object> fieldValues = new List<object> { ID };

            return sqlHelper.Delete(tableName, fieldFilters, fieldValues);
        }
        #endregion

        public ViewModelRequestListing SearchRequestListing(ViewModelRequestListing vm)
        {
            try
            {
                List<RequestListingData> reqListingObj = new();

                string workflowStatuses = string.Join(",", vm.searchModel.WorkflowStatus);

                string departments = string.Join(",", vm.searchModel.Department);

                string designations = string.Join(",", vm.searchModel.Designation);

                string requestType = string.Join(",", vm.searchModel.RequestType);

                string managerInchargeDepts = string.Join(",", vm.AccessDepartments);

                List<string> paramNames = new string[]
                {
                        ConstantHelper.StoreProcedureParameter.RequestListing.PageNumber,
                        ConstantHelper.StoreProcedureParameter.RequestListing.RowsPerPage,
                        ConstantHelper.StoreProcedureParameter.RequestListing.ReferenceNo,
                        ConstantHelper.StoreProcedureParameter.RequestListing.EmployeeName,
                        ConstantHelper.StoreProcedureParameter.RequestListing.EmployeeID,
                        ConstantHelper.StoreProcedureParameter.RequestListing.SubmittedBy,
                        ConstantHelper.StoreProcedureParameter.RequestListing.Designation,
                        ConstantHelper.StoreProcedureParameter.RequestListing.Department,
                        ConstantHelper.StoreProcedureParameter.RequestListing.CreatedStartDate,
                        ConstantHelper.StoreProcedureParameter.RequestListing.CreatedEndDate,
                        ConstantHelper.StoreProcedureParameter.RequestListing.SubmittedStartDate,
                        ConstantHelper.StoreProcedureParameter.RequestListing.SubmittedEndDate,
                        ConstantHelper.StoreProcedureParameter.RequestListing.WorkflowStatus,
                        ConstantHelper.StoreProcedureParameter.RequestListing.RequestType,
                        ConstantHelper.StoreProcedureParameter.RequestListing.SortField,
                        ConstantHelper.StoreProcedureParameter.RequestListing.SortFieldTable,
                        ConstantHelper.StoreProcedureParameter.RequestListing.SortDirection,
                        ConstantHelper.StoreProcedureParameter.RequestListing.ManagerInChargeDepartments,
                        ConstantHelper.StoreProcedureParameter.RequestListing.MemberLogin,
                        ConstantHelper.StoreProcedureParameter.RequestListing.CreatedByLogin
                }.ToList();
                List<object> paramValues = new object[]
                {
                        vm.PageNumber,
                        vm.RowsPerPage,
                        vm.searchModel.ReferenceNo,
                        vm.searchModel.EmployeeName,
                        vm.searchModel.EmployeeId,
                        vm.searchModel.SubmittedBy,
                        designations,
                        departments,
                        vm.searchModel.CreatedStartDate != null && vm.searchModel.CreatedStartDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.CreatedStartDate.Value):DBNull.Value,
                        vm.searchModel.CreatedEndDate != null && vm.searchModel.CreatedEndDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.CreatedEndDate.Value):DBNull.Value,
                        vm.searchModel.SubmittedStartDate != null && vm.searchModel.SubmittedStartDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.SubmittedStartDate.Value):DBNull.Value,
                        vm.searchModel.SubmittedEndDate != null && vm.searchModel.SubmittedEndDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.SubmittedEndDate.Value):DBNull.Value,
                        workflowStatuses,
                        requestType,
                        vm.SortField,
                        vm.SortFieldTable,
                        vm.SortDirection,
                        managerInchargeDepts,
                        vm.MemberLogin,
                        vm.CurrentUserLogin
                }.ToList();

                DataTable myActiveRequestDataTable = sqlHelper.SQLExecuteAsDataSet(false, ConstantHelper.StoreProcedureName.SearchRequestListing, paramNames, paramValues);

                List<string> countParamNames = new string[]
                {
                        ConstantHelper.StoreProcedureParameter.RequestListing.ReferenceNo,
                        ConstantHelper.StoreProcedureParameter.RequestListing.EmployeeName,
                        ConstantHelper.StoreProcedureParameter.RequestListing.EmployeeID,
                        ConstantHelper.StoreProcedureParameter.RequestListing.SubmittedBy,
                        ConstantHelper.StoreProcedureParameter.RequestListing.Designation,
                        ConstantHelper.StoreProcedureParameter.RequestListing.Department,
                        ConstantHelper.StoreProcedureParameter.RequestListing.CreatedStartDate,
                        ConstantHelper.StoreProcedureParameter.RequestListing.CreatedEndDate,
                        ConstantHelper.StoreProcedureParameter.RequestListing.SubmittedStartDate,
                        ConstantHelper.StoreProcedureParameter.RequestListing.SubmittedEndDate,
                        ConstantHelper.StoreProcedureParameter.RequestListing.WorkflowStatus,
                        ConstantHelper.StoreProcedureParameter.RequestListing.RequestType,
                        ConstantHelper.StoreProcedureParameter.RequestListing.ManagerInChargeDepartments,
                        ConstantHelper.StoreProcedureParameter.RequestListing.MemberLogin,
                        ConstantHelper.StoreProcedureParameter.RequestListing.CreatedByLogin
                }.ToList();
                List<object> countParamValues = new object[]
                {
                        vm.searchModel.ReferenceNo,
                        vm.searchModel.EmployeeName,
                        vm.searchModel.EmployeeId,
                        vm.searchModel.SubmittedBy,
                        designations,
                        departments,
                        vm.searchModel.CreatedStartDate != null && vm.searchModel.CreatedStartDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.CreatedStartDate.Value):DBNull.Value,
                        vm.searchModel.CreatedEndDate != null && vm.searchModel.CreatedEndDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.CreatedEndDate.Value):DBNull.Value,
                        vm.searchModel.SubmittedStartDate != null && vm.searchModel.SubmittedStartDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.SubmittedStartDate.Value):DBNull.Value,
                        vm.searchModel.SubmittedEndDate != null && vm.searchModel.SubmittedEndDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.SubmittedEndDate.Value):DBNull.Value,
                        workflowStatuses,
                        requestType,
                        managerInchargeDepts,
                        vm.MemberLogin,
                        vm.CurrentUserLogin
                }.ToList();

                int totalRows = sqlHelper.SQLExecuteScalar(false, ConstantHelper.StoreProcedureName.GetSearchRequestListingCount, countParamNames, countParamValues);
                vm.Count = totalRows;


                foreach (DataRow row in myActiveRequestDataTable.Rows)
                {
                    RequestListingData obj = new();
                    obj = ConvertEntitiesHelper.ConvertRequestListingObj(row);

                    reqListingObj.Add(obj);
                }

                vm.RequestListing = reqListingObj;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return vm;
        }
    }
}

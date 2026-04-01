using MHA.Framework.Core.SP;
using MHA.ELAN.Data;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using MHA.ELAN.Framework.JSONConstants;
using Microsoft.SharePoint.Client;
using System.Data;
using FieldHelper = MHA.Framework.Core.General.FieldHelper;
using FieldHelperSP = MHA.Framework.Core.SP.FieldHelper;

namespace MHA.ELAN.Business
{
    public class RequestBL
    {
        private static readonly JSONAppSettings appSettings;

        static RequestBL()
        {
            appSettings = ConfigurationManager.GetAppSetting();

        }

        #region Init

        public ViewModelRequest InitNewRequestForm(ViewModelRequest vm, string spHostURL, string accessToken)
        {
            try
            {
                using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(ctx);
                    bool isAuthorized = SharePointHelper.IsUserInGroup(currentUser, ConstantHelper.SPSecurityGroup.ElanMember);

                    if (isAuthorized)
                    {
                        if (vm.ID > 0)
                        {
                            vm = InitExistingRequestForm(vm, ctx);
                        }
                        else if (vm.ID == null)
                        {
                            vm = InitNewRequestFormFields(vm, ctx);
                        }
                    }
                    else
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = string.Format(ConstantHelper.ErrorMessage.NoAuthorized, ConstantHelper.PermissionConfigFunction.Request.CreateNewRequest);
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return vm;
        }

        private ViewModelRequest InitExistingRequestForm(ViewModelRequest vm, ClientContext ctx)
        {
            int changeRequestID = (int)vm.ID;
            RequestDA da = new RequestDA();

            // Load request data
            DataTable requestDT = da.RetrieveRequestByID(changeRequestID);
            if (requestDT.Rows.Count > 0)
            {
                DataRow row = requestDT.Rows[0];
                vm.ReferenceNo = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.RequestColumns.ReferenceNo);
                vm.WorkflowStatus = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.RequestColumns.WorkflowStatus);
                vm.RequestType = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.RequestColumns.RequestType);
                vm.ModifyEmployeeDetails = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyEmployeeDetails);
                vm.ModifyITRequirements = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyITRequirements);
                vm.ModifyHardware = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.RequestColumns.ModifyHardware);
            }

            //TODO: Use ConvertEntities (low priority)
            #region Employee Details
            DataTable empDT = da.RetrieveEmployeeDetailsByRequestID(changeRequestID);
            if (empDT.Rows.Count > 0)
            {
                DataRow row = empDT.Rows[0];
                vm.EmployeeDetails.EmployeeDetailsVM.EmployeeName = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeName);
                vm.EmployeeDetails.EmployeeDetailsVM.EmployeeID = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeID);
                vm.EmployeeDetails.EmployeeDetailsVM.MobileNo = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.MobileNo);
                vm.EmployeeDetails.EmployeeDetailsVM.Description1 = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description1);
                vm.EmployeeDetails.EmployeeDetailsVM.Description2 = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description2);
                vm.EmployeeDetails.EmployeeDetailsVM.Description3 = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description3);
                vm.EmployeeDetails.EmployeeDetailsVM.Description4 = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description4);
                vm.EmployeeDetails.EmployeeDetailsVM.Description5 = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Description5);
                vm.EmployeeDetails.EmployeeDetailsVM.Remarks = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.Remarks);
                vm.EmployeeDetails.EmployeeDetailsVM.CompanyID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.CompanyID);
                vm.EmployeeDetails.EmployeeDetailsVM.DesignationID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DesignationID);
                vm.EmployeeDetails.EmployeeDetailsVM.GradeID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.GradeID);
                vm.EmployeeDetails.EmployeeDetailsVM.DepartmentID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.DepartmentID);
                vm.EmployeeDetails.EmployeeDetailsVM.LocationID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.LocationID);
                vm.EmployeeDetails.EmployeeDetailsVM.ContractTemporaryStaff = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ContractOrTemporaryStaff);
                DateTime joinDate = FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.JoinDate);
                vm.EmployeeDetails.EmployeeDetailsVM.JoinedDate = joinDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(joinDate) : null;

                DateTime tempEndDate = FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EndDate);
                vm.EmployeeDetails.EmployeeDetailsVM.EndDate = tempEndDate != DateTime.MinValue ? DateTimeHelper.ConvertToLocalDateTime(tempEndDate) : null;

                //Reporting manager
                vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerLogin = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerLogin);
                vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerName = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerName);
                vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerEmail = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ReportingManagerEmail);

                if (!string.IsNullOrEmpty(vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerLogin))
                {
                    vm.EmployeeDetails.EmployeeDetailsVM.ReportingManager = new PeoplePickerUser
                    {
                        Login = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerLogin,
                        Name = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerName,
                        Email = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerEmail
                    };
                }

                //Employee
                vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeLogin);
                vm.EmployeeDetails.EmployeeDetailsVM.EmployeeEmail = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.EmployeeEmail);

                if (!string.IsNullOrEmpty(vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin))
                {
                    vm.EmployeeDetails.EmployeeDetailsVM.Employee = new PeoplePickerUser
                    {
                        Login = vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin,
                        Name = vm.EmployeeDetails.EmployeeDetailsVM.EmployeeName,
                        Email = vm.EmployeeDetails.EmployeeDetailsVM.EmployeeEmail
                    };
                }

                //New Reporting manager
                vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManagerLogin = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerLogin);
                vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManagerName = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerName);
                vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManagerEmail = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.NewReportingManagerEmail);

                if (!string.IsNullOrEmpty(vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManagerLogin))
                {
                    vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManager = new PeoplePickerUser
                    {
                        Login = vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManagerLogin,
                        Name = vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManagerName,
                        Email = vm.EmployeeDetails.EmployeeDetailsVM.NewReportingManagerEmail
                    };
                }
            }

            if (vm.RequestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType)
            {
                GetSubordinatesByRepManagerLogin(vm);
            }

            // Populate dropdowns
            ListItemCollection companyListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Company, string.Empty, null);
            ListItemCollection designationListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Designation, string.Empty, null);
            ListItemCollection gradeListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Grade, string.Empty, null);
            ListItemCollection departmentListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Department, string.Empty, null);
            ListItemCollection locationListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Location, string.Empty, null);

            vm.EmployeeDetails.EmployeeDetailsVM.CompanyList = ProjectHelper.GetListItemsAsDDLItems(companyListItem, ConstantHelper.SPColumn.Company.Title, ConstantHelper.SPColumn.Status);
            vm.EmployeeDetails.EmployeeDetailsVM.DesignationList = ProjectHelper.GetListItemsAsDDLItems(designationListItem, ConstantHelper.SPColumn.Designation.Title, ConstantHelper.SPColumn.Status);
            vm.EmployeeDetails.EmployeeDetailsVM.GradeList = ProjectHelper.GetListItemsAsDDLItems(gradeListItem, ConstantHelper.SPColumn.Grade.Title, ConstantHelper.SPColumn.Status);
            vm.EmployeeDetails.EmployeeDetailsVM.DepartmentList = ProjectHelper.GetListItemsAsDDLItems(departmentListItem, ConstantHelper.SPColumn.Department.Title, ConstantHelper.SPColumn.Status);
            vm.EmployeeDetails.EmployeeDetailsVM.LocationList = ProjectHelper.GetListItemsAsDDLItems(locationListItem, ConstantHelper.SPColumn.Location.Title, ConstantHelper.SPColumn.Status);

            // Set selected text values
            vm.EmployeeDetails.EmployeeDetailsVM.CompanyTitle = GetSelectedText(vm.EmployeeDetails.EmployeeDetailsVM.CompanyList, vm.EmployeeDetails.EmployeeDetailsVM.CompanyID);
            vm.EmployeeDetails.EmployeeDetailsVM.DesignationTitle = GetSelectedText(vm.EmployeeDetails.EmployeeDetailsVM.DesignationList, vm.EmployeeDetails.EmployeeDetailsVM.DesignationID);
            vm.EmployeeDetails.EmployeeDetailsVM.GradeTitle = GetSelectedText(vm.EmployeeDetails.EmployeeDetailsVM.GradeList, vm.EmployeeDetails.EmployeeDetailsVM.GradeID);
            vm.EmployeeDetails.EmployeeDetailsVM.DepartmentTitle = GetSelectedText(vm.EmployeeDetails.EmployeeDetailsVM.DepartmentList, vm.EmployeeDetails.EmployeeDetailsVM.DepartmentID);
            vm.EmployeeDetails.EmployeeDetailsVM.LocationTitle = GetSelectedText(vm.EmployeeDetails.EmployeeDetailsVM.LocationList, vm.EmployeeDetails.EmployeeDetailsVM.LocationID);

            #region Drop down list from constants

            string selectedRequestType = vm.RequestType;
            string selectedContractType = vm.EmployeeDetails.EmployeeDetailsVM.ContractTemporaryStaff;

            vm.RequestTypeList = new List<DropDownListItem>
            {
                new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.NewRequestType, Value = ConstantHelper.RequestForm.RequestType.NewRequestType },
                new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.ModificationRequestType, Value = ConstantHelper.RequestForm.RequestType.ModificationRequestType },
                new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.TerminationRequestType, Value = ConstantHelper.RequestForm.RequestType.TerminationRequestType },
                new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.TransferRequestType, Value = ConstantHelper.RequestForm.RequestType.TransferRequestType },
                new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.PromotionRequestType, Value = ConstantHelper.RequestForm.RequestType.PromotionRequestType }
            }.Select(item =>
            {
                item.Selected = item.Value == selectedRequestType;
                return item;
            }).ToList();

            vm.EmployeeDetails.EmployeeDetailsVM.ContractTypeList = new List<DropDownListItem>
            {
                new DropDownListItem { Text = ConstantHelper.RequestForm.ContractType.Permanent, Value = ConstantHelper.RequestForm.ContractType.Permanent },
                new DropDownListItem { Text = ConstantHelper.RequestForm.ContractType.Temporary, Value = ConstantHelper.RequestForm.ContractType.Temporary }
            }.Select(item =>
            {
                item.Selected = item.Value == selectedContractType;
                return item;
            }).ToList();

            #endregion
            #endregion

            #region IT Requirements List

            vm.ITRequirementsVM.InfraOptions.Clear();
            vm.ITRequirementsVM.ApplicationOptions.Clear();

            var itItems = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.ITRequirements, string.Empty, null);
            var selectedIT = da.RetrieveITRequirementsByRequestID(changeRequestID);
            vm.ITRequirementsVM.SelectedITRequirements = selectedIT.AsEnumerable()
                        .Select(row => new ITRequirements
                        {
                            ID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ID),
                            ItemID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemID),
                            ItemTitle = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemTitle),
                            Type = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Type),
                            IsAdded = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsAdded),
                            IsRemoved = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsRemoved),
                            Remark = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Remark),
                            DateAdded = ProjectHelper.NormalizeDate(FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateAdded)),
                            DateRemoved = ProjectHelper.NormalizeDate(FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateRemoved))
                        })
                        .ToList();
            var selectedItemIDs = selectedIT.AsEnumerable().Select(row => FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemID)).ToList();

            foreach (ListItem li in itItems)
            {
                int itemId = li.Id;
                string title = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.ITRequirements.Title);
                string itemType = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.ITRequirements.ItemType);

                var designationLookup = li[ConstantHelper.SPColumn.ITRequirements.Designation] as FieldLookupValue;
                int designationId = designationLookup?.LookupId ?? 0;

                var option = new ITOption
                {
                    ItemID = itemId,
                    Name = title,
                    DesignationID = designationId,
                    IsSelected = selectedItemIDs.Contains(itemId)
                };

                if (string.Equals(itemType, ConstantHelper.RequestForm.ITRequirementsType.Infra, StringComparison.OrdinalIgnoreCase))
                    vm.ITRequirementsVM.InfraOptions.Add(option);
                else if (string.Equals(itemType, ConstantHelper.RequestForm.ITRequirementsType.Applications, StringComparison.OrdinalIgnoreCase))
                    vm.ITRequirementsVM.ApplicationOptions.Add(option);
            }

            #endregion

            #region Folder Permission
            vm.ITRequirementsVM.FolderPermissions.Clear();

            DataTable folderDT = da.RetrieveFolderPermissionByRequestID(changeRequestID);
            foreach (DataRow row in folderDT.Rows)
            {
                var permission = new FolderPermission
                {
                    ID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ID),
                    RequestID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.RequestID),
                    NameOrPath = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.NameOrPath),
                    IsRead = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRead),
                    IsWrite = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsWrite),
                    IsDelete = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsDelete),
                    Status = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Status),
                    IsAdded = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsAdded),
                    IsRemoved = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRemoved),
                    DateAdded = FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateAdded),
                    DateRemoved = FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.DateRemoved),
                    Remark = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Remark),
                    Created = FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Created),
                    CreatedBy = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.CreatedBy),
                    CreatedByLogin = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.CreatedByLogin),
                    Modified = FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.Modified),
                    ModifiedBy = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedBy),
                    ModifiedByLogin = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.ModifiedByLogin),

                    OriginalRead = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsRead),
                    OriginalWrite = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsWrite),
                    OriginalDelete = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.FolderPermissionColumns.IsDelete)
                };

                vm.ITRequirementsVM.FolderPermissions.Add(permission);
            }

            #endregion

            #region Hardware List
            vm.HardwareVM.DepartmentList = ProjectHelper.GetListItemsAsDDLItems(departmentListItem, ConstantHelper.SPColumn.Department.Title, ConstantHelper.SPColumn.Status);
            var allHardwareRaw = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Hardware, string.Empty, null);
            var allHardware = allHardwareRaw.Cast<ListItem>().ToList();

            //filter active department
            var activeDepartments = departmentListItem.Cast<ListItem>()
                .Where(d => FieldHelperSP
                    .GetFieldValueAsString(d, ConstantHelper.SPColumn.Department.Status)
                    .Equals(ConstantHelper.ItemStatus.Active, StringComparison.OrdinalIgnoreCase))
                .Select(d => d.Id)
                .ToHashSet();

            var activeHardware = allHardware
                .Where(li =>
                {
                    var hwStatus = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.Hardware.Status);
                    var deptLookup = li[ConstantHelper.SPColumn.Hardware.Department] as FieldLookupValue;

                    return hwStatus.Equals(ConstantHelper.ItemStatus.Active, StringComparison.OrdinalIgnoreCase)
                           && deptLookup != null
                           && activeDepartments.Contains(deptLookup.LookupId);
                })
                .ToList();

            var hdMappings = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Hardware_Designation, string.Empty, null);

            // Populate HardwareDefinitions
            vm.HardwareVM.HardwareDefinitions.Clear();

            foreach (ListItem map in hdMappings)
            {
                var hwLookup = map[ConstantHelper.SPColumn.Hardware_Designation.Item] as FieldLookupValue;
                var desigLookup = map[ConstantHelper.SPColumn.Hardware_Designation.Designation] as FieldLookupValue;
                if (hwLookup == null) continue;

                var master = activeHardware.FirstOrDefault(m => m.Id == hwLookup.LookupId);
                if (master == null) continue;

                var title = FieldHelperSP.GetFieldValueAsString(master, ConstantHelper.SPColumn.Hardware.Title);
                var masterDeptLookup = master[ConstantHelper.SPColumn.Hardware.Department] as FieldLookupValue;

                var deptId = masterDeptLookup?.LookupId ?? 0;
                var deptTitle = "";
                if (masterDeptLookup != null)
                {
                    var deptItem = departmentListItem.Cast<ListItem>()
                                    .FirstOrDefault(d => d.Id == masterDeptLookup.LookupId);
                    deptTitle = FieldHelperSP.GetFieldValueAsString(deptItem, ConstantHelper.SPColumn.Department.Title);
                }

                vm.HardwareVM.HardwareDefinitions.Add(new HardwareDefinition
                {
                    ItemID = master.Id,
                    ItemTitle = title,
                    DepartmentID = deptId,
                    DepartmentTitle = deptTitle,
                    DesignationID = desigLookup?.LookupId ?? 0
                });
            }

            // Include unmapped hardware
            foreach (var master in activeHardware)
            {
                if (vm.HardwareVM.HardwareDefinitions.Any(d => d.ItemID == master.Id))
                    continue;

                var title = FieldHelperSP.GetFieldValueAsString(master, ConstantHelper.SPColumn.Hardware.Title);
                var masterDeptLookup = master[ConstantHelper.SPColumn.Hardware.Department] as FieldLookupValue;

                var deptId = masterDeptLookup?.LookupId ?? 0;
                var deptTitle = "";
                if (masterDeptLookup != null)
                {
                    var deptItem = departmentListItem.Cast<ListItem>()
                                    .FirstOrDefault(d => d.Id == masterDeptLookup.LookupId);
                    deptTitle = FieldHelperSP.GetFieldValueAsString(deptItem, ConstantHelper.SPColumn.Department.Title);
                }

                vm.HardwareVM.HardwareDefinitions.Add(new HardwareDefinition
                {
                    ItemID = master.Id,
                    ItemTitle = title,
                    DepartmentID = deptId,
                    DepartmentTitle = deptTitle,
                    DesignationID = 0
                });
            }

            // Populate dropdown
            vm.HardwareVM.HardwareItemList = activeHardware
                .Select(li => new DropDownListItem
                {
                    Id = li.Id.ToString(),
                    Text = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.Hardware.Title),
                    Value = li.Id.ToString()
                })
                .ToList();

            var assignedHardwareDT = da.RetrieveHardwareByRequestID(changeRequestID);

            vm.HardwareVM.AssignedItems = assignedHardwareDT.AsEnumerable()
                .Select(row => new HardwareItemVM
                {
                    ID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.HardwareColumns.ID),
                    DepartmentID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.HardwareColumns.DepartmentID),
                    ItemID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.HardwareColumns.ItemID),
                    Quantity = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.HardwareColumns.Quantity),
                    Remarks = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.HardwareColumns.RemarkHistory),
                    DesignationID = vm.EmployeeDetails.EmployeeDetailsVM.DesignationID
                })
                .ToList();

            foreach (var hw in vm.HardwareVM.AssignedItems)
            {
                var def = vm.HardwareVM.HardwareDefinitions
                            .FirstOrDefault(hd => hd.ItemID == hw.ItemID && hd.DesignationID == hw.DesignationID)
                         ?? vm.HardwareVM.HardwareDefinitions
                            .FirstOrDefault(hd => hd.ItemID == hw.ItemID);

                hw.ItemTitle = def?.ItemTitle ?? string.Empty;

                if (hw.DepartmentID == 0 && def?.DepartmentID > 0)
                    hw.DepartmentID = def.DepartmentID;
            }

            if (vm.HardwareVM.SelectedHardwareItems != null)
            {
                foreach (var assigned in vm.HardwareVM.AssignedItems)
                {
                    var selected = vm.HardwareVM.SelectedHardwareItems
                        .FirstOrDefault(s => s.ItemID == assigned.ItemID &&
                                              s.DepartmentID == assigned.DepartmentID);

                    if (selected != null)
                    {
                        selected.ID = assigned.ID;
                    }
                }
            }
            #endregion

            return vm;
        }

        private ViewModelRequest InitNewRequestFormFields(ViewModelRequest vm, ClientContext ctx)
        {
            vm.WorkflowStatus = ConstantHelper.RequestForm.WorkflowNewRequest.WorkflowStatusEmpty;
            vm.ReferenceNo = ConstantHelper.RequestForm.WorkflowNewRequest.ReferenceNoEmpty;

            #region Drop down list from maintenance list

            ListItemCollection companyListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Company, string.Empty, null);
            ListItemCollection designationListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Designation, string.Empty, null);
            ListItemCollection gradeListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Grade, string.Empty, null);
            ListItemCollection departmentListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Department, string.Empty, null);
            ListItemCollection locationListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Location, string.Empty, null);

            vm.EmployeeDetails.EmployeeDetailsVM.CompanyList = ProjectHelper.GetListItemsAsDDLItems(companyListItem, ConstantHelper.SPColumn.Company.Title, ConstantHelper.SPColumn.Status);
            vm.EmployeeDetails.EmployeeDetailsVM.DesignationList = ProjectHelper.GetListItemsAsDDLItems(designationListItem, ConstantHelper.SPColumn.Designation.Title, ConstantHelper.SPColumn.Status);
            vm.EmployeeDetails.EmployeeDetailsVM.GradeList = ProjectHelper.GetListItemsAsDDLItems(gradeListItem, ConstantHelper.SPColumn.Grade.Title, ConstantHelper.SPColumn.Status);
            vm.EmployeeDetails.EmployeeDetailsVM.DepartmentList = ProjectHelper.GetListItemsAsDDLItems(departmentListItem, ConstantHelper.SPColumn.Department.Title, ConstantHelper.SPColumn.Status);
            vm.EmployeeDetails.EmployeeDetailsVM.LocationList = ProjectHelper.GetListItemsAsDDLItems(locationListItem, ConstantHelper.SPColumn.Location.Title, ConstantHelper.SPColumn.Status);

            vm.HardwareVM.DepartmentList = ProjectHelper.GetListItemsAsDDLItems(departmentListItem, ConstantHelper.SPColumn.Department.Title, ConstantHelper.SPColumn.Status);

            #endregion

            #region Drop down list from constants

            vm.RequestTypeList = new List<DropDownListItem>
                            {
                                new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.NewRequestType, Value = ConstantHelper.RequestForm.RequestType.NewRequestType },
                                new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.ModificationRequestType, Value = ConstantHelper.RequestForm.RequestType.ModificationRequestType },
                                new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.TerminationRequestType, Value = ConstantHelper.RequestForm.RequestType.TerminationRequestType },
                                new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.TransferRequestType, Value = ConstantHelper.RequestForm.RequestType.TransferRequestType },
                                new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.PromotionRequestType, Value = ConstantHelper.RequestForm.RequestType.PromotionRequestType }
                            };

            vm.EmployeeDetails.EmployeeDetailsVM.ContractTypeList = new List<DropDownListItem>
                            {
                                new DropDownListItem { Text = ConstantHelper.RequestForm.ContractType.Permanent, Value = ConstantHelper.RequestForm.ContractType.Permanent },
                                new DropDownListItem { Text = ConstantHelper.RequestForm.ContractType.Temporary, Value = ConstantHelper.RequestForm.ContractType.Temporary }
                            };

            #endregion

            #region IT Requirements List
            var itItems = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.ITRequirements, string.Empty, null);

            vm.ITRequirementsVM.InfraOptions.Clear();
            vm.ITRequirementsVM.ApplicationOptions.Clear();

            foreach (ListItem li in itItems)
            {
                int itemId = li.Id;
                string title = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.ITRequirements.Title);
                string itemType = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.ITRequirements.ItemType);
                var seqObj = li[ConstantHelper.SPColumn.ITRequirements.Sequence];
                int sequence = seqObj != null && int.TryParse(seqObj.ToString(), out var s) ? s : 0;

                var designationLookup = li[ConstantHelper.SPColumn.ITRequirements.Designation] as FieldLookupValue;
                int lookupId = designationLookup?.LookupId ?? 0;

                var option = new ITOption
                {
                    ItemID = itemId,
                    Name = title,
                    DesignationID = lookupId
                };

                if (string.Equals(itemType, ConstantHelper.RequestForm.ITRequirementsType.Infra, StringComparison.OrdinalIgnoreCase))
                    vm.ITRequirementsVM.InfraOptions.Add(option);
                else if (string.Equals(itemType, ConstantHelper.RequestForm.ITRequirementsType.Applications, StringComparison.OrdinalIgnoreCase))
                    vm.ITRequirementsVM.ApplicationOptions.Add(option);
            }

            //Sorting
            vm.ITRequirementsVM.InfraOptions = vm.ITRequirementsVM.InfraOptions
                .OrderBy(opt => itItems
                    .Cast<ListItem>()
                    .First(li => FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.ITRequirements.Title) == opt.Name)
                    .GetFieldValueAs<int>(ConstantHelper.SPColumn.ITRequirements.Sequence))
                .ToList();

            vm.ITRequirementsVM.ApplicationOptions = vm.ITRequirementsVM.ApplicationOptions
                .OrderBy(opt => itItems
                    .Cast<ListItem>()
                    .First(li => FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.ITRequirements.Title) == opt.Name)
                    .GetFieldValueAs<int>(ConstantHelper.SPColumn.ITRequirements.Sequence))
                .ToList();
            #endregion

            #region Hardware List
            var allHardwareRaw = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Hardware, string.Empty, null);
            var allHardware = allHardwareRaw.Cast<ListItem>().ToList();

            //filter active department
            var activeDepartments = departmentListItem.Cast<ListItem>()
                .Where(d => FieldHelperSP
                    .GetFieldValueAsString(d, ConstantHelper.SPColumn.Department.Status)
                    .Equals(ConstantHelper.ItemStatus.Active, StringComparison.OrdinalIgnoreCase))
                .Select(d => d.Id)
                .ToHashSet();

            var activeHardware = allHardware
                .Where(li =>
                {
                    var hwStatus = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.Hardware.Status);
                    var deptLookup = li[ConstantHelper.SPColumn.Hardware.Department] as FieldLookupValue;

                    return hwStatus.Equals(ConstantHelper.ItemStatus.Active, StringComparison.OrdinalIgnoreCase)
                           && deptLookup != null
                           && activeDepartments.Contains(deptLookup.LookupId);
                })
                .ToList();

            var hdMappings = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Hardware_Designation, string.Empty, null);

            vm.HardwareVM.HardwareDefinitions.Clear();

            foreach (ListItem map in hdMappings)
            {
                var hwLookup = map[ConstantHelper.SPColumn.Hardware_Designation.Item] as FieldLookupValue;
                var desigLookup = map[ConstantHelper.SPColumn.Hardware_Designation.Designation] as FieldLookupValue;
                if (hwLookup == null) continue;

                var master = activeHardware.FirstOrDefault(m => m.Id == hwLookup.LookupId);
                if (master == null) continue;

                var title = FieldHelperSP.GetFieldValueAsString(master, ConstantHelper.SPColumn.Hardware.Title);
                var masterDeptLookup = master[ConstantHelper.SPColumn.Hardware.Department] as FieldLookupValue;

                var deptId = masterDeptLookup?.LookupId ?? 0;
                var deptTitle = "";
                if (masterDeptLookup != null)
                {
                    var deptItem = departmentListItem.Cast<ListItem>()
                                    .FirstOrDefault(d => d.Id == masterDeptLookup.LookupId);
                    deptTitle = FieldHelperSP.GetFieldValueAsString(deptItem, ConstantHelper.SPColumn.Department.Title);
                }

                vm.HardwareVM.HardwareDefinitions.Add(new HardwareDefinition
                {
                    ItemID = master.Id,
                    ItemTitle = title,
                    DepartmentID = deptId,
                    DepartmentTitle = deptTitle,
                    DesignationID = desigLookup?.LookupId ?? 0
                });
            }

            foreach (var master in activeHardware)
            {
                if (vm.HardwareVM.HardwareDefinitions.Any(d => d.ItemID == master.Id))
                    continue;

                var title = FieldHelperSP.GetFieldValueAsString(master, ConstantHelper.SPColumn.Hardware.Title);
                var masterDeptLookup = master[ConstantHelper.SPColumn.Hardware.Department] as FieldLookupValue;

                var deptId = masterDeptLookup?.LookupId ?? 0;
                var deptTitle = "";
                if (masterDeptLookup != null)
                {
                    var deptItem = departmentListItem.Cast<ListItem>()
                                    .FirstOrDefault(d => d.Id == masterDeptLookup.LookupId);
                    deptTitle = FieldHelperSP.GetFieldValueAsString(deptItem, ConstantHelper.SPColumn.Department.Title);
                }

                vm.HardwareVM.HardwareDefinitions.Add(new HardwareDefinition
                {
                    ItemID = master.Id,
                    ItemTitle = title,
                    DepartmentID = deptId,
                    DepartmentTitle = deptTitle,
                    DesignationID = 0
                });
            }

            vm.HardwareVM.HardwareItemList = activeHardware
                .Select(li => new DropDownListItem
                {
                    Id = li.Id.ToString(),
                    Text = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.Hardware.Title),
                    Value = li.Id.ToString()
                })
                .ToList();
            #endregion

            return vm;
        }

        #region Request Display Form
        public async Task<ViewModelViewRequestForm> InitRequestDisplayForm(ViewModelViewRequestForm vm, string spHostUrl, string accessToken)
        {
            using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
            {
                User currentUser = SharePointHelper.GetCurrentUser(clientContext);

                vm.CurrentUser = currentUser.LoginName;

                //Groups order by priority (Important)
                string[] groups = [ConstantHelper.SPSecurityGroup.ElanAdmin, ConstantHelper.SPSecurityGroup.DepartmentAdmin, ConstantHelper.SPSecurityGroup.ITManager, ConstantHelper.SPSecurityGroup.ITApplications, ConstantHelper.SPSecurityGroup.ITInfra, ConstantHelper.SPSecurityGroup.ElanMember];

                //Assign first matched group
                foreach (string group in groups)
                {
                    bool inGroup = false;

                    inGroup = SharePointHelper.IsUserInGroup(currentUser, group);

                    vm.AccessGroups.Add(group);
                }

                if (vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.DepartmentAdmin))
                {
                    string queryCondition = GeneralQueryHelper.ConcatCriteria(null, "Admin", "Integer", currentUser.Id.ToString(), "Contains", true);

                    ListItemCollection departmentItems = GeneralQueryHelper.GetSPItems(clientContext, ConstantHelper.SPList.Department, queryCondition, [ConstantHelper.SPColumn.Department.Title]);

                    if (departmentItems != null)
                    {
                        foreach (ListItem item in departmentItems)
                        {
                            vm.AccessDepartments.Add(FieldHelperSP.GetFieldValueAsNumber(item, ConstantHelper.SPColumn.Department.ID));
                        }
                    }
                }

                //temp object to store original value
                ViewModelViewRequestForm obj = new();

                obj = vm;

                //Store retrieved data into temp object
                RequestDA reqDA = new RequestDA();
                DataTable requestDT = reqDA.RetrieveRequestByID(vm.RequestID.Value);
                obj.RequestItem = ConvertEntitiesHelper.ConvertViewRequestItemObj(requestDT);

                DataTable employeeDT = reqDA.RetrieveEmployeeDetailsByRequestID(vm.RequestID.Value);
                obj.EmployeeDetailsItem = ConvertEntitiesHelper.ConvertViewEmployeeDetailsItemObj(employeeDT);

                DataTable hardwareDT = reqDA.RetrieveHardwareByRequestID(vm.RequestID.Value);
                obj.HardwareItems = ConvertEntitiesHelper.ConvertViewHardwareItemObj(hardwareDT);

                DataTable ITReqDT = reqDA.RetrieveITRequirementsByRequestID(vm.RequestID.Value);
                obj.ITRequirementsItems = ConvertEntitiesHelper.ConvertViewITRequirementsItemObj(ITReqDT);

                DataTable folderPermissionDT = reqDA.RetrieveFolderPermissionByRequestID(vm.RequestID.Value);
                obj.FolderPermissionsItems = ConvertEntitiesHelper.ConvertViewFolderPermissionObj(folderPermissionDT);

                DataTable changeDT = reqDA.RetrieveChangesByRequestID(vm.RequestID.Value);
                obj.ChangesItems = ConvertEntitiesHelper.ConvertChangesObj(changeDT);

                if (obj.RequestItem?.ProcessID != null && obj.RequestItem.ProcessID.Value > 0)
                {
                    string connString = ConnectionStringHelper.GetGenericWFConnString();
                    MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(connString);
                    obj.CurrentStage = wfBL.GetCurrentStepName(obj.RequestItem.ProcessID.Value);
                }

                //check role, if permission met, assign temp object to vm and return vm
                if (vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ElanAdmin))
                {
                    vm = obj;
                    vm.IsValid = true;
                }
                else if (vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.DepartmentAdmin))
                {
                    if (vm.AccessDepartments.Contains(vm.EmployeeDetailsItem.EmployeeDetailsVM.DepartmentID.Value)) vm = obj;
                    vm.IsValid = true;
                }
                else
                {
                    if (vm.EmployeeDetailsItem.EmployeeDetailsVM.EmployeeLogin == vm.CurrentUser) vm = obj;
                    vm.IsValid = true;
                }

                return vm;
            }
        }

        public async Task<RequestFormVisibilitySettings> SetVisibilitySettings(string currentStage)
        {
            RequestFormVisibilitySettings vm = new();

            if (currentStage == null) return vm;

            if (currentStage == ConstantHelper.WorkflowStatus.PENDING_REPORTING_MANAGER_APPROVAL)
            {
                vm.ShowClose = true;

                vm.ShowInfra = true;
                vm.ShowApplications = true;
                vm.ShowFolderPermission = true;
            }
            else if (currentStage == ConstantHelper.WorkflowStatus.PENDING_INFRA_TEAM_ACTION)
            {
                vm.ShowClose = true;

                vm.ShowInfra = true;
                vm.ShowFolderPermission = true;
            }
            else if (currentStage == ConstantHelper.WorkflowStatus.PENDING_APPLICATION_TEAM_ACTION)
            {
                vm.ShowClose = true;

                vm.ShowApplications = true;
            }
            else if (currentStage == ConstantHelper.WorkflowStatus.PENDING_IT_MANAGER_APPROVAL)
            {
                vm.ShowClose = true;

                vm.ShowInfra = true;
                vm.ShowApplications = true;
                vm.ShowFolderPermission = true;
            }
            else if (currentStage == ConstantHelper.WorkflowStatus.PENDING_DEPARTMENT_ADMIN_ACTION)
            {
                vm.ShowClose = true;

                vm.ShowInfra = true;
                vm.ShowApplications = true;
                vm.ShowFolderPermission = true;

                vm.ShowHardwareDetails = true;
            }
            else if (currentStage == ConstantHelper.WorkflowStatus.PENDING_ACKNOWLEDGEMENT)
            {
                vm.ShowClose = true;

                vm.ShowInfra = true;
                vm.ShowApplications = true;
                vm.ShowFolderPermission = true;
                vm.ShowAcknowledgement = true;

                vm.ShowHardwareDetails = true;
            }
            else if (currentStage == ConstantHelper.WorkflowStatus.REJECTED)
            {
                vm.ShowClose = true;

                vm.ShowInfra = true;
                vm.ShowApplications = true;
                vm.ShowFolderPermission = true;
            }
            else if (currentStage == ConstantHelper.WorkflowStatus.COMPLETED)
            {
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

        #region Modification Request
        public async Task<ViewModelRequest> InitExistingRequestFormByLogin(ViewModelRequest vm, string spHostURL, string accessToken)
        {
            try
            {
                using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    User currentUser = SharePointHelper.GetCurrentUser(ctx);
                    bool isAuthorized = SharePointHelper.IsUserInGroup(currentUser, ConstantHelper.SPSecurityGroup.ElanMember);

                    if (isAuthorized)
                    {
                        RequestDA da = new RequestDA();
                        string employeeLogin = string.Empty;

                        if (!string.IsNullOrWhiteSpace(vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin))
                        {
                            employeeLogin = vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin;
                        }

                        var obj = new ViewModelRequest();

                        #region Employee Details by Login
                        DataTable finalEmpDT = da.RetrieveFinalEmployeeDetailsByLogin(employeeLogin);
                        obj.FinalEmployeeDetails = ConvertEntitiesHelper.ConvertViewModelFinalEmployeeDetailsObj(finalEmpDT);

                        #region Map properties from Final_EmployeeDetails to EmployeeDetails
                        var finalEmployeeDetails = obj.FinalEmployeeDetails.Final_EmployeeDetails;

                        //Validate FinalEmployeeDetailsID
                        if (!finalEmployeeDetails.ID.HasValue)
                        {
                            vm.EmployeeDetails.HasError = true;
                            vm.EmployeeDetails.ErrorMessage = "No employee details found for the selected login.";
                            return vm;
                        }

                        vm.EmployeeDetails.EmployeeDetailsVM = new EmployeeDetails
                        {
                            CompanyID = finalEmployeeDetails.CompanyID,
                            CompanyTitle = finalEmployeeDetails.CompanyTitle,
                            EmployeeName = finalEmployeeDetails.EmployeeName,
                            EmployeeEmail = finalEmployeeDetails.EmployeeEmail,
                            EmployeeLogin = finalEmployeeDetails.EmployeeLogin,
                            EmployeeStatus = finalEmployeeDetails.EmployeeStatus,
                            EmployeeID = finalEmployeeDetails.EmployeeID,
                            DesignationID = finalEmployeeDetails.DesignationID,
                            DesignationTitle = finalEmployeeDetails.DesignationTitle,
                            GradeID = finalEmployeeDetails.GradeID,
                            GradeTitle = finalEmployeeDetails.GradeTitle,
                            DepartmentID = finalEmployeeDetails.DepartmentID,
                            DepartmentTitle = finalEmployeeDetails.DepartmentTitle,
                            ReportingManagerEmail = finalEmployeeDetails.ReportingManagerEmail,
                            ReportingManagerLogin = finalEmployeeDetails.ReportingManagerLogin,
                            ReportingManagerName = finalEmployeeDetails.ReportingManagerName,
                            LocationID = finalEmployeeDetails.LocationID,
                            LocationTitle = finalEmployeeDetails.LocationTitle,
                            MobileNo = finalEmployeeDetails.MobileNo,
                            ContractTemporaryStaff = finalEmployeeDetails.ContractOrTemporaryStaff,
                            JoinedDate = finalEmployeeDetails.JoinDate,
                            EndDate = finalEmployeeDetails.EndDate,
                            Description1 = finalEmployeeDetails.Description1,
                            Description2 = finalEmployeeDetails.Description2,
                            Description3 = finalEmployeeDetails.Description3,
                            Description4 = finalEmployeeDetails.Description4,
                            Description5 = finalEmployeeDetails.Description5,
                            TempFinalEmployeeDetailsID = finalEmployeeDetails.ID.Value
                        };

                        if (!string.IsNullOrEmpty(vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerLogin))
                        {
                            vm.EmployeeDetails.EmployeeDetailsVM.ReportingManager = new PeoplePickerUser
                            {
                                Login = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerLogin,
                                Name = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerName,
                                Email = vm.EmployeeDetails.EmployeeDetailsVM.ReportingManagerEmail
                            };
                        }

                        if (!string.IsNullOrEmpty(vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin))
                        {
                            vm.EmployeeDetails.EmployeeDetailsVM.Employee = new PeoplePickerUser
                            {
                                Login = vm.EmployeeDetails.EmployeeDetailsVM.EmployeeLogin,
                                Name = vm.EmployeeDetails.EmployeeDetailsVM.EmployeeName,
                                Email = vm.EmployeeDetails.EmployeeDetailsVM.EmployeeEmail
                            };
                        }
                        #endregion

                        #region Populate dropdowns
                        ListItemCollection companyListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Company, string.Empty, null);
                        ListItemCollection designationListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Designation, string.Empty, null);
                        ListItemCollection gradeListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Grade, string.Empty, null);
                        ListItemCollection departmentListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Department, string.Empty, null);
                        ListItemCollection locationListItem = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Location, string.Empty, null);

                        vm.EmployeeDetails.EmployeeDetailsVM.CompanyList = ProjectHelper.GetListItemsAsDDLItems(companyListItem, ConstantHelper.SPColumn.Company.Title, ConstantHelper.SPColumn.Status);
                        vm.EmployeeDetails.EmployeeDetailsVM.DesignationList = ProjectHelper.GetListItemsAsDDLItems(designationListItem, ConstantHelper.SPColumn.Designation.Title, ConstantHelper.SPColumn.Status);
                        vm.EmployeeDetails.EmployeeDetailsVM.GradeList = ProjectHelper.GetListItemsAsDDLItems(gradeListItem, ConstantHelper.SPColumn.Grade.Title, ConstantHelper.SPColumn.Status);
                        vm.EmployeeDetails.EmployeeDetailsVM.DepartmentList = ProjectHelper.GetListItemsAsDDLItems(departmentListItem, ConstantHelper.SPColumn.Department.Title, ConstantHelper.SPColumn.Status);
                        vm.EmployeeDetails.EmployeeDetailsVM.LocationList = ProjectHelper.GetListItemsAsDDLItems(locationListItem, ConstantHelper.SPColumn.Location.Title, ConstantHelper.SPColumn.Status);

                        vm.EmployeeDetails.EmployeeDetailsVM.CompanyTitle = GetSelectedText(vm.EmployeeDetails.EmployeeDetailsVM.CompanyList, vm.EmployeeDetails.EmployeeDetailsVM.CompanyID);
                        vm.EmployeeDetails.EmployeeDetailsVM.DesignationTitle = GetSelectedText(vm.EmployeeDetails.EmployeeDetailsVM.DesignationList, vm.EmployeeDetails.EmployeeDetailsVM.DesignationID);
                        vm.EmployeeDetails.EmployeeDetailsVM.GradeTitle = GetSelectedText(vm.EmployeeDetails.EmployeeDetailsVM.GradeList, vm.EmployeeDetails.EmployeeDetailsVM.GradeID);
                        vm.EmployeeDetails.EmployeeDetailsVM.DepartmentTitle = GetSelectedText(vm.EmployeeDetails.EmployeeDetailsVM.DepartmentList, vm.EmployeeDetails.EmployeeDetailsVM.DepartmentID);
                        vm.EmployeeDetails.EmployeeDetailsVM.LocationTitle = GetSelectedText(vm.EmployeeDetails.EmployeeDetailsVM.LocationList, vm.EmployeeDetails.EmployeeDetailsVM.LocationID);
                        #endregion

                        #region Drop down list from constants

                        string selectedRequestType = vm.RequestType;
                        string selectedContractType = vm.EmployeeDetails.EmployeeDetailsVM.ContractTemporaryStaff;

                        vm.RequestTypeList = new List<DropDownListItem>
                        {
                            new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.NewRequestType, Value = ConstantHelper.RequestForm.RequestType.NewRequestType },
                            new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.ModificationRequestType, Value = ConstantHelper.RequestForm.RequestType.ModificationRequestType },
                            new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.TerminationRequestType, Value = ConstantHelper.RequestForm.RequestType.TerminationRequestType },
                            new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.TransferRequestType, Value = ConstantHelper.RequestForm.RequestType.TransferRequestType },
                            new DropDownListItem { Text = ConstantHelper.RequestForm.RequestType.PromotionRequestType, Value = ConstantHelper.RequestForm.RequestType.PromotionRequestType }
                        }.Select(item =>
                        {
                            item.Selected = item.Value == selectedRequestType;
                            return item;
                        }).ToList();

                        vm.EmployeeDetails.EmployeeDetailsVM.ContractTypeList = new List<DropDownListItem>
                        {
                            new DropDownListItem { Text = ConstantHelper.RequestForm.ContractType.Permanent, Value = ConstantHelper.RequestForm.ContractType.Permanent },
                            new DropDownListItem { Text = ConstantHelper.RequestForm.ContractType.Temporary, Value = ConstantHelper.RequestForm.ContractType.Temporary }
                        }.Select(item =>
                        {
                            item.Selected = item.Value == selectedContractType;
                            return item;
                        }).ToList();

                        #endregion

                        #endregion

                        #region IT Requirements List

                        vm.ITRequirementsVM.InfraOptions.Clear();
                        vm.ITRequirementsVM.ApplicationOptions.Clear();

                        DataTable selectedIT = new DataTable();
                        var itItems = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.ITRequirements, string.Empty, null);
                        selectedIT = da.RetrieveFinalITRequirementsByFinalEmpID(finalEmployeeDetails.ID.Value.ToString());

                        vm.ITRequirementsVM.SelectedITRequirements = selectedIT.AsEnumerable()
                        .Select(row => new ITRequirements
                        {
                            ID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ID),
                            ItemID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemID),
                            ItemTitle = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemTitle),
                            Type = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Type),
                            IsAdded = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsAdded),
                            IsRemoved = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.IsRemoved),
                            Remark = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.Remark),
                            DateAdded = ProjectHelper.NormalizeDate(FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateAdded)),
                            DateRemoved = ProjectHelper.NormalizeDate(FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.DateRemoved))
                        })
                        .ToList();

                        var selectedItemIDs = selectedIT.AsEnumerable().Select(row => FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.ITRequirementsColumns.ItemID)).ToList();

                        foreach (ListItem li in itItems)
                        {
                            int itemId = li.Id;
                            string title = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.ITRequirements.Title);
                            string itemType = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.ITRequirements.ItemType);

                            var designationLookup = li[ConstantHelper.SPColumn.ITRequirements.Designation] as FieldLookupValue;
                            int designationId = designationLookup?.LookupId ?? 0;

                            var option = new ITOption
                            {
                                ItemID = itemId,
                                Name = title,
                                DesignationID = designationId,
                                IsSelected = selectedItemIDs.Contains(itemId)
                            };

                            if (string.Equals(itemType, ConstantHelper.RequestForm.ITRequirementsType.Infra, StringComparison.OrdinalIgnoreCase))
                                vm.ITRequirementsVM.InfraOptions.Add(option);
                            else if (string.Equals(itemType, ConstantHelper.RequestForm.ITRequirementsType.Applications, StringComparison.OrdinalIgnoreCase))
                                vm.ITRequirementsVM.ApplicationOptions.Add(option);
                        }

                        #endregion

                        #region Folder Permission
                        vm.ITRequirementsVM.FolderPermissions.Clear();

                        DataTable folderDT = da.RetrieveFinalFolderPermissionByFinalEmpID(finalEmployeeDetails.ID.Value.ToString());
                        obj.FinalFolderPermission = ConvertEntitiesHelper.ConvertViewModelFinalFolderPermission(folderDT);
                        var finalFolderPermissions = obj.FinalFolderPermission;

                        // Map each FinalFolderPermission to FolderPermission and add to the list
                        foreach (var finalFolderPermission in finalFolderPermissions)
                        {
                            var fp = finalFolderPermission.Final_FolderPermission;
                            var permission = new FolderPermission
                            {
                                NameOrPath = fp.NameOrPath,
                                IsRead = fp.IsRead,
                                IsWrite = fp.IsWrite,
                                IsDelete = fp.IsDelete,
                                Status = fp.Status,
                                IsAdded = fp.IsAdded,
                                IsRemoved = fp.IsRemoved,
                                DateAdded = fp.DateAdded,
                                DateRemoved = fp.DateRemoved,
                                Remark = fp.Remark,

                                OriginalRead = fp.IsRead,
                                OriginalWrite = fp.IsWrite,
                                OriginalDelete = fp.IsDelete
                            };

                            vm.ITRequirementsVM.FolderPermissions.Add(permission);
                        }
                        #endregion

                        #region Hardware List
                        vm.HardwareVM.DepartmentList = ProjectHelper.GetListItemsAsDDLItems(departmentListItem, ConstantHelper.SPColumn.Department.Title, ConstantHelper.SPColumn.Status);
                        var allHardwareRaw = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Hardware, string.Empty, null);
                        var allHardware = allHardwareRaw.Cast<ListItem>().ToList();

                        //filter active department
                        var activeDepartments = departmentListItem.Cast<ListItem>()
                            .Where(d => FieldHelperSP
                                .GetFieldValueAsString(d, ConstantHelper.SPColumn.Department.Status)
                                .Equals(ConstantHelper.ItemStatus.Active, StringComparison.OrdinalIgnoreCase))
                            .Select(d => d.Id)
                            .ToHashSet();

                        var activeHardware = allHardware
                            .Where(li =>
                            {
                                var hwStatus = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.Hardware.Status);
                                var deptLookup = li[ConstantHelper.SPColumn.Hardware.Department] as FieldLookupValue;

                                return hwStatus.Equals(ConstantHelper.ItemStatus.Active, StringComparison.OrdinalIgnoreCase)
                                       && deptLookup != null
                                       && activeDepartments.Contains(deptLookup.LookupId);
                            })
                            .ToList();

                        var hdMappings = GeneralQueryHelper.GetSPItems(ctx, ConstantHelper.SPList.Hardware_Designation, string.Empty, null);

                        // Populate HardwareDefinitions
                        vm.HardwareVM.HardwareDefinitions.Clear();

                        foreach (ListItem map in hdMappings)
                        {
                            var hwLookup = map[ConstantHelper.SPColumn.Hardware_Designation.Item] as FieldLookupValue;
                            var desigLookup = map[ConstantHelper.SPColumn.Hardware_Designation.Designation] as FieldLookupValue;
                            if (hwLookup == null) continue;

                            var master = activeHardware.FirstOrDefault(m => m.Id == hwLookup.LookupId);
                            if (master == null) continue;

                            var title = FieldHelperSP.GetFieldValueAsString(master, ConstantHelper.SPColumn.Hardware.Title);
                            var masterDeptLookup = master[ConstantHelper.SPColumn.Hardware.Department] as FieldLookupValue;

                            var deptId = masterDeptLookup?.LookupId ?? 0;
                            var deptTitle = "";
                            if (masterDeptLookup != null)
                            {
                                var deptItem = departmentListItem.Cast<ListItem>()
                                                .FirstOrDefault(d => d.Id == masterDeptLookup.LookupId);
                                deptTitle = FieldHelperSP.GetFieldValueAsString(deptItem, ConstantHelper.SPColumn.Department.Title);
                            }

                            vm.HardwareVM.HardwareDefinitions.Add(new HardwareDefinition
                            {
                                ItemID = master.Id,
                                ItemTitle = title,
                                DepartmentID = deptId,
                                DepartmentTitle = deptTitle,
                                DesignationID = desigLookup?.LookupId ?? 0
                            });
                        }

                        // Include unmapped hardware
                        foreach (var master in activeHardware)
                        {
                            if (vm.HardwareVM.HardwareDefinitions.Any(d => d.ItemID == master.Id))
                                continue;

                            var title = FieldHelperSP.GetFieldValueAsString(master, ConstantHelper.SPColumn.Hardware.Title);
                            var masterDeptLookup = master[ConstantHelper.SPColumn.Hardware.Department] as FieldLookupValue;

                            var deptId = masterDeptLookup?.LookupId ?? 0;
                            var deptTitle = "";
                            if (masterDeptLookup != null)
                            {
                                var deptItem = departmentListItem.Cast<ListItem>()
                                                .FirstOrDefault(d => d.Id == masterDeptLookup.LookupId);
                                deptTitle = FieldHelperSP.GetFieldValueAsString(deptItem, ConstantHelper.SPColumn.Department.Title);
                            }

                            vm.HardwareVM.HardwareDefinitions.Add(new HardwareDefinition
                            {
                                ItemID = master.Id,
                                ItemTitle = title,
                                DepartmentID = deptId,
                                DepartmentTitle = deptTitle,
                                DesignationID = 0
                            });
                        }

                        // Populate dropdown
                        vm.HardwareVM.HardwareItemList = activeHardware
                            .Select(li => new DropDownListItem
                            {
                                Id = li.Id.ToString(),
                                Text = FieldHelperSP.GetFieldValueAsString(li, ConstantHelper.SPColumn.Hardware.Title),
                                Value = li.Id.ToString()
                            })
                            .ToList();

                        var assignedHardwareDT = da.RetrieveFinalHardwareByFinalEmpID(finalEmployeeDetails.ID.Value.ToString());

                        vm.HardwareVM.AssignedItems = assignedHardwareDT.AsEnumerable()
                            .Select(row => new HardwareItemVM
                            {
                                ID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ID),
                                DepartmentID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DepartmentID),
                                ItemID = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.ItemID),
                                Quantity = FieldHelper.GetFieldValueAsNumber(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.Quantity),
                                Remarks = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.RemarkHistory),
                                DesignationID = finalEmployeeDetails.DesignationID,

                                Model = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.Model),
                                SerialNumber = FieldHelper.GetFieldValueAsString(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.SerialNumber),
                                DateAssigned = ProjectHelper.NormalizeDate(FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DateAssigned)),
                                IsReceived = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.IsReceived),
                                DateReceived = ProjectHelper.NormalizeDate(FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DateReceived)),
                                IsReturned = FieldHelper.GetFieldValueAsBoolean(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.IsReturned),
                                DateReturned = ProjectHelper.NormalizeDate(FieldHelper.GetFieldValueAsDateTime(row, ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.DateReturned))
                            })
                            .ToList();

                        foreach (var hw in vm.HardwareVM.AssignedItems)
                        {
                            var def = vm.HardwareVM.HardwareDefinitions
                                        .FirstOrDefault(hd => hd.ItemID == hw.ItemID && hd.DesignationID == hw.DesignationID)
                                     ?? vm.HardwareVM.HardwareDefinitions
                                        .FirstOrDefault(hd => hd.ItemID == hw.ItemID);

                            hw.ItemTitle = def?.ItemTitle ?? string.Empty;

                            if ((hw.DepartmentID == null || hw.DepartmentID == 0) && def?.DepartmentID > 0)
                                hw.DepartmentID = def.DepartmentID;
                        }
                        #endregion

                        if (vm.RequestType == ConstantHelper.RequestForm.RequestType.TerminationRequestType)
                        {
                            await GetSubordinatesByRepManagerLogin(vm);
                        }
                    }
                    else
                    {
                        vm.HasError = true;
                        vm.ErrorMessage = string.Format(ConstantHelper.ErrorMessage.NoAuthorized, ConstantHelper.PermissionConfigFunction.Request.CreateModificationRequest);
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

        #region Termination Request
        public async Task<ViewModelRequest> GetSubordinatesByRepManagerLogin(ViewModelRequest vm)
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

        #endregion

        #region Save Request

        public async Task<ViewModelRequest> SaveELANRequest(ViewModelRequest vm, string spHostURL, string accessToken, string appAccessToken)
        {
            LogHelper logHelper = new LogHelper();

            try
            {
                using (ClientContext ctx = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    //vm.WorkflowStatus = ConstantHelper.WorkflowStatus.DRAFT;
                    //vm.ReferenceNo = ConstantHelper.RequestForm.WorkflowNewRequest.ReferenceNoDraft;
                    vm = SaveSubmitELANRequest(vm, ctx, false, spHostURL, appAccessToken);
                }
            }
            catch (ELANActionException ex)
            {
                logHelper.LogMessage("RequestBL - SaveELANRequest Error: " + ex.ToString());

                vm.HasError = true;
                vm.ErrorMessage = $"Save failed: {ex.Message}";
            }
            catch (Exception ex)
            {
                logHelper.LogMessage("RequestBL - SaveELANRequest Error: " + ex.ToString());

                vm.HasError = true;
                vm.ErrorMessage = ConstantHelper.ErrorMessage.SaveError;
            }
            return vm;
        }

        public ViewModelRequest SaveSubmitELANRequest(ViewModelRequest vm, ClientContext ctx, bool isSubmit, string spHostURL, string accessToken)
        {
            try
            {
                User user = SharePointHelper.GetCurrentUser(ctx);

                #region Modification Info
                vm.ModifiedBy = user.Title;
                vm.ModifiedByLogin = user.LoginName;
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

                RequestDA da = new RequestDA();
                if (vm.ID == 0 || vm.ID == null)
                {
                    vm.WorkflowStatus = ConstantHelper.WorkflowStatus.DRAFT;
                    vm.ReferenceNo = ConstantHelper.RequestForm.WorkflowNewRequest.ReferenceNoDraft;

                    #region Creation Info
                    vm.CreatedBy = user.Title;
                    vm.CreatedByLogin = user.LoginName;
                    vm.Created = DateTimeHelper.GetCurrentDateTime();

                    void ApplyCreationInfo(dynamic target)
                    {
                        target.CreatedBy = vm.CreatedBy;
                        target.CreatedByLogin = vm.CreatedByLogin;
                        target.Created = vm.Created;
                    }

                    //Employee Details
                    ApplyCreationInfo(vm.EmployeeDetails.EmployeeDetailsVM);
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

                    vm = da.CreateNewRequest(vm, spHostURL);
                }
                else if (vm.ID > 0)
                {
                    if (isSubmit)
                    {
                        #region Submission Info
                        vm.SubmittedBy = user.Title;
                        vm.SubmittedByLogin = user.LoginName;
                        vm.Submitted = DateTimeHelper.GetCurrentDateTime();
                        #endregion
                    }

                    #region Creation Info For New Added Item

                    void ApplyCreationInfo(dynamic target)
                    {
                        target.CreatedBy = user.Title;
                        target.CreatedByLogin = user.LoginName;
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

                    bool success = da.UpdateRequest(vm, isSubmit);
                    vm.HasSuccess = success;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return vm;
        }
        #endregion

        #region Start Workflow
        public async Task<ViewModelRequest> SubmitELANRequest(ViewModelRequest vm, string spHostURL, string accessToken, string appAccessToken)
        {
            LogHelper logHelper = new LogHelper();
            string errorMessage = string.Empty;

            try
            {
                TokenHelper.CheckValidAccessToken(accessToken, spHostURL);
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostURL, accessToken))
                {
                    if (vm.ID <= 0 || vm.ID == null)
                    {
                        vm = SaveSubmitELANRequest(vm, clientContext, false, spHostURL, appAccessToken);
                        vm = InitExistingRequestForm(vm, clientContext);
                    }

                    //Generate ref no
                    int selectedNumber = -1;

                    vm.ReferenceNo = GenerateELANRequestRefNumber(spHostURL, appAccessToken, ref selectedNumber);

                    if (!string.IsNullOrEmpty(vm.ReferenceNo) || vm.ReferenceNo != ConstantHelper.RequestForm.WorkflowNewRequest.ReferenceNoDraft)
                    {
                        //Update refNo
                        //RequestDA da = new RequestDA();
                        //da.UpdateRequestRefNo(vm);

                        User currentUser = SharePointHelper.GetCurrentUser(clientContext);

                        PeoplePickerUser originator = new PeoplePickerUser();
                        originator.Name = currentUser.Title;
                        originator.Login = currentUser.LoginName;
                        originator.Email = currentUser.Email;

                        StartWorkflowObject startWFObject = new StartWorkflowObject();
                        startWFObject.Originator = originator;
                        int processId = -1;
                        errorMessage = WorkflowBL.StartWorkflow(startWFObject, vm, spHostURL, clientContext, appAccessToken, ref processId);

                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            vm.ProcessID = processId;
                            //Save data into db
                            vm = SaveSubmitELANRequest(vm, clientContext, true, spHostURL, appAccessToken);

                            vm.HasSuccess = true;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(vm.ReferenceNo))
                            {
                                //Restore running number here
                                GenerateELANRequestRefNumber(spHostURL, appAccessToken, ref selectedNumber, true);

                                vm.ReferenceNo = ConstantHelper.RequestForm.WorkflowNewRequest.ReferenceNoDraft;
                                vm.WorkflowStatus = ConstantHelper.WorkflowStatus.DRAFT;
                                vm = SaveSubmitELANRequest(vm, clientContext, true, spHostURL, appAccessToken);

                                vm.HasError = true;
                                vm.ErrorMessage = errorMessage;
                            }
                        }
                    }
                }
            }
            catch (ELANActionException ex)
            {
                logHelper.LogMessage("RequestBL - SubmitELANRequest Error: " + ex.ToString());

                vm.HasError = true;
                vm.ErrorMessage = $"Submission failed: {ex.Message}";
            }
            catch (Exception ex)
            {
                logHelper.LogMessage("RequestBL - SubmitELANRequest Error: " + ex.ToString());

                vm.HasError = true;
                vm.ErrorMessage = ConstantHelper.ErrorMessage.SubmitError;
            }

            return vm;
        }

        public string GenerateELANRequestRefNumber(string spHostURL, string appAccessToken, ref int selectedNumber, bool isDowngrade = false)
        {
            string newRequestRefNo = string.Empty;

            try
            {
                using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostURL, appAccessToken))
                {
                    string runningNumberTitle = ConstantHelper.RunningNumberFormatType.ELANRequestReferenceNumber;

                    RunningNumberFormat rnfObj = new RunningNumberFormat();
                    string query = GeneralQueryHelper.ConcatCriteria(string.Empty, ConstantHelper.SPColumn.RunningNumberFormat.Title, "Text", runningNumberTitle, "Eq", false);
                    ListItemCollection runningNumberItems = GeneralQueryHelper.GetSPItems(clientContext, ConstantHelper.SPList.RunningNumberFormat, query, null);
                    if (runningNumberItems != null && runningNumberItems.Count > 0)
                    {
                        string year = DateTimeHelper.GetCurrentDateTime().Year.ToString();

                        rnfObj = ConvertEntitiesHelper.ConvertRunningNumberFormatObject(runningNumberItems[0]);
                        string prefix = rnfObj.Prefix;
                        prefix = prefix.Replace(ConstantHelper.RunningNumberFormatInstance.RequestYear, year);

                        if (isDowngrade)
                        {
                            RunningNumberHelper.RestoreRunningNumber(runningNumberTitle, prefix, selectedNumber, clientContext);
                        }
                        else
                        {
                            RunningNumber rnObj = new RunningNumber();
                            rnObj = RunningNumberHelper.CreateRunningNumber(runningNumberTitle, rnfObj.Format, prefix, clientContext);

                            string format = rnObj.Format;
                            format = format.Replace(ConstantHelper.RunningNumberFormatInstance.RequestYear, year);

                            newRequestRefNo = ProjectHelper.ReplaceKeywordWithValue(ConstantHelper.RunningNumberFormatInstance.RunningNo, format, rnObj.Number);
                            selectedNumber = rnObj.Number;
                        }
                    }
                    else
                    {
                        throw new ELANActionException($"Running number format {runningNumberTitle} not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper logHelper = new LogHelper();
                logHelper.LogMessage("RequestBL - GenerateELANRequestRefNumber Error:" + ex.ToString());

                throw new ELANActionException("Unable to generate reference number.");
            }

            return newRequestRefNo;
        }
        #endregion

        #region General Function
        private string GetSelectedText(List<DropDownListItem> list, int? selectedID)
        {
            return list.FirstOrDefault(x => x.Value == selectedID?.ToString())?.Text ?? "";
        }
        #endregion

        #region Exception
        public class DataNotFoundException : Exception
        {
            public DataNotFoundException(string message) : base(message) { }
        }

        public class ELANActionException : Exception
        {
            public ELANActionException(string message) : base(message) { }
        }
        #endregion

        #region Changes Log
        public string CompareAndCreateOrUpdateChangesLog(ViewModelRequest original, ViewModelRequest updated)
        {
            var changes = new List<string>();

            CompareEmployeeDetails(original.EmployeeDetails, updated.EmployeeDetails, changes);
            CompareITRequirements(original.ITRequirementsVM, updated.ITRequirementsVM, changes);
            CompareHardwareChanges(original.HardwareVM, updated.HardwareVM, changes);

            return changes.Any() ? string.Join("; ", changes) : "No changes";
        }

        private void CompareEmployeeDetails(ViewModelEmployeeDetails oldVal, ViewModelEmployeeDetails newVal, List<string> changes)
        {
            if (oldVal?.EmployeeDetailsVM == null || newVal?.EmployeeDetailsVM == null) return;

            var propsToCompare = new Dictionary<string, Func<EmployeeDetails, object>>
            {
                { "Company", x => x.CompanyTitle },
                { "Employee Name", x => x.EmployeeName },
                { "Employee ID", x => x.EmployeeID },
                { "Designation", x => x.DesignationTitle },
                { "Grade", x => x.GradeTitle },
                { "Department", x => x.DepartmentTitle },
                { "Reporting Manager", x => x.ReportingManagerName },
                { "Location", x => x.LocationTitle },
                { "Mobile No", x => x.MobileNo },
                { "Contract/Temporary Staff", x => x.ContractTemporaryStaff },
                { "Joined Date", x => x.JoinedDate.HasValue ? x.JoinedDate.Value.ToString(appSettings.DefaultDateFormat) : "empty" },
                { "End Date", x => x.EndDate.HasValue ? x.EndDate.Value.ToString(appSettings.DefaultDateFormat) : "empty" },
                { "Description1", x => x.Description1 },
                { "Description2", x => x.Description2 },
                { "Description3", x => x.Description3 },
                { "Description4", x => x.Description4 },
                { "Description5", x => x.Description5 },
                { "Remarks", x => x.Remarks }
            };

            foreach (var kv in propsToCompare)
            {
                var oldValStr = kv.Value(oldVal.EmployeeDetailsVM)?.ToString() ?? "empty";
                var newValStr = kv.Value(newVal.EmployeeDetailsVM)?.ToString() ?? "empty";

                if (!Equals(oldValStr, newValStr))
                    changes.Add($"{kv.Key} changed from '{oldValStr}' to '{newValStr}'");
            }
        }

        private void CompareITRequirements(ViewModelITRequirements oldVal, ViewModelITRequirements newVal, List<string> changes)
        {
            CompareITOptions(oldVal.InfraOptions, newVal.InfraOptions, "Infra", changes);
            CompareITOptions(oldVal.ApplicationOptions, newVal.ApplicationOptions, "Application", changes);
            CompareFolderPermissions(oldVal.FolderPermissions, newVal.FolderPermissions, changes);
        }

        private void CompareHardwareChanges(ViewModelHardware oldHardwareVM, ViewModelHardware newHardwareVM, List<string> changeLogs)
        {
            var oldList = oldHardwareVM.AssignedItems ?? new List<HardwareItemVM>();
            var newList = newHardwareVM.AssignedItems ?? new List<HardwareItemVM>();

            // Compare removed items
            var removed = oldList.Where(o => !newList.Any(n => n.ItemID == o.ItemID)).ToList();
            foreach (var item in removed)
            {
                changeLogs.Add(string.Format(ConstantHelper.InfoMessage.CompareHardwareChangesRemoved, item.ItemTitle, item.Quantity, item.Remarks));
            }

            // Compare added items
            var added = newList.Where(n => !oldList.Any(o => o.ItemID == n.ItemID)).ToList();
            foreach (var item in added)
            {
                changeLogs.Add(string.Format(ConstantHelper.InfoMessage.CompareHardwareChangesAdded, item.ItemTitle, item.Quantity, item.Remarks));
            }

            // Compare modified items (only quantity & remarks)
            var modified = from oldItem in oldList
                           join newItem in newList on oldItem.ItemID equals newItem.ItemID
                           where oldItem.Quantity != newItem.Quantity || oldItem.Remarks != newItem.Remarks
                           select new { oldItem, newItem };

            foreach (var change in modified)
            {
                if (change.oldItem.Quantity != change.newItem.Quantity)
                {
                    changeLogs.Add(string.Format(ConstantHelper.InfoMessage.CompareHardwareChangesQuantity, change.oldItem.ItemTitle, change.oldItem.Quantity, change.newItem.Quantity));
                }
                if (change.oldItem.Remarks != change.newItem.Remarks)
                {
                    changeLogs.Add(string.Format(ConstantHelper.InfoMessage.CompareHardwareChangesRemarks, change.oldItem.ItemTitle, change.oldItem.Remarks, change.newItem.Remarks));
                }
            }
        }

        private void CompareITOptions(List<ITOption> oldOptions, List<ITOption> newOptions, string categoryName, List<string> changes)
        {
            // Get only selected items
            var oldSelected = oldOptions?.Where(o => o.IsSelected).Select(o => o.Name).ToList() ?? new List<string>();
            var newSelected = newOptions?.Where(o => o.IsSelected).Select(o => o.Name).ToList() ?? new List<string>();

            // Items removed
            foreach (var removed in oldSelected.Except(newSelected))
            {
                changes.Add(string.Format(ConstantHelper.InfoMessage.CompareITChangesRemoved, categoryName, removed));
            }

            // Items added
            foreach (var added in newSelected.Except(oldSelected))
            {
                changes.Add(string.Format(ConstantHelper.InfoMessage.CompareITChangesAdded, categoryName, added));
            }
        }

        private void CompareFolderPermissions(List<FolderPermission> oldFolders, List<FolderPermission> newFolders, List<string> changes)
        {
            var oldSet = oldFolders?.Select(f => f.NameOrPath).ToList() ?? new List<string>();
            var newSet = newFolders?.Select(f => f.NameOrPath).ToList() ?? new List<string>();

            // Detect removed folders
            foreach (var removed in oldSet.Except(newSet))
            {
                changes.Add(string.Format(ConstantHelper.InfoMessage.CompareFolderChangesRemoved, removed));
            }

            // Detect added folders
            foreach (var added in newSet.Except(oldSet))
            {
                changes.Add(string.Format(ConstantHelper.InfoMessage.CompareFolderChangesAdded, added));
            }

            // Detect permission changes for folders that exist in both
            foreach (var folderName in oldSet.Intersect(newSet))
            {
                var oldFolder = oldFolders.First(f => f.NameOrPath == folderName);
                var newFolder = newFolders.First(f => f.NameOrPath == folderName);

                if (oldFolder.IsRead != newFolder.IsRead)
                    changes.Add(string.Format(ConstantHelper.InfoMessage.CompareFolderChangesPermissionRead, folderName, oldFolder.IsRead, newFolder.IsRead));

                if (oldFolder.IsWrite != newFolder.IsWrite)
                    changes.Add(string.Format(ConstantHelper.InfoMessage.CompareFolderChangesPermissionWrite, folderName, oldFolder.IsWrite, newFolder.IsWrite));

                if (oldFolder.IsDelete != newFolder.IsDelete)
                    changes.Add(string.Format(ConstantHelper.InfoMessage.CompareFolderChangesPermissionDelete, folderName, oldFolder.IsDelete, newFolder.IsDelete));
            }
        }
        #endregion
    }
}

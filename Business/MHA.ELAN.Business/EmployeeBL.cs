using MHA.Framework.Core.SP;
using MHA.ELAN.Data;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using Microsoft.SharePoint.Client;
using System.Data;

namespace MHA.ELAN.Business
{
    public class EmployeeBL
    {
        #region Init
        public async Task<ViewModelViewEmployeeForm> InitEmployeeDisplayForm(ViewModelViewEmployeeForm vm, string spHostUrl, string accessToken)
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
                            vm.AccessDepartments.Add(FieldHelper.GetFieldValueAsNumber(item, ConstantHelper.SPColumn.Department.ID));
                        }
                    }
                }

                //temp object to store original value
                ViewModelViewEmployeeForm obj = new();

                obj = vm;

                //Store retrieved data into temp object
                EmployeeDA employeeDA = new EmployeeDA();
                DataTable requestDT = employeeDA.RetrieveRequestByEmployeeID(vm.EmployeeID.Value);
                obj.RequestItem = ConvertEntitiesHelper.ConvertRequestItemList(requestDT);

                DataTable employeeDT = employeeDA.RetrieveEmployeeDetailsByEmployeeID(vm.EmployeeID.Value);
                obj.EmployeeDetailsItem = ConvertEntitiesHelper.ConvertViewFinalEmployeeDetailsItemObj(employeeDT);

                DataTable hardwareDT = employeeDA.RetrieveHardwareByEmployeeID(vm.EmployeeID.Value);
                obj.HardwareItems = ConvertEntitiesHelper.ConvertViewFinalHardwareItemObj(hardwareDT);

                DataTable ITReqDT = employeeDA.RetrieveITRequirementsByEmployeeID(vm.EmployeeID.Value);
                obj.ITRequirementsItems = ConvertEntitiesHelper.ConvertViewFinalITRequirementsItemObj(ITReqDT);

                DataTable folderPermissionDT = employeeDA.RetrieveFolderPermissionByEmployeeID(vm.EmployeeID.Value);
                obj.FolderPermissionsItems = ConvertEntitiesHelper.ConvertViewFinalFolderPermissionObj(folderPermissionDT);

                //check role, if permission met, assign temp object to vm and return vm
                if (vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ElanAdmin))
                {
                    vm = obj;
                    vm.IsValid = true;
                }
                else if (vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.DepartmentAdmin))
                {
                    if (vm.AccessDepartments.Contains(vm.EmployeeDetailsItem.DepartmentID.Value)) vm = obj;
                    vm.IsValid = true;
                }
                else
                {
                    if (vm.EmployeeDetailsItem.EmployeeLogin == vm.CurrentUser) vm = obj;
                    vm.IsValid = true;
                }

                return vm;
            }
        }
        #endregion
    }
}

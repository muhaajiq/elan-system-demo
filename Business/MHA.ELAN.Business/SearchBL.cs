using MHA.Framework.Core.SP;
using MHA.ELAN.Data;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using Azure.Core;
using Microsoft.SharePoint.Client;

namespace MHA.ELAN.Business
{
    public class SearchBL
    {
        #region Employee Listing
        public async Task<ViewModelEmployeeListing> SearchEmployeeListing(ViewModelEmployeeListing vm, string spHostUrl, string accessToken)
        {
            ViewModelEmployeeListing obj = new();
            EmployeeDA employeeDA = new EmployeeDA();

            using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
            {
                User currentUser = SharePointHelper.GetCurrentUser(clientContext);

                vm.CurrentUser = currentUser.Title;

                //Groups order by priority
                string[] groups = [ConstantHelper.SPSecurityGroup.ElanAdmin, ConstantHelper.SPSecurityGroup.DepartmentAdmin, ConstantHelper.SPSecurityGroup.ITManager, ConstantHelper.SPSecurityGroup.ITApplications, ConstantHelper.SPSecurityGroup.ITInfra, ConstantHelper.SPSecurityGroup.ElanMember];

                //Assign matched group
                foreach (string group in groups)
                {
                    bool inGroup = false;

                    inGroup = SharePointHelper.IsUserInGroup(currentUser, group);

                    if (inGroup == true) vm.AccessGroups.Add(group);
                }

                //Permission checking
                if (vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ElanAdmin))
                {
                    //Full Access
                }
                else if (vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.DepartmentAdmin))
                {
                    //See own departments only
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
                else if (vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ElanMember) || vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ITManager) || vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ITApplications) || vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ITInfra))
                {
                    //See own records only
                    vm.MemberLogin = vm.CurrentUser;
                }

                if (vm.searchModel.JoinedEndDate != null) vm.searchModel.JoinedEndDate = vm.searchModel.JoinedEndDate.Value.Date.AddDays(1).AddTicks(-1);

                if (vm.SortField != null && vm.SortDirection != null)
                {
                    SQLSortingMapping mapping = new();

                    mapping = SQLSortingHelper.EmployeeListingSortingMapper(vm.SortField);

                    vm.SortField = mapping.ColumnName;
                    vm.SortFieldTable = mapping.TableName;
                    vm.SortDirection = SQLSortingHelper.AscOrDesc(vm.SortDirection);
                }

                obj = employeeDA.SearchEmployeeListing(vm);
            }

            return obj;
        }
        #endregion

        #region Request Listing
        public async Task<ViewModelRequestListing> SearchRequestListing(ViewModelRequestListing vm, string spHostUrl, string accessToken)
        {
            ViewModelRequestListing obj = new();
            RequestDA reqDA = new RequestDA();

            using (ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(spHostUrl, accessToken))
            {
                User currentUser = SharePointHelper.GetCurrentUser(clientContext);

                vm.CurrentUser = currentUser.Title;
                vm.CurrentUserLogin = currentUser.LoginName;

                //Groups order by priority
                string[] groups = [ConstantHelper.SPSecurityGroup.ElanAdmin, ConstantHelper.SPSecurityGroup.DepartmentAdmin, ConstantHelper.SPSecurityGroup.ITManager, ConstantHelper.SPSecurityGroup.ITApplications, ConstantHelper.SPSecurityGroup.ITInfra, ConstantHelper.SPSecurityGroup.ElanMember];

                //Assign matched group
                foreach (string group in groups)
                {
                    bool inGroup = false;

                    inGroup = SharePointHelper.IsUserInGroup(currentUser, group);

                    if (inGroup == true) vm.AccessGroups.Add(group);
                }

                if (vm.searchModel.CreatedEndDate != null) vm.searchModel.CreatedEndDate = vm.searchModel.CreatedEndDate.Value.Date.AddDays(1).AddTicks(-1);
                if (vm.searchModel.SubmittedEndDate != null) vm.searchModel.SubmittedEndDate = vm.searchModel.SubmittedEndDate.Value.Date.AddDays(1).AddTicks(-1);

                if (vm.SortField != null && vm.SortDirection != null)
                {
                    SQLSortingMapping mapping = new();

                    mapping = SQLSortingHelper.RequestListingSortingMapper(vm.SortField);

                    vm.SortField = mapping.ColumnName;
                    vm.SortFieldTable = mapping.TableName;
                    vm.SortDirection = SQLSortingHelper.AscOrDesc(vm.SortDirection);
                }

                //Permission checking
                if (vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ElanAdmin) || vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ITManager) || vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ITInfra) || vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ITApplications))
                {
                    //Full Access
                }
                else if (vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.DepartmentAdmin))
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
                else if (vm.AccessGroups.Contains(ConstantHelper.SPSecurityGroup.ElanMember))
                {
                    //See own records only
                    vm.MemberLogin = vm.CurrentUser;
                }

                obj = reqDA.SearchRequestListing(vm);
            }

            return obj;
        }
        #endregion

    }
}

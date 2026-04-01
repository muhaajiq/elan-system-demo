using MHA.Framework.Core.General;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using System.Data;

namespace MHA.ELAN.Data
{
    public class EmployeeDA
    {
        private readonly SQLHelper sqlHelper;

        public EmployeeDA()
        {
            sqlHelper = new SQLHelper(ConnectionStringHelper.GetGenericWFConnString());
        }

        public DataTable RetrieveRequestByEmployeeID(int employeeId)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.RequestColumns.FinalEmployeeDetailsID };
            List<object> paramValues = new List<object>() { employeeId };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.Request, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveEmployeeDetailsByEmployeeID(int employeeId)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.EmployeeDetailsColumns.ID };
            List<object> paramValues = new List<object>() { employeeId };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveHardwareByEmployeeID(int employeeId)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.FinalHardwareColumns.FinalEmployeeDetailsID };
            List<object> paramValues = new List<object>() { employeeId };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.FinalHardware, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveITRequirementsByEmployeeID(int employeeId)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.FinalITRequirementsColumns.FinalEmployeeDetailsID };
            List<object> paramValues = new List<object>() { employeeId };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.FinalITRequirements, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public DataTable RetrieveFolderPermissionByEmployeeID(int employeeId)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.SQLDataTable.Table.FinalFolderPermissionColumns.FinalEmployeeDetailsID };
            List<object> paramValues = new List<object>() { employeeId };

            DataTable dt = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.FinalFolderPermission, new List<string>(), paramNames, paramValues);

            return dt;
        }

        public ViewModelEmployeeListing SearchEmployeeListing(ViewModelEmployeeListing vm)
        {
            try
            {
                List<EmployeeListingData> reqListingObj = new();

                string designations = string.Join(",", vm.searchModel.Designation);

                string grades = string.Join(",", vm.searchModel.Grade);

                string departments = string.Join(",", vm.searchModel.Department);

                string locations = string.Join(",", vm.searchModel.Location);

                string contractOrPermanents = string.Join(",", vm.searchModel.ContractOrPermanent);

                string employeeStatuses = string.Join(",", vm.searchModel.EmployeeStatus);

                string managerInchargeDepts = string.Join(",", vm.AccessDepartments);

                List<string> paramNames = new string[]
                {
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.PageNumber,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.RowsPerPage,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.EmployeeName,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.EmployeeID,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.Designation,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.Grade,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.Department,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.ReportingManager,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.Location,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.JoinedStartDate,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.JoinedEndDate,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.ContractOrPermanent,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.EmployeeStatus,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.SortField,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.SortFieldTable,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.SortDirection,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.ManagerInChargeDepartments,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.MemberLogin
                }.ToList();

                List<object> paramValues = new object[]
                {
                        vm.PageNumber,
                        vm.RowsPerPage,
                        vm.searchModel.EmployeeName,
                        vm.searchModel.EmployeeId,
                        designations,
                        grades,
                        departments,
                        vm.searchModel.ReportingManager,
                        locations,
                        vm.searchModel.JoinedStartDate != null && vm.searchModel.JoinedStartDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.JoinedStartDate.Value):DBNull.Value,
                        vm.searchModel.JoinedEndDate != null && vm.searchModel.JoinedEndDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.JoinedEndDate.Value):DBNull.Value,
                        contractOrPermanents,
                        employeeStatuses,
                        vm.SortField,
                        vm.SortFieldTable,
                        vm.SortDirection,
                        managerInchargeDepts,
                        vm.MemberLogin

                }.ToList();

                DataTable myActiveEmployeeDataTable = sqlHelper.SQLExecuteAsDataSet(false, ConstantHelper.StoreProcedureName.SearchEmployeeListing, paramNames, paramValues);

                List<string> countParamNames = new string[]
                {
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.EmployeeName,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.EmployeeID,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.Designation,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.Grade,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.Department,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.ReportingManager,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.Location,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.JoinedStartDate,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.JoinedEndDate,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.ContractOrPermanent,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.EmployeeStatus,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.ManagerInChargeDepartments,
                        ConstantHelper.StoreProcedureParameter.EmployeeListing.MemberLogin
                }.ToList();

                List<object> countParamValues = new object[]
                {
                        vm.searchModel.EmployeeName,
                        vm.searchModel.EmployeeId,
                        designations,
                        grades,
                        departments,
                        vm.searchModel.ReportingManager,
                        locations,
                        vm.searchModel.JoinedStartDate != null && vm.searchModel.JoinedStartDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.JoinedStartDate.Value):DBNull.Value,
                        vm.searchModel.JoinedEndDate != null && vm.searchModel.JoinedEndDate >= DateTime.MinValue?DateTimeHelper.ConvertToUTCDateTime(vm.searchModel.JoinedEndDate.Value):DBNull.Value,
                        contractOrPermanents,
                        employeeStatuses,
                        managerInchargeDepts,
                        vm.MemberLogin
                }.ToList();

                int totalRows = sqlHelper.SQLExecuteScalar(false, ConstantHelper.StoreProcedureName.GetSearchEmployeeListingCount, countParamNames, countParamValues);
                vm.Count = totalRows;


                foreach (DataRow row in myActiveEmployeeDataTable.Rows)
                {
                    EmployeeListingData obj = new();
                    obj = ConvertEntitiesHelper.ConvertEmployeeListingObj(row);

                    reqListingObj.Add(obj);
                }

                vm.EmployeeListing = reqListingObj;

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return vm;
        }
    }
}

using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using static MHA.ELAN.Framework.Constants.ConstantHelper.SQLDataTable.Table;

namespace MHA.ELAN.Framework.Helpers
{
    public class SQLSortingHelper
    {
        public static string AscOrDesc (string s)
        {
            if (s == ConstantHelper.Sorting.GridAscending) return ConstantHelper.Sorting.SQLAscending;
            if (s == ConstantHelper.Sorting.GridDescending) return ConstantHelper.Sorting.SQLDescending;

            return string.Empty;
        }

        public static SQLSortingMapping RequestListingSortingMapper(string sortField)
        {
            return sortField switch
            {
                nameof(RequestListingData.ReferenceNo) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.Request, ColumnName = RequestColumns.ReferenceNo },
                nameof(RequestListingData.RequestType) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.Request, ColumnName = RequestColumns.RequestType },
                nameof(RequestListingData.WorkflowStatus) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.Request, ColumnName = RequestColumns.WorkflowStatus },
                nameof(RequestListingData.CreatedDate) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.Request, ColumnName = RequestColumns.Created },
                nameof(RequestListingData.SubmittedDate) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.Request, ColumnName = RequestColumns.Submitted }, 
                nameof(RequestListingData.EmployeeName) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.EmployeeDetails, ColumnName = EmployeeDetailsColumns.EmployeeName},
                nameof(RequestListingData.EmployeeID) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.EmployeeDetails, ColumnName = EmployeeDetailsColumns.EmployeeID },
                nameof(RequestListingData.Designation) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.EmployeeDetails, ColumnName = EmployeeDetailsColumns.DesignationTitle },
                nameof(RequestListingData.Department) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.EmployeeDetails, ColumnName = EmployeeDetailsColumns.DepartmentTitle },
                nameof(RequestListingData.SubmittedBy) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.Request, ColumnName = RequestColumns.SubmittedBy },
                nameof(RequestListingData.CreatedByLogin) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.Request, ColumnName = RequestColumns.CreatedByLogin },

                _ => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.Request, ColumnName = RequestColumns.ID } 
            };
        }

        public static SQLSortingMapping EmployeeListingSortingMapper(string sortField)
        {
            return sortField switch
            {
                nameof(EmployeeListingData.EmployeeName) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, ColumnName = FinalEmployeeDetailsColumns.EmployeeName },
                nameof(EmployeeListingData.EmployeeID) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, ColumnName = FinalEmployeeDetailsColumns.EmployeeID },
                nameof(EmployeeListingData.Designation) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, ColumnName = FinalEmployeeDetailsColumns.DesignationTitle },
                nameof(EmployeeListingData.Grade) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, ColumnName = FinalEmployeeDetailsColumns.GradeTitle }, 
                nameof(EmployeeListingData.Department) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, ColumnName = FinalEmployeeDetailsColumns.DepartmentTitle},
                nameof(EmployeeListingData.ReportingManager) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, ColumnName = FinalEmployeeDetailsColumns.ReportingManagerName },
                nameof(EmployeeListingData.Location) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, ColumnName = FinalEmployeeDetailsColumns.LocationTitle },
                nameof(EmployeeListingData.JoinedDate) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, ColumnName = FinalEmployeeDetailsColumns.JoinDate },
                nameof(EmployeeListingData.ContractOrPermanent) => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, ColumnName = FinalEmployeeDetailsColumns.ContractOrTemporaryStaff },

                _ => new SQLSortingMapping { TableName = ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, ColumnName = FinalEmployeeDetailsColumns.ID } 
            };
        }
    }
}

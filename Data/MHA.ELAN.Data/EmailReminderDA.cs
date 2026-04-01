using MHA.Framework.Core.General;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using System.Data;

namespace MHA.ELAN.Data
{
    public class EmailReminderDA
    {
        private readonly SQLHelper sqlHelper;

        public EmailReminderDA()
        {
            sqlHelper = new SQLHelper(ConnectionStringHelper.GetGenericWFConnString());
        }

        public DataTable RetrieveExpiringContractEmployees(int remainingMonths)
        {
            List<string> paramNames = new List<string>() { ConstantHelper.StoreProcedureParameter.ReminderEmail.RemainingMonths };
            List<object> paramValues = new List<object>() { remainingMonths };

            DataTable dt = sqlHelper.SQLExecuteAsDataSet(false, ConstantHelper.StoreProcedureName.GetExpiringContractEmployees, paramNames, paramValues);

            return dt;
        }

        public bool UpdateIsReminderEmailSentFlagByEmployeeID(List<int> employeeIDs)
        {
            bool isSuccess = false;
            List<string> updateFields = new List<string>() { ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.IsReminderEmailSent };
            List<object> updateValues = new List<object>() { 1 };

            foreach (int employeeID in employeeIDs)
            {
                isSuccess = sqlHelper.Update(ConstantHelper.SQLDataTable.Table.FinalEmployeeDetails, updateFields, updateValues, ConstantHelper.SQLDataTable.Table.FinalEmployeeDetailsColumns.ID, employeeID);
            }

            return isSuccess;
        }
    }
}

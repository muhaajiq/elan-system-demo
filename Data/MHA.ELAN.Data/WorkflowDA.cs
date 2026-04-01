using MHA.Framework.Core.General;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using System.Data;

namespace MHA.ELAN.Data
{
    public class WorkflowDA
    {
        private readonly SQLHelper sqlHelper;

        public WorkflowDA()
        {
            sqlHelper = new SQLHelper(ConnectionStringHelper.GetGenericWFConnString());
        }

        public int GetWFStepDueDateDaysFromDb(string internalStepName, string wfConnString)
        {
            string query = $@"
                SELECT {ConstantHelper.WFSQLTableFields.d_tblStep.DueDateDay}
                FROM {ConstantHelper.SQLDataTable.Table.d_tblStep} step
                WHERE step.{ConstantHelper.WFSQLTableFields.d_tblStep.InternalStepName} = @InternalStepName";

            List<string> paramNames = new List<string> { "@InternalStepName" };
            List<object> paramValues = new List<object> { internalStepName };

            int result = sqlHelper.SQLExecuteScalar(true, query, paramNames, paramValues);

            return result;
        }

        public DataTable GetWorkflowHistory(int processID)
        {
            List<string> paramNames = new List<string>() { "@ProcessID" };
            List<object> paramValues = new List<object>() { processID };

            DataTable dt = sqlHelper.SQLExecuteAsDataSet(false, ConstantHelper.StoreProcedureName.GetWorkflowTaskHistoryByProcessID, paramNames, paramValues);

            return dt;
        }

        public bool IsTaskOngoing(int taskID)
        {
            string query = string.Format("SELECT COUNT(ID) AS TotalCount FROM {0} WHERE {1} = @TaskID AND {2} = @IsComplete", ConstantHelper.WFOnGoingTasks.TableName, ConstantHelper.WFOnGoingTasks.ColumnName.TaskID, ConstantHelper.WFOnGoingTasks.ColumnName.IsComplete);
            List<string> parameterNames = new List<string> { "TaskID", "IsComplete" };
            List<object> parameterValues = new List<object> { taskID, false };
            DataTable result = sqlHelper.SQLExecuteAsDataSet(true, query, parameterNames, parameterValues);

            if (result != null && result.Rows.Count > 0)
                return (FieldHelper.GetFieldValueAsNumber(result.Rows[0], "TotalCount") > 0);
            else
                return false;
        }

        public void InsertOnGoingTasks(int taskID)
        {
            List<string> columnNames = new List<string>() { ConstantHelper.WFOnGoingTasks.ColumnName.TaskID };
            List<object> columnValues = new List<object>() { taskID };
            sqlHelper.Create(ConstantHelper.WFOnGoingTasks.TableName, columnNames, columnValues);
        }

        public void UpdateOnGoingTasks(int taskID)
        {
            string query = string.Format("UPDATE {0} SET {1} = @IsComplete WHERE {2} = @TaskID AND {1} = 0 ", ConstantHelper.WFOnGoingTasks.TableName, ConstantHelper.WFOnGoingTasks.ColumnName.IsComplete, ConstantHelper.WFOnGoingTasks.ColumnName.TaskID);
            List<string> parameterNames = new List<string> { "IsComplete", "TaskID" };
            List<object> parameterValues = new List<object> { true, taskID };
            sqlHelper.SQLExecuteAsDataSet(true, query, parameterNames, parameterValues);
        }

        public int GetStepTemplateIDByInternalName(string stepInternalName)
        {
            int stepTemplateID = -1;
            List<string> fieldNames = new List<string>();
            List<string> parameterNames = new List<string>();
            List<object> parameterValues = new List<object>();

            fieldNames.Add("StepID");
            fieldNames.Add("StepName");
            fieldNames.Add("InternalStepName");

            parameterNames.Add("InternalStepName");
            parameterValues.Add(stepInternalName);
            //string query = "SELECT StepID, StepName ,InternalStepName FROM d_tblStep WHERE InternalStepName = @StepInternalName";

            DataTable rseultTable = sqlHelper.Retrieve(ConstantHelper.SQLDataTable.Table.d_tblStep, fieldNames, parameterNames, parameterValues);
            if (rseultTable != null && rseultTable.Rows.Count > 0)
            {
                stepTemplateID = FieldHelper.GetFieldValueAsNumber(rseultTable.Rows[0], ConstantHelper.WFSQLTableFields.d_tblStep.StepID);
            }
            return stepTemplateID;

        }
        public DataTable GetSpecificStageActiveActioner(int processID, int stepTemplateID)
        {
            DataTable rseultTable = null;

            List<string> parameterNames = new List<string>();
            List<object> parameterValues = new List<object>();

            parameterNames.Add("ProcessID");
            parameterNames.Add("StepTemplateID");
            parameterValues.Add(processID);
            parameterValues.Add(stepTemplateID);

            rseultTable = sqlHelper.SQLExecuteAsDataSet(false, ConstantHelper.StoreProcedureName.GetActiveActionerByProcessIDStepTemplateID, parameterNames, parameterValues);

            return rseultTable;
        }

        public DataTable GetManagerTask(DateTime fromDate)
        {
            List<string> paramNames = new List<string>() { "@InternalStepName", "@FromDate", "@ProcessName" };
            List<object> paramValues = new List<object>() { ConstantHelper.WorkflowStepName.ELANWorkflow.PendingReportingManagerApproval, fromDate, ConstantHelper.WorkflowName.ELANWorkflow };

            DataTable dt = sqlHelper.SQLExecuteAsDataSet(false, ConstantHelper.StoreProcedureName.GetPendingReportingManagerApprovalTasksByProcessName, paramNames, paramValues);

            return dt;
        }
    }
}

using MHA.Framework.Core.General;
using MHA.Framework.Core.Workflow.BO;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using MHA.ELAN.Framework.Helpers;
using System.Data;

namespace MHA.ELAN.Data
{
    public class HomeDA
    {
        private readonly SQLHelper sqlHelper;

        public HomeDA()
        {
            sqlHelper = new SQLHelper(ConnectionStringHelper.GetGenericWFConnString());
        }

        public DataTable GetMyPendingTaskData(ViewModelMyPendingTask vmMyTask, Actioner currentUser, string applicationName)
        {
            int processTemplateId = ConstantHelper.WorkflowTemplateId.PULZApprovalWorkflow;

            List<string> pendingTaskParamNames = new string[]
            {
                        ConstantHelper.StoreProcedureParameter.ActionerLogin,
                        ConstantHelper.StoreProcedureParameter.ProcessName,
                        ConstantHelper.StoreProcedureParameter.ApplicationName,
                        ConstantHelper.StoreProcedureParameter.PageNumber,
                        ConstantHelper.StoreProcedureParameter.RowsPerPage,
                        ConstantHelper.StoreProcedureParameter.ProcessTemplateID
            }.ToList();
            List<object> pendingTaskParamValues = new object[]
            {
                        currentUser.LoginName,
                        string.Empty,
                        applicationName,
                        vmMyTask.CurrentPage,
                        vmMyTask.RowsPerPage,
                        processTemplateId
            }.ToList();

            return sqlHelper.SQLExecuteAsDataSet(false, ConstantHelper.StoreProcedureName.GetMyPendingAndIncomingTasksByBatchByProcessNameApplicationName, pendingTaskParamNames, pendingTaskParamValues);
        }

        public int GetMyPendingTaskDataCount(Actioner currentUser, string applicationName)
        {
            int processTemplateId = ConstantHelper.WorkflowTemplateId.PULZApprovalWorkflow;

            List<string> pendingTaskCountParamNames = new string[]
                    {
                        ConstantHelper.StoreProcedureParameter.ActionerLogin,
                        ConstantHelper.StoreProcedureParameter.ProcessName,
                        ConstantHelper.StoreProcedureParameter.ApplicationName,
                        ConstantHelper.StoreProcedureParameter.ProcessTemplateID
                    }.ToList();
            List<object> pendingTaskCountParamValues = new object[]
            {
                        currentUser.LoginName,
                        string.Empty,
                        applicationName,
                        processTemplateId
            }.ToList();
            
            return sqlHelper.SQLExecuteScalar(false, ConstantHelper.StoreProcedureName.GetMyPendingAndIncomingTasksCountByBatchByProcessNameApplicationName, pendingTaskCountParamNames, pendingTaskCountParamValues);
        }

        public DataTable GetActiveRequestData(ViewModelMyActiveRequest vm, Actioner currentUser, string workflowStatuses)
        {
            List<string> paramNames = new string[]
                    {
                        ConstantHelper.StoreProcedureParameter.ActionerLogin,
                        ConstantHelper.StoreProcedureParameter.PageNumber,
                        ConstantHelper.StoreProcedureParameter.RowsPerPage,
                        ConstantHelper.StoreProcedureParameter.WorkflowStatus,
                    }.ToList();
            List<object> paramValues = new object[]
            {
                        currentUser.LoginName,
                        vm.PageNumber,
                        vm.RowsPerPage,
                        workflowStatuses
            }.ToList();

            return sqlHelper.SQLExecuteAsDataSet(false, ConstantHelper.StoreProcedureName.GetMyActiveRequest, paramNames, paramValues);
        }

        public int GetActiveRequestDataCount(Actioner currentUser, string workflowStatuses)
        {
            List<string> countParamNames = new string[]
                    {
                        ConstantHelper.StoreProcedureParameter.ActionerLogin,
                        ConstantHelper.StoreProcedureParameter.WorkflowStatus,
                    }.ToList();
            List<object> countParamValues = new object[]
            {
                        currentUser.LoginName,
                        workflowStatuses
            }.ToList();

            return sqlHelper.SQLExecuteScalar(false, ConstantHelper.StoreProcedureName.GetMyActiveRequestCount, countParamNames, countParamValues);
        }
    }
}

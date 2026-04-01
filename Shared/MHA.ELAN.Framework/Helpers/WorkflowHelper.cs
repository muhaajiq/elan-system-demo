using MHA.Framework.Core.SP;
using MHA.Framework.Core.Workflow.BL;
using MHA.Framework.Core.Workflow.BO;
using MHA.ELAN.Entities;
using MHA.ELAN.Framework.Constants;
using Microsoft.SharePoint.Client;
using System.Data;

namespace MHA.ELAN.Framework.Helpers
{
    public class WorkflowHelper
    {
        public static Actioner ConstructActioner(User user)
        {
            Actioner actioner = new Actioner(user.LoginName, user.Title, user.Email);
            return actioner;
        }

        public static Actioner ConstructActioner(PeoplePickerUser PPUser)
        {
            if (PPUser.Email == null)
            {
                PPUser.Email = "";
            }
            Actioner actioner = new Actioner(PPUser.Login, PPUser.Name, PPUser.Email);
            return actioner;
        }

        public static List<Actioner> ContructActionerList(List<PeoplePickerUser> peoplePickerList)
        {
            List<Actioner> ListActioner = new List<Actioner>();
            foreach (PeoplePickerUser pplPickerUser in peoplePickerList)
            {
                Actioner actioner = new Actioner(pplPickerUser.Login, pplPickerUser.Name, pplPickerUser.Email);
                ListActioner.Add(actioner);
            }
            return ListActioner;
        }

        public static void AddStage(ref StageActioners stageActioners, string stageName, int dueDays, List<PeoplePickerUser> users)
        {
            if (users != null && users.Count > 0)
            {
                stageActioners.NextStage(stageName, dueDays);
                WorkflowHelper.AddStageActionerFromPeoplePickerList(ref stageActioners, users);
            }
        }

        public static void AddStageActionerFromPeoplePickerList(ref StageActioners stageActioners, List<PeoplePickerUser> userList)
        {
            foreach (PeoplePickerUser stageUser in userList)
            {
                Actioner actioner = ConstructActioner(stageUser);
                stageActioners.AddActioner(actioner);
            }
        }

        public static bool IsWorkflowRunning(string refNo)
        {
            // 1. use wfbl.GetWorkflowInstanceByReferenceKey
            String WFConnString = ConnectionStringHelper.GetGenericWFConnString();
            MHA.Framework.Core.Workflow.BL.WorkflowBL wfBL = new MHA.Framework.Core.Workflow.BL.WorkflowBL(WFConnString);

            DataTable dtWFInstance = wfBL.GetWorkflowInstancesByReferenceKey(refNo);

            // 2. then wfbl.GetPendingTasksByProcessID
            if (dtWFInstance.Rows.Count > 0)
            {
                foreach (DataRow dr in dtWFInstance.Rows)
                {
                    int processID = Convert.ToInt32(dr[ConstantHelper.WFSQLTableFields.d_tblProcess.ProcessID]);
                    DataTable dtPendingTaskForCurrentMDR = wfBL.GetPendingTasksByProcessID(processID);
                    if (dtPendingTaskForCurrentMDR.Rows.Count > 0)
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        #region Workflow Status
        public static bool IsWorkflowRunning(int processID)
        {
            string ConnString = ConnectionStringHelper.GetGenericWFConnString();
            WorkflowBL wfBL = new WorkflowBL(ConnString);
            string curStep = wfBL.GetCurrentStepName(processID);
            //TODO
            if (curStep.Equals(ConstantHelper.WorkflowStatus.APPROVED) || curStep.Equals(ConstantHelper.WorkflowStatus.COMPLETED) || curStep.Equals(ConstantHelper.WorkflowStatus.REJECTED) || curStep.Equals(ConstantHelper.WorkflowStatus.TERMINATED))
                return false;
            else
                return true;
        }

        public static bool IsWorkflowRunning(List<ViewRequestItem> reqList)
        {
            if (reqList != null && reqList.Count > 0)
            {
                foreach (ViewRequestItem item in reqList)
                {
                    if (item.WorkflowStatus != ConstantHelper.WorkflowStatus.COMPLETED &&
                        item.WorkflowStatus != ConstantHelper.WorkflowStatus.REJECTED &&
                        item.WorkflowStatus != ConstantHelper.WorkflowStatus.TERMINATED)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        public static int GetWFDueDays(ListItemCollection listItems, string filterField, string filterValue, string durationField)
        {
            int dueDays = 0;

            ListItem targetItem = listItems?.FirstOrDefault(item =>
                FieldHelper.GetFieldValueAsString(item, filterField) == filterValue);

            if (targetItem == null)
            {
                throw new Exception(string.Format(ConstantHelper.ErrorMessage.MissingWFDueDate, filterValue));
            }

            dueDays = FieldHelper.GetFieldValueAsNumber(targetItem, durationField);

            return dueDays;
        }

        public static DateTime CalculateNewDueDate(int processID, string processXML)
        {
            DateTime newDueDate = DateTime.MinValue;

            string ConnString = ConnectionStringHelper.GetGenericWFConnString();
            WorkflowBL wfBL = new WorkflowBL(ConnString);

            ProcessKeywords keywords = new ProcessKeywords(processXML);

            //Get current workflow information
            string currentStep = wfBL.GetCurrentStepName(processID);

            //Get list of due days from the keyword xml
            string workflowCycleDueDays = string.Empty;
            int workflowCycleDueDaysIndex = processXML.IndexOf(ConstantHelper.WorkflowKeywords.Common.WorkflowCycleDueDays);
            if (workflowCycleDueDaysIndex > -1)
            {
                workflowCycleDueDays = keywords.GetKeywordValue(ConstantHelper.WorkflowKeywords.Common.WorkflowCycleDueDays);
            }

            //Working Days
            List<DayOfWeek> daysOfWeek = new List<DayOfWeek>
                    {
                        DayOfWeek.Monday,
                        DayOfWeek.Tuesday,
                        DayOfWeek.Wednesday,
                        DayOfWeek.Thursday,
                        DayOfWeek.Friday
                    };

            int dueDay = -1;
            if (!string.IsNullOrEmpty(workflowCycleDueDays))
            {
                List<int> dueDays = workflowCycleDueDays.Split(',').Select(x => int.Parse(x)).ToList();

                switch (currentStep)
                {
                    case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingReportingManagerApproval:
                        dueDay = dueDays[0];
                        break;

                    case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingInfraTeamAction:
                        dueDay = dueDays[1];
                        break;

                    case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingApplicationTeamAction:
                        dueDay = dueDays[2];
                        break;

                    case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingITManagerApproval:
                        dueDay = dueDays[3];
                        break;

                    case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingDepartmentAdminAction:
                        dueDay = dueDays[4];
                        break;

                    case ConstantHelper.WorkflowStepName.ELANWorkflow.PendingAcknowledgement:
                        dueDay = dueDays[5];
                        break;

                    default:
                        break;
                }

                if (dueDay > -1)
                {
                    newDueDate = DateTimeHelper.GetNextWorkingDate(daysOfWeek, DateTimeHelper.GetCurrentUtcDateTime(), dueDay);//.Date;

                }
            }

            return newDueDate;
        }

        public static PeoplePickerUser GetTaskOwnerByTaskID(int taskID, WorkflowBL wfBL)
        {
            PeoplePickerUser user = new PeoplePickerUser();
            TaskInstance taskInstance = wfBL.GetTask(taskID);
            user.Login = taskInstance.Assignee.LoginName;
            user.Name = taskInstance.Assignee.Name;
            return user;
        }
    }
}

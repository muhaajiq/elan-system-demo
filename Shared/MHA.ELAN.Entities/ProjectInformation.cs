using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    [Serializable]
    [DataContract]
    public class ProjectInformation
    {
        [DataMember]
        public string ProjectTitle { get; set; }
        [DataMember]
        public string ProjectCode { get; set; }
        [DataMember]
        public string WorkingDays { get; set; }
        [DataMember]
        public string ProjectPhase { get; set; }


        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string TransmitalCode { get; set; }
        [DataMember]
        public int ReminderDays { get; set; }
        [DataMember]
        public int DCCReminderDays { get; set; }
        [DataMember]
        public string PlanDateAutomation { get; set; }
        [DataMember]
        public string CompanySubsiteTemplateName { get; set; }
        [DataMember]
        public string DocPrepReminderDays { get; set; }
        [DataMember]
        public List<int> ReviewDueDays { get; set; }
        [DataMember]
        public List<int> ApprovalDueDays { get; set; }

        private string _HomePage;
        [DataMember]
        public string HomePage
        {
            get { return _HomePage; }
            set { _HomePage = value; }
        }

        private string _AdministrationPage;
        [DataMember]
        public string AdministrationPage
        {
            get { return _AdministrationPage; }
            set { _AdministrationPage = value; }
        }

        public ProjectInformation()
        {
            ReviewDueDays = new List<int>();
            ApprovalDueDays = new List<int>();
        }
    }
}

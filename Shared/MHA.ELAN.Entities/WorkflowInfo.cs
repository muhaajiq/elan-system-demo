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
    public class WorkflowInfo
    {
        [DataMember]
        public string ProcessName { get; set; }
        [DataMember]
        public string ApplicationName { get; set; }
        [DataMember]
        public string List { get; set; }
        [DataMember]
        public int ProcessID { get; set; }
        [DataMember]
        public int ProfileCardID { get; set; }
        [DataMember]
        public string ProfileCardTitle { get; set; }
        [DataMember]
        public string DocumentNumber { get; set; }
        [DataMember]
        public string Revision { get; set; }
        [DataMember]
        public string CompanyCode { get; set; }
        [DataMember]
        public string InternalReviewCode { get; set; }
        [DataMember]
        public string ClientDocumentNo { get; set; }
        [DataMember]
        public string ClientRevision { get; set; }
        [DataMember]
        public string ViewProfileCardURL { get; set; }
        [DataMember]
        public string ViewCommentFormURL { get; set; }
        [DataMember]
        public DateTime CompletedDate { get; set; }
        [DataMember]
        public string SubsiteMember { get; set; }
        [DataMember]
        public string DocumentOwner { get; set; }
        [DataMember]
        public bool HasApprovalComments { get; set; }


        public WorkflowInfo()
        {

        }

    }
}

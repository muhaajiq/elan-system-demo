using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class Hardware
    {
        [DataMember]
        public int? ID { get; set; }
        [DataMember]
        public int? DepartmentID { get; set; }
        [DataMember]
        public string DepartmentTitle { get; set; } = string.Empty;
        [DataMember]
        public int ItemID { get; set; }
        [DataMember]
        public string ItemTitle { get; set; } = string.Empty;
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public string Remarks { get; set; } = string.Empty;
        [DataMember]
        public string RemarkHistory { get; set; } = string.Empty;
        [DataMember]
        public string Model { get; set; } = string.Empty;
        [DataMember]
        public string SerialNumber { get; set; } = string.Empty;
        [DataMember]
        public DateTime? DateAssigned { get; set; }
        [DataMember]
        public bool IsReturned { get; set; }
        [DataMember]
        public DateTime? DateReturned { get; set; }
        [DataMember]
        public bool IsReceived { get; set; }
        [DataMember]
        public DateTime? DateReceived { get; set; }
        [DataMember]
        public int? RequestID { get; set; }
        [DataMember]
        public DateTime? Created { get; set; }
        [DataMember]
        public string CreatedBy { get; set; } = string.Empty;
        [DataMember]
        public string CreatedByLogin { get; set; } = string.Empty;
        [DataMember]
        public DateTime? Modified { get; set; }
        [DataMember]
        public string ModifiedBy { get; set; } = string.Empty;
        [DataMember]
        public string ModifiedByLogin { get; set; } = string.Empty;

        public bool IsTempRemoved { get; set; }
        public string TempStatus { get; set; } = string.Empty;
        public bool PendingIsReturned { get; set; }
        public DateTime? PendingDateReturned { get; set; }
        public bool IsInUndoMode { get; set; } = false;
    }
}

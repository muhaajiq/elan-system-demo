using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    [Serializable]
    [DataContract]
    public class FolderPermission
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember, Required]
        public string NameOrPath { get; set; } = string.Empty;
        [DataMember]
        public bool IsRead { get; set; }
        [DataMember]
        public bool IsWrite { get; set; }
        [DataMember]
        public bool IsDelete { get; set; }
        [DataMember]
        public string Status { get; set; } = string.Empty;
        [DataMember]
        public bool IsAdded { get; set; }
        [DataMember]
        public bool IsRemoved { get; set; }
        [DataMember]
        public DateTime? DateAdded { get; set; }
        [DataMember]
        public DateTime? DateRemoved { get; set; }
        [DataMember]
        public string Remark { get; set; } = string.Empty;
        [DataMember]
        public int RequestID { get; set; }
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

        [DataMember]
        public bool IsBeingRemoved { get; set; }
        [DataMember]
        public bool IsTempRemoved { get; set; }
        [DataMember]
        public string TempStatus { get; set; } = string.Empty;

        // Track original state
        public bool OriginalRead { get; set; }
        public bool OriginalWrite { get; set; }
        public bool OriginalDelete { get; set; }

        public string ReadLabel => IsRead != OriginalRead ? (IsRead ? "To be added" : "To be deleted") : "";
        public string WriteLabel => IsWrite != OriginalWrite ? (IsWrite ? "To be added" : "To be deleted") : "";
        public string DeleteLabel => IsDelete != OriginalDelete ? (IsDelete ? "To be added" : "To be deleted") : "";
    }
}

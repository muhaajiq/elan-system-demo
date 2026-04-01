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
    public class ITRequirements : IItemWithStatus
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public int RequestID { get; set; }

        [DataMember]
        public int? ItemID { get; set; }

        [DataMember]
        public string ItemTitle { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public bool IsAdded { get; set; }

        [DataMember]
        public bool IsRemoved { get; set; }

        [DataMember]
        public DateTime? DateAdded { get; set; }

        [DataMember]
        public DateTime? DateRemoved { get; set; }

        [DataMember]
        public string Remark { get; set; }

        [DataMember]
        public DateTime? Created { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public string CreatedByLogin { get; set; }

        [DataMember]
        public DateTime? Modified { get; set; }

        [DataMember]
        public string ModifiedBy { get; set; }

        [DataMember]
        public string ModifiedByLogin { get; set; }
    }
}

namespace MHA.ELAN.Entities
{
    public class Final_ITRequirements
    {
        public int? ID { get; set; }
        public int? FinalEmployeeDetailsID { get; set; }
        public int? ItemID { get; set; }
        public string ItemTitle { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool IsAdded { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateRemoved { get; set; }
        public string Remark { get; set; } = string.Empty;
        public DateTime? Created { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByLogin { get; set; } = string.Empty;
        public DateTime? Modified { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
        public string ModifiedByLogin { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class ViewModelHardware
    {
        public ViewModelHardware()
        {
            DepartmentList = new List<DropDownListItem>();
            DesignationList = new List<DropDownListItem>();
            SelectedHardwareItems = new List<Hardware>();
            HardwareDefinitions = new List<HardwareDefinition>();
            AssignedItems = new List<HardwareItemVM>();
            HardwareVM = new Hardware();
        }

        #region Common VM Properties
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

        public bool HasSuccess { get; set; }
        public string SuccessMessage { get; set; }

        public bool RequestModelHasError { get; set; }
        public string RequestModelErrorMessage { get; set; }

        #endregion

        [DataMember]
        public string Designation { get; set; }

        [DataMember]
        public int DesignationID { get; set; }

        [DataMember]
        public List<Hardware> SelectedHardwareItems { get; set; }

        public List<HardwareItemVM> AssignedItems { get; set; } = new();

        [DataMember]
        public List<DropDownListItem> DepartmentList { get; set; }
        
        [DataMember]
        public List<DropDownListItem> DesignationList { get; set; }
        
        public List<DropDownListItem> HardwareItemList { get; set; }

        public List<HardwareDefinition> HardwareDefinitions { get; set; }

        public Hardware HardwareVM { get; set; }

    }

    public class HardwareItemVM
    {
        public int ID { get; set; }

        public int? DepartmentID { get; set; }

        [Required(ErrorMessage = "Item is required")]
        public int? ItemID { get; set; }

        public string ItemTitle { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;

        public string Remarks { get; set; }

        public int? DesignationID { get; set; }

        public bool IsTempRemoved { get; set; }
        public string TempStatus { get; set; } = string.Empty;

        public int? OriginalItemID { get; set; }
        public int OriginalQuantity { get; set; }
        public string OriginalRemarks { get; set; }
        public bool OriginalRemoved { get; set; }

        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime? DateAssigned { get; set; }
        public bool IsReturned { get; set; }
        public DateTime? DateReturned { get; set; }
        public bool IsReceived { get; set; }
        public DateTime? DateReceived { get; set; }
    }

    public class HardwareDefinition
    {
        public string ItemTitle { get; set; }
        public int ItemID { get; set; }
        public int DepartmentID { get; set; }
        public string DepartmentTitle { get; set; }
        public int DesignationID { get; set; }
    }

}

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
    public class ViewModelITRequirements
    {
        public ViewModelITRequirements()
        {
            InfraOptions = new List<ITOption>();
            ApplicationOptions = new List<ITOption>();
            FolderPermissions = new List<FolderPermission>();
            SelectedITRequirements = new List<ITRequirements>();
            ITRequirementsVM = new ITRequirements();
            HasError = false;
            HasSuccess = false;
        }

        #region Common VM Properties
        [DataMember]
        public bool HasError { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public bool HasSuccess { get; set; }

        [DataMember]
        public string SuccessMessage { get; set; }

        [DataMember]
        public bool RequestModelHasError { get; set; }

        [DataMember]
        public string RequestModelErrorMessage { get; set; }

        #endregion

        #region IT Requirements Properties
        [DataMember]
        public string Designation { get; set; }

        [DataMember]
        public int DesignationID { get; set; }

        [DataMember]
        public List<ITOption> InfraOptions { get; set; }

        [DataMember]
        public List<ITOption> ApplicationOptions { get; set; }

        [DataMember]
        public List<FolderPermission> FolderPermissions { get; set; }

        [DataMember]
        public List<ITRequirements> SelectedITRequirements { get; set; }

        [DataMember]
        public ITRequirements ITRequirementsVM { get; set; }

        #endregion
    }

    [Serializable]
    [DataContract]
    public class ITOption
    {
        [DataMember]
        public int? ItemID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsSelected { get; set; }

        [DataMember]
        public int? DesignationID { get; set; }

        public bool OriginalSelected { get; set; }

        public bool IsSoftRemoved { get; set; }

        public string ChangeLabel =>
            IsSelected != OriginalSelected
                ? (IsSelected ? "To be added" : "To be deleted")
                : string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MHA.ELAN.Entities
{
    public class RequestFormVisibilitySettings
    {
        //Buttons
        public bool ShowRequireAmendment { get; set; } = false;
        public bool ShowApprove { get; set; } = false;
        public bool ShowReject { get; set; } = false;
        public bool ShowSave { get; set; } = false;
        public bool ShowComplete { get; set; } = false;
        public bool ShowClose { get; set; } = false;

        //IT Requirements
        public bool ShowInfra { get; set; } = false;
        public bool ShowApplications { get; set; } = false;
        public bool ShowFolderPermission { get; set; } = false;
        public bool ShowAcknowledgement { get; set; } = false;

        //Hardware
        public bool ShowHardwareDetails { get; set; } = false;
        public bool ShowColumnsAsEditable { get; set; } = false;

        //Employee PeoplePicker
        public bool ShowEmployeePeoplePicker { get; set; } = false;
    }
}

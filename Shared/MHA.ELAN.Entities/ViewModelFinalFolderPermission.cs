using System;
using System.Runtime.Serialization;

namespace MHA.ELAN.Entities
{
    public class ViewModelFinalFolderPermission
    {
        public ViewModelFinalFolderPermission()
        {
            Final_FolderPermission = new Final_FolderPermission();
        }
        [DataMember]
        public Final_FolderPermission Final_FolderPermission { get; set; }
    }
}

using System.Runtime.Serialization;

namespace MHA.ELAN.Entities
{
    [Serializable]
    [DataContract]
    public class PeoplePickerUser
    {
        private int _LookupId;
        private string _Login;
        private string _Name;
        private string _Email;
        private bool _IsEmailSendSuccess;
        private bool _IsThirdParty;

        [DataMember]
        public bool IsEmailSendSuccess
        {
            get { return _IsEmailSendSuccess; }
            set { _IsEmailSendSuccess = value; }
        }

        [DataMember]
        public int LookupId
        {
            get { return _LookupId; }
            set { _LookupId = value; }
        }

        [DataMember]
        public string Login
        {
            get { return _Login; }
            set { _Login = value; }
        }

        [DataMember]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        [DataMember]
        public string Email
        {
            get { return _Email; }
            set { _Email = value; }
        }

        [DataMember]
        public bool IsThirdParty
        {
            get { return _IsThirdParty; }
            set { _IsThirdParty = value; }
        }

        public PeoplePickerUser()
        {

        }
    }
}

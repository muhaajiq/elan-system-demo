using Newtonsoft.Json;
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
    public class DropDownListItem
    {
        private bool _Selected;

        [DataMember]
        public bool Selected
        {
            get { return _Selected; }
            set { _Selected = value; }
        }

        private string _Text;

        [DataMember]
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        private string _Value;

        [DataMember]
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        private string _Status;

        [DataMember]
        public string Status
        {
            get { return _Status; }
            set { _Status = Status; }
        }

        private string _Id;
        [DataMember]
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public DropDownListItem()
        {
        }

        public DropDownListItem(String jsonObject)
        {
            DropDownListItem ddlItem = JsonConvert.DeserializeObject<DropDownListItem>(jsonObject);
            Selected = ddlItem.Selected;
            Text = ddlItem.Text;
            Value = ddlItem.Value;
            Id = ddlItem.Id;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

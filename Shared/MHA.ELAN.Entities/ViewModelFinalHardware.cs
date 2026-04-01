using System.Runtime.Serialization;

namespace MHA.ELAN.Entities
{
    public class ViewModelFinalHardware
    {
        public ViewModelFinalHardware()
        {
            Final_Hardware = new Final_Hardware();
        }
        [DataMember]
        public Final_Hardware Final_Hardware { get; set; }
    }
}

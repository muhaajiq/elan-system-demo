using System.Runtime.Serialization;

namespace MHA.ELAN.Entities
{
    public class ViewModelFinalITRequirements
    {
        public ViewModelFinalITRequirements()
        {
            Final_ITRequirements = new Final_ITRequirements();
        }
        [DataMember]
        public Final_ITRequirements Final_ITRequirements { get; set; }
    }
}

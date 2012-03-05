namespace Gour.Phone.Models
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Samples.WindowsPhoneCloud.Models", Name = "PushUserServiceRequest")]
    public class PushUserServiceRequest
    {
        [DataMember]
        public Uri ChannelUri { get; set; }

        [DataMember]
        public string ApplicationId { get; set; }

        [DataMember]
        public string DeviceId { get; set; }
    }
}

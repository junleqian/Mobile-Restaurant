namespace Gour.Web.Services
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Gour.Web.Models;

    [ServiceContract]
    public interface ISamplePushUserRegistrationService
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/register",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string Register(PushUserServiceRequest pushUserRegister);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/unregister",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string Unregister(PushUserServiceRequest pushUserRegister);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "/updates?applicationId={applicationId}&deviceId={deviceId}")]
        string[] GetUpdates(string applicationId, string deviceId);
    }
}

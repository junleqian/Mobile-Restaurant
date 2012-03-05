namespace Gour.Web.Services
{
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using Gour.Web.Models;

    [ServiceContract]
    public interface IAuthenticationService
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/login",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string GenerateAuthToken(Login login);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/validate",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string ValidateAuthToken(string token);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/register",
            RequestFormat = WebMessageFormat.Xml,
            ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare)]
        string CreateUser(RegistrationUser createUser);
    }
}
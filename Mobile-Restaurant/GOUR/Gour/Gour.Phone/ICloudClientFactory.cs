namespace Gour.Phone
{
    using Microsoft.Samples.Data.Services.Client;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;
    using Gour.Phone.Push;

    public interface ICloudClientFactory
    {
        string UserName { get; }

        string AuthenticationToken { get; }

        void VerifyLoggedIn(System.Action userAlreadyLoggedInCallback, System.Action userNotLoggedInCallback);

        void SetUserName(string userName, bool persist = false);

        void SetAuthenticationToken(string authToken, bool persist = false);

        IAuthenticationClient ResolveAuthenticationClient();

        ISamplePushUserRegistrationClient ResolvePushNotificationClient();

        System.Data.Services.Client.DataServiceContext ResolveOdataServiceContext();

        void CleanAuthenticationToken();
    }
}

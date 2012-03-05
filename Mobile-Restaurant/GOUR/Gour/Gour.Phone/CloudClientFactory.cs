namespace Gour.Phone
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Threading;
    using Microsoft.Samples.Data.Services.Client;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;
    using Gour.Phone.Push;
    using Gour.Phone.Helpers;

    public class CloudClientFactory : ICloudClientFactory
    {
        private readonly IDictionary<object, object> appResources;
        private readonly Dispatcher dispatcher;

        public CloudClientFactory()
            : this(Application.Current.Resources, Deployment.Current.Dispatcher)
        {
        }

        public CloudClientFactory(IDictionary<object, object> appResources, Dispatcher dispatcher)
        {
            this.appResources = appResources;
            this.dispatcher = dispatcher;

            // Initialize the PushContext.Current instance.
            if (PushContext.Current == null)
            {
                new PushContext(
                    this.appResources["PushChannelName"].ToString(),
                    this.appResources["PushServiceName"].ToString(),
                    new[] { new Uri("https://127.0.0.1") },
                    this.dispatcher);
            }
        }

        public string UserName
        {
            get
            {
                return this.GetValue<string>("UserName");
            }
        }

        public string AuthenticationToken
        {
            get
            {
                return this.GetValue<string>("AuthenticationToken");
            }
        }


        public ISamplePushUserRegistrationClient ResolvePushNotificationClient()
        {
            return new SamplePushUserRegistrationClient(new Uri(this.appResources["PushNotificationServiceEndpoint"].ToString()), this.ResolveStorageCredentials(), this.appResources["ApplicationId"].ToString());
        }

        public System.Data.Services.Client.DataServiceContext ResolveOdataServiceContext()
        {
            var serviceUri = new Uri(this.appResources["SqlOdataEndpoint"].ToString());
            var context = new System.Data.Services.Client.DataServiceContext(serviceUri);

            context.SendingRequest += (s, e) =>
            {
                var credentials = this.ResolveStorageCredentials();

                credentials.AddAuthenticationHeadersLite(null, e.RequestHeaders);
            };

            return context;
        }

        public IStorageCredentials ResolveStorageCredentials()
        {
            return new StorageCredentialsAuthToken(this.AuthenticationToken);
        }

        public IAuthenticationClient ResolveAuthenticationClient()
        {
            return new AuthenticationClient(this.appResources["AuthenticationServiceEndpoint"].ToString());
        }

        public void VerifyLoggedIn(Action userAlreadyLoggedInCallback, Action userNotLoggedInCallback)
        {
            var authToken = this.AuthenticationToken;
            if (string.IsNullOrWhiteSpace(authToken))
            {
                this.dispatcher.BeginInvoke(() => userNotLoggedInCallback());
                return;
            }

            this.ResolveAuthenticationClient().Validate(
                authToken,
                args =>
                {
                    this.dispatcher.BeginInvoke(() => userAlreadyLoggedInCallback());
                },
                args =>
                {
                    this.dispatcher.BeginInvoke(() => userNotLoggedInCallback());
                });
        }

        public void SetUserName(string userName, bool persist = false)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("userName", "The userName cannot be null nor empty.");
            }

            this.SaveValue(userName, "UserName", persist);
        }

        public void SetAuthenticationToken(string authToken, bool persist = false)
        {
            if (string.IsNullOrWhiteSpace(authToken))
            {
                throw new ArgumentNullException("authToken", "The authentication token cannot be null nor empty.");
            }

            this.SaveValue(authToken, "AuthenticationToken", persist);
        }

        public void CleanAuthenticationToken()
        {
            // Remove the authentication token from the phone state dictionary.
            PhoneHelpers.RemoveApplicationState("AuthenticationToken");

            // Remove the authentication token from the phone isolated storage.
            PhoneHelpers.RemoveIsolatedStorageSetting("AuthenticationToken");

        }

        private void SaveValue(object value, string key, bool persist)
        {
            // Save the value in the phone application state.
            PhoneHelpers.SetApplicationState(key, value);

            if (persist)
            {
                // Persist the value in the phone isolated storage.
                PhoneHelpers.SetIsolatedStorageSetting(key, value);
            }
            else
            {
                // Remove old value from the phone isolated storage.
                PhoneHelpers.RemoveIsolatedStorageSetting(key);
            }
        }

        private T GetValue<T>(string key)
        {
            return PhoneHelpers.ContainsApplicationState(key) ? PhoneHelpers.GetApplicationState<T>(key) : PhoneHelpers.GetIsolatedStorageSetting<T>(key);
        }
    }
}

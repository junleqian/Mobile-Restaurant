// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace Microsoft.Samples.CRUDSqlAzure.Phone
{
    using System;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Windows;
    using System.Windows.Threading;
    using Microsoft.Phone.Shell;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;
    using SL.Phone.Federation.Utilities;

    public class CloudClientFactory : ICloudClientFactory
    {
        private readonly IDictionary<string, object> phoneState;
        private readonly IDictionary<string, object> isolatedStorage;
        private readonly IDictionary<object, object> appResources;
        private readonly Dispatcher dispatcher;

        public CloudClientFactory()
            : this(PhoneApplicationService.Current.State, IsolatedStorageSettings.ApplicationSettings, Application.Current.Resources, Deployment.Current.Dispatcher)
        {
        }

        public CloudClientFactory(IDictionary<string, object> phoneState, IDictionary<string, object> isolatedStorage, IDictionary<object, object> appResources, Dispatcher dispatcher)
        {
            this.phoneState = phoneState;
            this.isolatedStorage = isolatedStorage;
            this.appResources = appResources;
            this.dispatcher = dispatcher;
        }

        public RequestSecurityTokenResponseStore TokenStore
        {
            get
            {
                return this.appResources["rstrStore"] as SL.Phone.Federation.Utilities.RequestSecurityTokenResponseStore;
            }
        }

        public NorthwindContext ResolveNorthwindContext()
        {
            var serviceUri = new Uri(this.appResources["NorthwindOdataEndpoint"].ToString());
            var context = new NorthwindContext(serviceUri);

            context.SendingRequest += (s, e) =>
            {
                var credentials = this.ResolveStorageCredentials();

                credentials.AddAuthenticationHeadersLite(null, e.RequestHeaders);
            };

            return context;
        }

        public IRegistrationClient ResolveRegistrationClient()
        {
            return new RegistrationClient(this.appResources["RegistrationServiceEndpoint"].ToString(), this.ResolveStorageCredentials());
        }

        public void CleanAuthenticationToken()
        {
            // Remove the authentication token from the phone state dictionary.
            if (this.TokenStore.ContainsValidRequestSecurityTokenResponse())
            {
                this.TokenStore.RequestSecurityTokenResponse = null;
            }
        }

        private IStorageCredentials ResolveStorageCredentials()
        {
            return new StorageCredentialsSwtToken(
                this.TokenStore.SecurityToken,
                this.TokenStore.RequestSecurityTokenResponse != null ? this.TokenStore.RequestSecurityTokenResponse.expires : 0);
        }
    }
}

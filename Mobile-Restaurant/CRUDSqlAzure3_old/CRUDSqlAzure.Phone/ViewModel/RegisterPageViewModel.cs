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

namespace Microsoft.Samples.CRUDSqlAzure.Phone.ViewModel
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using System.Windows.Threading;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;

    public class RegisterPageViewModel : BaseViewModel
    {
        private readonly IRegistrationClient authenticationClient;
        private readonly Dispatcher dispatcher;

        private Uri sslCertificateUri;
        private string userName;
        private string email;
        private bool isBusy;

        public RegisterPageViewModel() :
            this(App.CloudClientFactory.ResolveRegistrationClient(), Application.Current.Resources["SSLCertificateUrl"] as string, Deployment.Current.Dispatcher)
        {
        }

        public RegisterPageViewModel(IRegistrationClient authenticationClient, string sslCertificateUrl, Dispatcher dispatcher)
        {
            this.authenticationClient = authenticationClient;
            this.dispatcher = dispatcher;

            Uri uri = null;
            Uri.TryCreate(sslCertificateUrl, UriKind.Absolute, out uri);
            this.sslCertificateUri = uri;
        }

        public bool IsSslCertificateLinkVisible
        {
            get
            {
                return this.SslCertificateUri != null;
            }
        }

        public Uri SslCertificateUri
        {
            get
            {
                return this.sslCertificateUri;
            }

            set
            {
                if (this.sslCertificateUri != value)
                {
                    this.sslCertificateUri = value;
                    this.NotifyPropertyChanged("SslCertificateUri");
                    this.NotifyPropertyChanged("IsSslCertificateLinkVisisble");
                }
            }
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }

            set
            {
                if (this.userName != value)
                {
                    this.userName = value;
                    this.NotifyPropertyChanged("UserName");
                    this.NotifyPropertyChanged("IsRegisterEnabled");
                }
            }
        }

        public string EMail
        {
            get
            {
                return this.email;
            }

            set
            {
                if (this.email != value)
                {
                    this.email = value;
                    this.NotifyPropertyChanged("EMail");
                    this.NotifyPropertyChanged("IsRegisterEnabled");
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                if (this.isBusy != value)
                {
                    this.isBusy = value;
                    this.NotifyPropertyChanged("IsBusy");
                }
            }
        }

        public bool IsRegisterEnabled
        {
            get
            {
                return !string.IsNullOrEmpty(this.UserName)
                    && !string.IsNullOrEmpty(this.email);
            }
        }

        public void Register(Action successCallback, Action<string> exceptionCallback)
        {
            this.IsBusy = true;
            try
            {
                this.authenticationClient.Register(
                    this.UserName,
                    this.EMail,
                    args =>
                    {
                        this.dispatcher.BeginInvoke(
                            () =>
                            {
                                successCallback();
                                this.IsBusy = false;
                            });
                    },
                    args =>
                    {
                        this.dispatcher.BeginInvoke(
                            () =>
                            {
                                this.IsBusy = false;

                                var errorMessage = string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Error in registration. Please try again later.\n{0} {1}",
                                    args.Exception.Message,
                                    args.Exception.StatusCode == HttpStatusCode.NotFound ? "Please, make sure to install the SSL certificate in the phone." : string.Empty);

                                exceptionCallback(errorMessage);
                            });
                    });
            }
            catch (Exception exception)
            {
                this.IsBusy = false;
                exceptionCallback(exception.Message);
            }
        }
    }
}
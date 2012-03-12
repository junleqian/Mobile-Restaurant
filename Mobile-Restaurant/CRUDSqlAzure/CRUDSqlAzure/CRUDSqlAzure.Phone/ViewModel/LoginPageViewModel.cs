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
    using System.Windows;
    using System.Windows.Threading;

    public class LoginPageViewModel : BaseViewModel
    {
        private readonly ICloudClientFactory cloudClientFactory;
        private readonly Dispatcher dispatcher;

        private Uri sslCertificateUri;
        private string message;
        private bool isBusy;

        public LoginPageViewModel() :
            this(App.CloudClientFactory, Application.Current.Resources["SSLCertificateUrl"] as string, Deployment.Current.Dispatcher)
        {
        }

        public LoginPageViewModel(ICloudClientFactory cloudClientFactory, string sslCertificateUrl, Dispatcher dispatcher)
        {
            this.cloudClientFactory = cloudClientFactory;
            this.dispatcher = dispatcher;

            Uri uri = null;
            Uri.TryCreate(sslCertificateUrl, UriKind.Absolute, out uri);
            this.sslCertificateUri = uri;
        }

        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.NotifyPropertyChanged("Message");
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

        public void HandleRequestSecurityTokenResponseCompleted(object sender, SL.Phone.Federation.Controls.RequestSecurityTokenResponseCompletedEventArgs e, Action registeredCallback, Action notRegisteredCallback, Action<string> errorCallback)
        {
            if (e.Error == null)
            {
                // check if user is already registered for this application
                this.dispatcher.BeginInvoke(
                    () =>
                    {
                        try
                        {
                            this.IsBusy = true;
                            this.Message = "Checking user registration...";
                            this.cloudClientFactory.ResolveRegistrationClient().CheckUserRegistration(
                                args =>
                                {
                                    this.dispatcher.BeginInvoke(
                                    () =>
                                    {
                                        this.IsBusy = false;
                                        this.Message = "User registered";
                                        registeredCallback();
                                    });
                                },
                                args =>
                                {
                                    this.dispatcher.BeginInvoke(
                                        () =>
                                        {
                                            this.IsBusy = false;
                                            this.Message = "User not registered";
                                            notRegisteredCallback();
                                        });
                                },
                                args =>
                                {
                                    this.dispatcher.BeginInvoke(
                                        () =>
                                        {
                                            this.cloudClientFactory.CleanAuthenticationToken();
                                            this.IsBusy = false;
                                            this.Message = "Error";
                                            errorCallback(string.Empty);
                                        });
                                });
                        }
                        catch (Exception)
                        {
                            this.IsBusy = false;
                            this.Message = "Server Configuration Error";
                            errorCallback("Theres is a problem with the server configuration. Please, contact the application vendor. Exit the application and try again later.");
                        }
                    });
            }
        }
    }
}

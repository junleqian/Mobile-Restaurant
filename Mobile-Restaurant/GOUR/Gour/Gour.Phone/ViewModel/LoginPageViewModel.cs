namespace Gour.Phone.ViewModel
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    public class LoginPageViewModel : BaseViewModel
    {
        private readonly ICloudClientFactory cloudClientFactory;
        private readonly Dispatcher dispatcher;

        private readonly Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials.IAuthenticationClient authenticationClient;
        private Uri sslCertificateUri;
        private string message;
        private string userName;
        private string password;
        private bool rememberMe = true;
        private bool credentialsError;
        private bool isBusy;

        public LoginPageViewModel() :
            this(App.CloudClientFactory, Application.Current.Resources["SSLCertificateUrl"] as string, Deployment.Current.Dispatcher)
        {
        }

        public LoginPageViewModel(ICloudClientFactory cloudClientFactory, string sslCertificateUrl, Dispatcher dispatcher)
        {
            this.cloudClientFactory = cloudClientFactory;
            this.authenticationClient = this.cloudClientFactory.ResolveAuthenticationClient();
            this.dispatcher = dispatcher;

            Uri uri = null;
            Uri.TryCreate(sslCertificateUrl, UriKind.Absolute, out uri);
            this.sslCertificateUri = uri;
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
                    this.NotifyPropertyChanged("IsLoginEnabled");
                    this.CredentialsError = false;
                }
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                if (this.password != value)
                {
                    this.password = value;
                    this.NotifyPropertyChanged("Password");
                    this.NotifyPropertyChanged("IsLoginEnabled");
                    this.CredentialsError = false;
                }
            }
        }

        public bool RememberMe
        {
            get
            {
                return this.rememberMe;
            }

            set
            {
                if (this.rememberMe != value)
                {
                    this.rememberMe = value;
                    this.NotifyPropertyChanged("RememberMe");
                }
            }
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

        public bool IsSslCertificateLinkVisisble
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

        public bool CredentialsError
        {
            get
            {
                return this.credentialsError;
            }

            set
            {
                if (this.credentialsError != value)
                {
                    this.credentialsError = value;
                    this.NotifyPropertyChanged("CredentialsError");
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

        public bool IsLoginEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.UserName) && !string.IsNullOrWhiteSpace(this.Password);
            }
        }

        public void Login(Action successCallback, Action<string> exceptionCallback)
        {
            this.CredentialsError = false;
            this.IsBusy = true;
            this.Message = "Logging in...";
            this.authenticationClient.Login(
                this.UserName,
                this.Password,
                args =>
                {
                    this.cloudClientFactory.SetAuthenticationToken(args.AuthenticationToken, this.RememberMe);
                    this.dispatcher.BeginInvoke(
                        () =>
                        {
                            this.CredentialsError = false;
                            this.IsBusy = false;

                            // Continue flow.
                            successCallback();
                        });
                },
                args =>
                {
                    this.dispatcher.BeginInvoke(
                        () =>
                        {
                            this.IsBusy = false;

                            if (args.WrongCredentials)
                            {
                                this.CredentialsError = true;
                            }
                            else
                            {
                                this.CredentialsError = false;

                                var errorMessage = string.Format(
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    "Error in log in. Please try again later.\n{0} {1}",
                                    args.Exception.Message,
                                    args.Exception.StatusCode == System.Net.HttpStatusCode.NotFound ? "Please, make sure to install the SSL certificate in the phone." : string.Empty);

                                exceptionCallback(errorMessage);
                            }
                        });
                });
        }
    }
}

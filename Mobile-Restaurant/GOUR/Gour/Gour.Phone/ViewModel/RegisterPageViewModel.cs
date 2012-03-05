namespace Gour.Phone.ViewModel
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using System.Windows.Threading;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient.Credentials;

    public class RegisterPageViewModel : BaseViewModel
    {
        private readonly IAuthenticationClient authenticationClient;
        private readonly Dispatcher dispatcher;

        private string userName;
        private string password;
        private string confirmPassword;
        private string email;
        private bool isBusy;

        public RegisterPageViewModel() :
            this(App.CloudClientFactory.ResolveAuthenticationClient(), Deployment.Current.Dispatcher)
        {
        }

        public RegisterPageViewModel(IAuthenticationClient authenticationClient, Dispatcher dispatcher)
        {
            this.authenticationClient = authenticationClient;
            this.dispatcher = dispatcher;
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

        public string ConfirmPassword
        {
            get
            {
                return this.confirmPassword;
            }

            set
            {
                if (this.confirmPassword != value)
                {
                    this.confirmPassword = value;
                    this.NotifyPropertyChanged("ConfirmPassword");
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
                return !string.IsNullOrWhiteSpace(this.UserName)
                    && !string.IsNullOrWhiteSpace(this.email)
                    && !string.IsNullOrWhiteSpace(this.Password)
                    && !string.IsNullOrWhiteSpace(this.ConfirmPassword);
            }
        }

        public void Register(Action userRegisteredCallback, Action<string> userNotRegisteredCallback)
        {
            this.IsBusy = true;
            this.authenticationClient.Register(
                this.UserName,
                this.EMail,
                this.Password,
                this.ConfirmPassword,
                args =>
                {
                    this.dispatcher.BeginInvoke(
                        () =>
                        {
                            this.IsBusy = false;
                            userRegisteredCallback();
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

                            userNotRegisteredCallback(errorMessage);
                        });
                });
        }
    }
}
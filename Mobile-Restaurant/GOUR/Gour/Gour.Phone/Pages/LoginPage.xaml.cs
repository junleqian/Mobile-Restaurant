namespace Gour.Phone.Pages
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Gour.Phone.Helpers;
    using Gour.Phone.ViewModel;

    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            this.InitializeComponent();

            this.PageTransitionReset.Begin();
        }

        public LoginPageViewModel ViewModel
        {
            get { return this.DataContext as LoginPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!PhoneHelpers.GetApplicationState<bool>("UserBackPress"))
            {
                // If there is a valid authentication token available in the isolated storage,
                // then the application navigates to the main pivot page and is registered with 
                // the push notification service in case 'push notification' is enabled by the user.
                // If there is not any valid authentication token available in the isolated storage,
                // then the application navigates to the log in page.
                App.CloudClientFactory.VerifyLoggedIn(
                    () => this.NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative)),
                    () => this.StartTransition());
            }
            else
            {
                this.StartTransition();

                PhoneHelpers.SetApplicationState("UserBackPress", false);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.PageTransitionReset.Begin();
        }

        private void StartTransition()
        {
            this.PageTransitionIn.Begin();
            if (this.ViewModel == null)
            {
                this.ViewModel = new LoginPageViewModel();
            }

            this.ViewModel.UserName = App.CloudClientFactory.UserName ?? this.ViewModel.UserName;
            this.ViewModel.Password = string.Empty;
        }

        private void OnLoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.IsLoginEnabled)
            {
                // When the user navigates to an identity provider, the previous login information is cleaned up.
                App.CloudClientFactory.CleanAuthenticationToken();

                this.ViewModel.Login(
                    () =>
                    {
                        PhoneHelpers.SetApplicationState("userIdentifier", this.ViewModel.UserName);

                        // Save the username of the last user logged in.
                        App.CloudClientFactory.SetUserName(this.ViewModel.UserName, this.ViewModel.RememberMe);

                        this.NavigationService.Navigate(new Uri("/Pages/MainPage.xaml", UriKind.Relative));
                    },
                    msg => MessageBox.Show(msg, "Log in error", MessageBoxButton.OK));
            }
            else
            {
                MessageBox.Show("Please fill out all the information in the log in page.", "Log in incomplete", MessageBoxButton.OK);
            }
        }

        private void OnTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender == this.UserNameTextBox)
                {
                    this.PasswordBox.Focus();
                }
                else
                {
                    this.LoginButton.Focus();
                }
            }
        }

        private void OnRegisterButtonClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/RegisterPage.xaml", UriKind.Relative));
        }
    }
}
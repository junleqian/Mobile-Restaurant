namespace Gour.Phone.Pages
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Phone.Controls;
    using Gour.Phone.Helpers;
    using Gour.Phone.ViewModel;

    public partial class RegisterPage : PhoneApplicationPage
    {
        public RegisterPage()
        {
            this.InitializeComponent();

            this.ViewModel = new RegisterPageViewModel();
        }

        public RegisterPageViewModel ViewModel
        {
            get { return this.DataContext as RegisterPageViewModel; }
            set { this.DataContext = value; }
        }

        private void OnRegisterButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.IsRegisterEnabled)
            {
                this.ViewModel.Register(
                    () =>
                    {
                        MessageBox.Show(
                            string.Format(CultureInfo.CurrentCulture, "User {0} successfully registered.", this.ViewModel.UserName),
                            "Registration Successful",
                            MessageBoxButton.OK);

                        // Clean up authentication token (in case there is any available).
                        App.CloudClientFactory.CleanAuthenticationToken();

                        // Save the username of the last user registered.
                        App.CloudClientFactory.SetUserName(this.ViewModel.UserName);

                        this.NavigationService.GoBack();
                    },
                    msg =>
                    {
                        MessageBox.Show(msg, "Registration Error", MessageBoxButton.OK);
                    });
            }
            else
            {
                MessageBox.Show("Please fill out all the information in the registration page.", "Registration Incomplete", MessageBoxButton.OK);
            }
        }

        private void OnTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender == this.UserNameTextBox)
                {
                    this.EMailTextBox.Focus();
                }
                else if (sender == this.EMailTextBox)
                {
                    this.PasswordBox.Focus();
                }
                else if (sender == this.PasswordBox)
                {
                    this.ConfirmPasswordBox.Focus();
                }
                else
                {
                    this.RegisterButton.Focus();
                }
            }
        }
    }
}
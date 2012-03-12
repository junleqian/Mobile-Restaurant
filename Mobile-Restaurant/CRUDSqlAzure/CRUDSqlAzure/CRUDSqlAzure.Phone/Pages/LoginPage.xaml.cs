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

namespace Microsoft.Samples.CRUDSqlAzure.Phone.Pages
{
    using System;
    using System.Windows;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Helpers;
    using Microsoft.Samples.CRUDSqlAzure.Phone.ViewModel;
    using SL.Phone.Federation.Utilities;

    public partial class LoginPage : PhoneApplicationPage
    {
        private readonly RequestSecurityTokenResponseStore rstrStore = App.Current.Resources["rstrStore"] as RequestSecurityTokenResponseStore;

        public LoginPage()
        {
            this.InitializeComponent();

            this.PageTransitionReset.Begin();

            this.SignInControl.RequestSecurityTokenResponseCompleted +=
                (s, e) => this.ViewModel.HandleRequestSecurityTokenResponseCompleted(
                    s,
                    e,
                    () =>
                    {
                        PhoneHelpers.SetIsolatedStorageSetting("UserIsRegistered", true);
                        //this.NavigationService.Navigate(new Uri("/Pages/ProductsPage.xaml", UriKind.Relative));
                        this.NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                    },
                    () =>
                    {
                        PhoneHelpers.SetIsolatedStorageSetting("UserIsRegistered", false);
                        this.NavigationService.Navigate(new Uri("/Pages/RegisterPage.xaml", UriKind.Relative));
                    },
                    errorMessage =>
                    {
                        this.SignInControl.GetSecurityToken();

                        errorMessage = string.IsNullOrEmpty(errorMessage)
                            ? "An error occurred determining if the user was already registered. Make sure that the appropriate SSL certificate is installed."
                            : errorMessage;
                        MessageBox.Show(errorMessage, "Registration Error", MessageBoxButton.OK);
                    });
            this.SignInControl.NavigatingToIdentityProvider +=
                (s, e) =>
                {
                    // When the user navigates to an identity provider, the previous login information is cleaned up.
                    PhoneHelpers.SetIsolatedStorageSetting("UserIsRegistered", false);
                    App.CloudClientFactory.CleanAuthenticationToken();
                };
        }

        public LoginPageViewModel ViewModel
        {
            get { return this.DataContext as LoginPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.rstrStore.IsValid() && PhoneHelpers.GetIsolatedStorageSetting<bool>("UserIsRegistered") && !PhoneHelpers.GetApplicationState<bool>("UserBackPress"))
            {
                this.NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
            }
            else
            {
                this.StartTransition();

                PhoneHelpers.SetApplicationState("UserBackPress", false);

                this.SignInControl.GetSecurityToken();
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
        }
    }
}
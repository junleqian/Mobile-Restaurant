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
    using System.Globalization;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Phone.Controls;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Helpers;
    using Microsoft.Samples.CRUDSqlAzure.Phone.ViewModel;

    public partial class RegisterPage : PhoneApplicationPage
    {
        private const string NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        private const string EmailClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

        public RegisterPage()
        {
            this.InitializeComponent();

            this.ViewModel = new RegisterPageViewModel();
            var items = ParseQueryString(App.CloudClientFactory.TokenStore.SecurityToken);
            var claimsUserName = items[System.Net.HttpUtility.UrlEncode(NameClaimType)];
            var claimsEmail = items[System.Net.HttpUtility.UrlEncode(EmailClaimType)];
            this.ViewModel.UserName = string.IsNullOrEmpty(claimsUserName) ? string.Empty : claimsUserName;
            this.ViewModel.EMail = string.IsNullOrEmpty(claimsEmail) ? string.Empty : claimsEmail;
        }

        public RegisterPageViewModel ViewModel
        {
            get { return this.DataContext as RegisterPageViewModel; }
            set { this.DataContext = value; }
        }

        private static System.Net.WebHeaderCollection ParseQueryString(string queryString)
        {
            var res = new System.Net.WebHeaderCollection();
            int num = (queryString != null) ? queryString.Length : 0;
            for (int i = 0; i < num; i++)
            {
                int startIndex = i;
                int num4 = -1;
                while (i < num)
                {
                    char ch = queryString[i];
                    if (ch == '=')
                    {
                        if (num4 < 0)
                        {
                            num4 = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }

                    i++;
                }

                var str = string.Empty;
                var str2 = string.Empty;
                if (num4 >= 0)
                {
                    str = queryString.Substring(startIndex, num4 - startIndex);
                    str2 = queryString.Substring(num4 + 1, (i - num4) - 1);
                }
                else
                {
                    str2 = queryString.Substring(startIndex, i - startIndex);
                }

                res[str.Replace("?", string.Empty)] = System.Net.HttpUtility.UrlDecode(str2);
            }

            return res;
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

                        PhoneHelpers.SetIsolatedStorageSetting("UserIsRegistered", true);
                        PhoneHelpers.SetIsolatedStorageSetting("reg_username", this.ViewModel.UserName);
                        PhoneHelpers.SetIsolatedStorageSetting("reg_email", this.ViewModel.EMail);
                        PhoneHelpers.SetIsolatedStorageSetting("reg_id", -1);
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
                else
                {
                    this.RegisterButton.Focus();
                }
            }
        }
    }
}
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
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Helpers;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
    using Microsoft.Samples.CRUDSqlAzure.Phone.ViewModel;

    public partial class ProfileDetailsPage : PhoneApplicationPage
    {
        public ProfileDetailsPage()
        {
            this.InitializeComponent();

            this.ViewModel = new CustomerDetailsPageViewModel();
            this.ViewModel.SaveChangesSuccess += (s, e) => this.NavigationService.GoBack();
        }

        public CustomerDetailsPageViewModel ViewModel
        {
            get { return this.DataContext as CustomerDetailsPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var editCategoryAsString = this.NavigationContext.QueryString["editProfile"];
            bool editCategory;

            if (string.IsNullOrWhiteSpace(editCategoryAsString) || !bool.TryParse(editCategoryAsString, out editCategory))
            {
                editCategory = false;
            }

            if (this.ViewModel.Customer == null)
            {
                var customer = PhoneHelpers.GetApplicationState<Customer>("CurrentProfile");
                if (editCategory && (customer == null))
                {
                    this.NavigationService.GoBack();
                }

                this.ViewModel.SetCustomerModel(customer);
                PhoneHelpers.RemoveApplicationState("CurrentProfile");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.ViewModel.DetachCustomerModel();
        }

        private void OnDeleteCategory(object sender, EventArgs e)
        {
            this.ViewModel.DeleteCustomer();
        }

        private void OnSaveCategory(object sender, EventArgs e)
        {
            var focusTextbox = FocusManager.GetFocusedElement() as TextBox;
            if (focusTextbox != null)
            {
                var binding = focusTextbox.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }

            this.ViewModel.SaveCustomer();
        }

        private void OnNavigatePage(object sender, NavigationEventArgs e)
        {
            this.NavigationService.Navigate(e.Uri);
        }
    }
}
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

    public partial class SupplierDetailsPage : PhoneApplicationPage
    {
        public SupplierDetailsPage()
        {
            this.InitializeComponent();

            this.ViewModel = new SupplierDetailsPageViewModel();
            this.ViewModel.SaveChangesSuccess += (s, e) => this.NavigationService.GoBack();
        }

        public SupplierDetailsPageViewModel ViewModel
        {
            get { return this.DataContext as SupplierDetailsPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var editSupplierAsString = this.NavigationContext.QueryString["editSupplier"];
            bool editSupplier;

            if (string.IsNullOrWhiteSpace(editSupplierAsString) || !bool.TryParse(editSupplierAsString, out editSupplier))
            {
                editSupplier = false;
            }

            if (this.ViewModel.Supplier == null)
            {
                var supplier = PhoneHelpers.GetApplicationState<Supplier>("CurrentSupplierRow");
                if (editSupplier && (supplier == null))
                {
                    this.NavigationService.GoBack();
                }

                this.ViewModel.SetSupplierModel(supplier);
                PhoneHelpers.RemoveApplicationState("CurrentSupplierRow");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.ViewModel.DetachSupplierModel();
        }

        private void OnDeleteSupplier(object sender, EventArgs e)
        {
            this.ViewModel.DeleteSupplier();
        }

        private void OnSaveSupplier(object sender, EventArgs e)
        {
            var focusTextbox = FocusManager.GetFocusedElement() as TextBox;
            if (focusTextbox != null)
            {
                var binding = focusTextbox.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }

            this.ViewModel.SaveSupplier();
        }

        private void OnNavigatePage(object sender, NavigationEventArgs e)
        {
            this.NavigationService.Navigate(e.Uri);
        }
    }
}
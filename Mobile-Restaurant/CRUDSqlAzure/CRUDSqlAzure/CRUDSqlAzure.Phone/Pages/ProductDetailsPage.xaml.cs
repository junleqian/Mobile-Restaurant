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

    public partial class ProductDetailsPage : PhoneApplicationPage
    {
        public ProductDetailsPage()
        {
            this.InitializeComponent();

            this.ViewModel = new ProductDetailsPageViewModel();
            this.ViewModel.SaveChangesSuccess += (s, e) => this.NavigationService.GoBack();
        }

        public ProductDetailsPageViewModel ViewModel
        {
            get { return this.DataContext as ProductDetailsPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var editProductAsString = this.NavigationContext.QueryString["editProduct"];
            bool editProduct;

            if (string.IsNullOrWhiteSpace(editProductAsString) || !bool.TryParse(editProductAsString, out editProduct))
            {
                editProduct = false;
            }

            if (this.ViewModel.Product == null)
            {
                var product = PhoneHelpers.GetApplicationState<Product>("CurrentProductRow");
                if (editProduct && (product == null))
                {
                    this.NavigationService.GoBack();
                }

                this.ViewModel.SetProductModel(product);
                PhoneHelpers.RemoveApplicationState("CurrentProductRow");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.ViewModel.DetachProductModel();
        }

        private void OnDeleteProduct(object sender, EventArgs e)
        {
            this.ViewModel.DeleteProduct();
        }

        private void OnSaveProduct(object sender, EventArgs e)
        {
            var focusTextbox = FocusManager.GetFocusedElement() as TextBox;
            if (focusTextbox != null)
            {
                var binding = focusTextbox.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }

            this.ViewModel.SaveProduct();
        }

        private void OnNavigatePage(object sender, NavigationEventArgs e)
        {
            this.NavigationService.Navigate(e.Uri);
        }
    }
}
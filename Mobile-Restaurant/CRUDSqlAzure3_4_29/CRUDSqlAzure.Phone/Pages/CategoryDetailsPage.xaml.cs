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

    public partial class CategoryDetailsPage : PhoneApplicationPage
    {
        public CategoryDetailsPage()
        {
            this.InitializeComponent();

            this.ViewModel = new CategoryDetailsPageViewModel();
            this.ViewModel.SaveChangesSuccess += (s, e) => this.NavigationService.GoBack();
        }

        public CategoryDetailsPageViewModel ViewModel
        {
            get { return this.DataContext as CategoryDetailsPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var editCategoryAsString = this.NavigationContext.QueryString["editCategory"];
            bool editCategory;

            if (string.IsNullOrWhiteSpace(editCategoryAsString) || !bool.TryParse(editCategoryAsString, out editCategory))
            {
                editCategory = false;
            }

            if (this.ViewModel.Category == null)
            {
                var category = PhoneHelpers.GetApplicationState<Category>("CurrentCategoryRow");
                if (editCategory && (category == null))
                {
                    this.NavigationService.GoBack();
                }

                this.ViewModel.SetCategoryModel(category);
                PhoneHelpers.RemoveApplicationState("CurrentCategoryRow");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.ViewModel.DetachCategoryModel();
        }

        private void OnDeleteCategory(object sender, EventArgs e)
        {
            this.ViewModel.DeleteCategory();
        }

        private void OnSaveCategory(object sender, EventArgs e)
        {
            var focusTextbox = FocusManager.GetFocusedElement() as TextBox;
            if (focusTextbox != null)
            {
                var binding = focusTextbox.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }

            this.ViewModel.SaveCategory();
        }

        private void OnNavigatePage(object sender, NavigationEventArgs e)
        {
            this.NavigationService.Navigate(e.Uri);
        }
    }
}
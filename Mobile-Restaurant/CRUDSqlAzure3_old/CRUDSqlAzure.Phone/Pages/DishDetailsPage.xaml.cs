// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, dishs, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, dish, domain name, email address, logo, person,
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

    public partial class DishDetailsPage : PhoneApplicationPage
    {
        public DishDetailsPage()
        {
            this.InitializeComponent();

            this.ViewModel = new DishDetailsPageViewModel();
            this.ViewModel.SaveChangesSuccess += (s, e) => this.NavigationService.GoBack();
        }

        public DishDetailsPageViewModel ViewModel
        {
            get { return this.DataContext as DishDetailsPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var editDishAsString = this.NavigationContext.QueryString["editDish"];
            bool editDish;

            if (string.IsNullOrWhiteSpace(editDishAsString) || !bool.TryParse(editDishAsString, out editDish))
            {
                editDish = false;
            }

            if (this.ViewModel.Dish == null)
            {
                var dish = PhoneHelpers.GetApplicationState<Dish>("CurrentDishRow");
                if (editDish && (dish == null))
                {
                    this.NavigationService.GoBack();
                }

                this.ViewModel.SetDishModel(dish);
                PhoneHelpers.RemoveApplicationState("CurrentDishRow");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.ViewModel.DetachDishModel();
        }

        private void OnDeleteDish(object sender, EventArgs e)
        {
            this.ViewModel.DeleteDish();
        }

        private void OnSaveDish(object sender, EventArgs e)
        {
            var focusTextbox = FocusManager.GetFocusedElement() as TextBox;
            if (focusTextbox != null)
            {
                var binding = focusTextbox.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }

            this.ViewModel.SaveDish();
        }

        private void OnNavigatePage(object sender, NavigationEventArgs e)
        {
            this.NavigationService.Navigate(e.Uri);
        }
    }
}
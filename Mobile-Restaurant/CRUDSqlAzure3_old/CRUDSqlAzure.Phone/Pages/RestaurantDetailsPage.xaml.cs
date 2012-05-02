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

    public partial class RestaurantDetailsPage : PhoneApplicationPage
    {
        public RestaurantDetailsPage()
        {
            this.InitializeComponent();

            this.ViewModel = new RestaurantDetailsPageViewModel();
            this.ViewModel.SaveChangesSuccess += (s, e) => this.NavigationService.GoBack();
        }

        public RestaurantDetailsPageViewModel ViewModel
        {
            get { return this.DataContext as RestaurantDetailsPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var editRestaurantAsString = this.NavigationContext.QueryString["editRestaurant"];
            bool editRestaurant;

            if (string.IsNullOrWhiteSpace(editRestaurantAsString) || !bool.TryParse(editRestaurantAsString, out editRestaurant))
            {
                editRestaurant = false;
            }

            if (this.ViewModel.Restaurant == null)
            {
                var restaurant = PhoneHelpers.GetApplicationState<Restaurant>("CurrentRestaurantRow");
                if (editRestaurant && (restaurant == null))
                {
                    this.NavigationService.GoBack();
                }

                this.ViewModel.SetRestaurantModel(restaurant);
                PhoneHelpers.RemoveApplicationState("CurrentRestaurantRow");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.ViewModel.DetachRestaurantModel();
        }

        private void OnDeleteRestaurant(object sender, EventArgs e)
        {
            this.ViewModel.DeleteRestaurant();
        }

        private void OnSaveRestaurant(object sender, EventArgs e)
        {
            var focusTextbox = FocusManager.GetFocusedElement() as TextBox;
            if (focusTextbox != null)
            {
                var binding = focusTextbox.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }

            this.ViewModel.SaveRestaurant();
        }

        private void OnNavigatePage(object sender, NavigationEventArgs e)
        {
            this.NavigationService.Navigate(e.Uri);
        }
    }
}
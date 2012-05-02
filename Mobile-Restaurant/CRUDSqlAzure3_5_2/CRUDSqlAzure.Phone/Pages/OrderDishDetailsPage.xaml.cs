// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, Dishes, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, Dish, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace Microsoft.Samples.CRUDSqlAzure.Phone.Pages
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Helpers;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
    using Microsoft.Samples.CRUDSqlAzure.Phone.ViewModel;

    public partial class OrderDishDetailsPage : PhoneApplicationPage
    {
        public OrderDishDetailsPage()
        {
            this.InitializeComponent();
            this.ViewModel = new OrderDishDetailsPageViewModel();
            this.Loaded += (s, e) => this.ViewModel.LoadData();
            this.Loaded += (s, e) => this.ViewModel.LoadOdData();
            this.ViewModel.odItems.LoadCompleted += new EventHandler<System.Data.Services.Client.LoadCompletedEventArgs>(odItems_LoadCompleted);
            this.ViewModel.SaveChangesSuccess += new EventHandler(ViewModel_SaveChangesSuccess);
        }

        void ViewModel_SaveChangesSuccess(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            //NavigationService.GoBack();
        }

        void odItems_LoadCompleted(object sender, System.Data.Services.Client.LoadCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            if (this.ViewModel.odItems.Count == 1)
            {
                itemQuantity = this.ViewModel.odItems[0].Quantity;
                DishQuantity.Text = itemQuantity.ToString();
                DishQuantity.Opacity = 1;
                LessQuantity.Opacity = 1;
                MoreQuantity.Opacity = 1;
                qEnabled = true;
            }
            else if (this.ViewModel.odItems.Count == 0)
            {
                DishQuantity.Text = "-";
            }
        }

        public int itemQuantity = 0;
        public bool qEnabled = false;

        public OrderDishDetailsPageViewModel ViewModel
        {
            get { return this.DataContext as OrderDishDetailsPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            PhoneHelpers.SetApplicationState("UserBackPress", true);
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.ViewModel.PageNumber = (int)PhoneHelpers.GetApplicationState<int>("CurrentPageNumber");
            string RestIDasString = this.NavigationContext.QueryString["RestaurantID"];
            this.ViewModel.RestID = int.Parse(RestIDasString);
            string DishIDasString = this.NavigationContext.QueryString["DishID"];
            this.ViewModel.DishID = int.Parse(DishIDasString);
            string ReservationIDasString = this.NavigationContext.QueryString["ReservationID"];
            this.ViewModel.ReservationID = int.Parse(ReservationIDasString);
            PageTitle.Text = this.NavigationContext.QueryString["DishName"];

            if (this.ViewModel.Dish == null)
            {
                var dish = PhoneHelpers.GetApplicationState<Dish>("CurrentRestaurantDishRow");
                if (dish == null)
                {
                    this.NavigationService.GoBack();
                }
                this.ViewModel.SetDishModel(dish);
                PhoneHelpers.RemoveApplicationState("CurrentRestaurantDishRow");
                //TODO: technically these should be set using data binding
                DishPrice.Text = '$' + String.Format("{0:0.00}", dish.UnitPrice);
                //DishDescription.Text = dish.Description;
                DishDescription.DataContext = dish;
                DishImage.DataContext = dish;
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (this.ViewModel != null)
            {
                PhoneHelpers.SetApplicationState("CurrentPageNumber", this.ViewModel.PageNumber);
            }
            if (this.ViewModel.qPrevious != itemQuantity)
            {
                if (itemQuantity == 0)
                    this.ViewModel.DeleteOrderDetails();
                else
                    this.ViewModel.saveOrderDetails();
                //NavigationService.StopLoading();
            }
        }

        private void ItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void LoadMoreItems(object sender, RoutedEventArgs e)
        {
            this.ViewModel.LoadNextPage();
        }

        private void OnAddRating(object sender, EventArgs e)
        {
            //PhoneHelpers.RemoveApplicationState("CurrentDishRow");
            //this.NavigationService.Navigate(new Uri("/Pages/DishDetailsPage.xaml?editDish=false", UriKind.Relative));
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            this.ViewModel.LoadData();
        }

        private void OnLogout(object sender, EventArgs e)
        {
            this.CleanUp();
        }
        
        private void CleanUp()
        {
            // Clean the current authentication token, flags and view models.
            App.CloudClientFactory.CleanAuthenticationToken();
            PhoneHelpers.SetApplicationState("UserBackPress", false);
            PhoneHelpers.RemoveIsolatedStorageSetting("UserIsRegistered");

            this.ViewModel = null;

            // Navigate to the log in page.
            this.NavigationService.GoBack();
        }

        private void LessQuantity_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (itemQuantity > 0 & qEnabled == true)
            {
                itemQuantity--;
                DishQuantity.Text = itemQuantity.ToString();
                this.ViewModel.odItems[0].Quantity = (short)itemQuantity;
            }
        }

        private void MoreQuantity_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (itemQuantity < 9 & qEnabled == true)
            {
                itemQuantity++;
                DishQuantity.Text = itemQuantity.ToString();
                this.ViewModel.odItems[0].Quantity = (short)itemQuantity;
            }
        }

        private void rate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
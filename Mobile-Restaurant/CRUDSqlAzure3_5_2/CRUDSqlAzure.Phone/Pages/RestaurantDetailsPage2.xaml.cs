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

    public partial class RestaurantDetailsPage2 : PhoneApplicationPage
    {
        public RestaurantDetailsPage2()
        {
            this.InitializeComponent();
            this.ViewModel = new RestaurantDetailsPage2ViewModel();

            this.Loaded += (s, e) => this.ViewModel.LoadData();
        }

        public RestaurantDetailsPage2ViewModel ViewModel
        {
            get { return this.DataContext as RestaurantDetailsPage2ViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            PhoneHelpers.SetApplicationState("UserBackPress", true);
            base.OnBackKeyPress(e);
        }

        //byte[] restaurantImageBytes;
        //string restaurantDescription;
        //BitmapImage restaurantImage;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.ViewModel.PageNumber = (int)PhoneHelpers.GetApplicationState<int>("CurrentPageNumber");
            string RestIDasString = this.NavigationContext.QueryString["RestaurantID"];
            this.ViewModel.RestID = int.Parse(RestIDasString);
            this.ViewModel.DishID = -2;
            PageTitle.Text = this.NavigationContext.QueryString["RestaurantName"];

            if (this.ViewModel.Restaurant == null)
            {
                var restaurant = PhoneHelpers.GetApplicationState<Restaurant>("CurrentRestaurantRow");
                if (restaurant == null)
                {
                    this.NavigationService.GoBack();
                }
                this.ViewModel.SetRestaurantModel(restaurant);
                PhoneHelpers.RemoveApplicationState("CurrentRestaurantRow");
                //TODO: technically these should be set using data binding
                //DishDescription.Text = dish.Description;
                restaurantDescription.DataContext = restaurant;
                restaurantHomePage.DataContext = restaurant;
                restaurantImage.DataContext = restaurant;
                //dishDescription = dish.Description;
                //dishImageBytes = dish.Image;
                
                /*
                if (dish.Image != null)
                {
                    //TODO: needs valid bmp image loaded in db
                    MemoryStream memStream = new MemoryStream(dish.Image);
                    memStream.Seek(0, SeekOrigin.Begin);
                    //memStream.Write(dish.Image, 78, dish.Image.Length - 78);
                    dishImage = new BitmapImage();
                    dishImage.SetSource(memStream);
                    DishImage2.Source = dishImage;
                }*/
            }

            /*
            System.Data.Services.Client.DataServiceCollection<Models.Dish> myList = new System.Data.Services.Client.DataServiceCollection<Models.Dish>();
            for (int i = 0; i < this.ViewModel.Items.Count; i++)
            {
                if (this.ViewModel.Items[i].RestaurantID == RestID)
                {
                    myList.Insert(i, this.ViewModel.Items[i]);
                }
            }
            this.ViewModel.Items = myList;
            */
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (this.ViewModel != null)
            {
                PhoneHelpers.SetApplicationState("CurrentPageNumber", this.ViewModel.PageNumber);
            }
        }

        private void ItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var selector = sender as Selector;
            //if ((selector == null) || (selector.SelectedIndex == -1))
            //{
            //    return;
            //}

            //PhoneHelpers.SetApplicationState("CurrentDishRow", this.ViewModel.Items[selector.SelectedIndex]);
            //this.NavigationService.Navigate(new Uri("/Pages/DishDetailsPage.xaml?editDish=true", UriKind.Relative));
            //selector.SelectedIndex = -1;
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //Models.Restaurant temp = this.ViewModel.Restaurant;
            //PhoneHelpers.SetApplicationState("CurrentRestaurantDishRow", temp);

            //PhoneHelpers.SetApplicationState("CurrentRestaurantRow", this.ViewModel.Items[selector.SelectedIndex]);
            int RestID = this.ViewModel.Restaurant.RestaurantID;
            string RestName = this.ViewModel.Restaurant.RestaurantName;
            this.NavigationService.Navigate(new Uri("/Pages/RestaurantDishesPage.xaml?RestaurantID=" + RestID.ToString() + "&RestaurantName=" + RestName + "&isOrder=false", UriKind.Relative));

        }

        private void reserve_Click(object sender, RoutedEventArgs e)
        {
            int RestID = this.ViewModel.Restaurant.RestaurantID;
            string RestName = this.ViewModel.Restaurant.RestaurantName;
            this.NavigationService.Navigate(new Uri("/Pages/ReservationPage.xaml?RestaurantID=" + RestID.ToString() + "&RestaurantName=" + RestName, UriKind.Relative));
            
        }

        private void rate_Click(object sender, RoutedEventArgs e)
        {
            PhoneHelpers.RemoveApplicationState("CurrentReviewRow");
            int RestID = this.ViewModel.Restaurant.RestaurantID;
            this.NavigationService.Navigate(new Uri("/Pages/ReviewDetailsPage.xaml?editReview=false&RestaurantID=" + RestID.ToString(), UriKind.Relative));
        }

    }
}
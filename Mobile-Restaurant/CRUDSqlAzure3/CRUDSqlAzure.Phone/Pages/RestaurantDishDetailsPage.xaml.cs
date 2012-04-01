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

    public partial class RestaurantDishDetailsPage : PhoneApplicationPage
    {
        public RestaurantDishDetailsPage()
        {
            this.InitializeComponent();
            this.ViewModel = new RestaurantDishDetailsPageViewModel();

            this.Loaded += (s, e) => this.ViewModel.LoadData();
        }

        public RestaurantDishDetailsPageViewModel ViewModel
        {
            get { return this.DataContext as RestaurantDishDetailsPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            PhoneHelpers.SetApplicationState("UserBackPress", true);
            base.OnBackKeyPress(e);
        }

        byte[] dishImageBytes;
        string dishDescription;
        BitmapImage dishImage;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.ViewModel.PageNumber = (int)PhoneHelpers.GetApplicationState<int>("CurrentPageNumber");
            string RestIDasString = this.NavigationContext.QueryString["RestaurantID"];
            this.ViewModel.RestID = int.Parse(RestIDasString);
            string DishIDasString = this.NavigationContext.QueryString["DishID"];
            this.ViewModel.DishID = int.Parse(DishIDasString);
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
    }
}
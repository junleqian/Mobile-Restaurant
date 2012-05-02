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
    using System.Device;
    using System.Device.Location;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Helpers;
    using Microsoft.Samples.CRUDSqlAzure.Phone.ViewModel;
    using Microsoft.Phone.Controls.Maps;

    public partial class MapPage : PhoneApplicationPage
    {
        public MapPage()
        {
            this.InitializeComponent();
            this.ViewModel = new RestaurantsPageViewModel();
            this.watcher = new GeoCoordinateWatcher();
            this.watcher.MovementThreshold = 100;
            this.watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            //this.Loaded += (s, e) => this.ViewModel.LoadData();
            this.ViewModel.Items.LoadCompleted += new EventHandler<System.Data.Services.Client.LoadCompletedEventArgs>(Items_LoadCompleted);
            watcher.Start();
        }

        void Items_LoadCompleted(object sender, System.Data.Services.Client.LoadCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            for (int i = 0; i < this.ViewModel.Items.Count; i++)
            {
                if (this.ViewModel.Items[i].Lat != null && this.ViewModel.Items[i].Long != null)
                {
                    Pushpin tempPin = new Pushpin();
                    tempPin.Content = this.ViewModel.Items[i].RestaurantName;
                    tempPin.Location = new GeoCoordinate((float)this.ViewModel.Items[i].Lat, (float)this.ViewModel.Items[i].Long);
                    tempPin.Tag = i;
                    tempPin.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(pin1_Tap);
                    mapMain.Children.Add(tempPin);

                    //((((Lat sub 40.11) mul 69.1) mul ((Lat sub 40.11) mul 69.1)) add (((Long add 88.23) mul 53.0) mul ((Long add 88.23) mul 53.0))) le 1
                }
            }
        }

        GeoCoordinate my_coord;

        public void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            //throw new NotImplementedException();
            // Update the map to show the current location
            my_coord = e.Position.Location;
            mapMain.SetView(e.Position.Location, 17);
            //update pushpin location and show
            Pushpin pin1 = new Pushpin();
            pin1.Location = e.Position.Location;
            pin1.Content = "YOU";
            mapMain.Children.Add(pin1);
            this.ViewModel.searchLat = my_coord.Latitude;
            this.ViewModel.searchLong = my_coord.Longitude;
            this.ViewModel.LoadData();
        }

        void pin1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //throw new NotImplementedException();
            var pin = sender as Pushpin;
            int index = (int)pin.Tag;
            Models.Restaurant temp = this.ViewModel.Items[index];
            PhoneHelpers.SetApplicationState("CurrentRestaurantRow", temp);

            //PhoneHelpers.SetApplicationState("CurrentRestaurantRow", this.ViewModel.Items[selector.SelectedIndex]);
            int RestID = temp.RestaurantID;
            string RestName = temp.RestaurantName;
            this.NavigationService.Navigate(new Uri("/Pages/RestaurantDetailsPage2.xaml?RestaurantID=" + RestID.ToString() + "&RestaurantName=" + RestName, UriKind.Relative));

        }

        GeoCoordinateWatcher watcher; 

        public RestaurantsPageViewModel ViewModel
        {
            get { return this.DataContext as RestaurantsPageViewModel; }
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
            if (this.NavigationContext.QueryString.ContainsKey("SearchName"))
            {
                this.ViewModel.searchName = this.NavigationContext.QueryString["SearchName"];
                this.ViewModel.searchCol = "RestaurantName";
            }
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
            var selector = sender as Selector;
            if ((selector == null) || (selector.SelectedIndex == -1))
            {
                return;
            }
            Models.Restaurant temp = (Models.Restaurant)selector.Items[selector.SelectedIndex];
            PhoneHelpers.SetApplicationState("CurrentRestaurantRow", temp);
            
            //PhoneHelpers.SetApplicationState("CurrentRestaurantRow", this.ViewModel.Items[selector.SelectedIndex]);
            int RestID = this.ViewModel.Items[selector.SelectedIndex].RestaurantID;
            string RestName = this.ViewModel.Items[selector.SelectedIndex].RestaurantName;
            this.NavigationService.Navigate(new Uri("/Pages/RestaurantDetailsPage2.xaml?RestaurantID=" + RestID.ToString() + "&RestaurantName=" + RestName, UriKind.Relative));
            selector.SelectedIndex = -1;
        }

        private void LoadMoreItems(object sender, RoutedEventArgs e)
        {
            this.ViewModel.LoadNextPage();
        }

        private void OnAddRestaurant(object sender, EventArgs e)
        {
            PhoneHelpers.RemoveApplicationState("CurrentRestaurantRow");
            this.NavigationService.Navigate(new Uri("/Pages/RestaurantDetailsPage.xaml?editRestaurant=false", UriKind.Relative));
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
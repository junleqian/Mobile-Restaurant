using System;
using System.Device;
using System.Device.Location;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Data.Services.Client;
using Microsoft.Samples.CRUDSqlAzure.Phone.Helpers;
using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
using Microsoft.Samples.WindowsPhoneCloud.StorageClient;
using System.Windows.Threading;

namespace Microsoft.Samples.CRUDSqlAzure.Phone.Pages
{
    public partial class HomePage : PhoneApplicationPage
    {
        public HomePage()
        {
            InitializeComponent();
            this.Context = App.CloudClientFactory.ResolveNorthwindContext();
            this.Items = new DataServiceCollection<Reservation>(this.Context);
            this.Items.LoadCompleted += this.OnItemsLoadCompleted;
            this.dispatcher = Deployment.Current.Dispatcher;
            this.ItemsListBox.DataContext = Items;
            this.mvMessage.DataContext = Message;
            this.mvProgressBar.DataContext = isListing;
            this.watcher = new GeoCoordinateWatcher();
            this.watcher.MovementThreshold = 100;
            this.watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            //this.Loaded += (s, e) => this.LoadData();
            watcher.Start();
        }

        private readonly Dispatcher dispatcher;
        public DataServiceCollection<Reservation> Items { get; set; }
        protected NorthwindContext Context { get; private set; }
        private bool isListing = false;
        private string message = string.Empty;
        public bool IsListing
        {
            get
            {
                return this.isListing;
            }

            set
            {
                this.isListing = value;
                //this.NotifyPropertyChanged("IsListing");
            }
        }
        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.NotifyPropertyChanged("Message");
                }
            }
        }
        public GeoCoordinateWatcher watcher;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string my_username = PhoneHelpers.GetIsolatedStorageSetting<string>("reg_username");
            string my_email = PhoneHelpers.GetIsolatedStorageSetting<string>("reg_email");
            textBlock1.Text = "username: " + my_username + "\nemail: " + my_email;
            this.LoadData();
            //GetLocationProperty();
        }

        public virtual void LoadData()
        {
            if (!this.IsListing)
            {
                int my_id = PhoneHelpers.GetIsolatedStorageSetting<int>("reg_id");
                if (my_id == -1) //definitely haven't made a profile yet
                {
                    return;
                }
                var requestUri = new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}/Reservations?$filter=CustomerID eq {1}&$orderby=StartTime",
                        this.Context.BaseUri,
                        my_id.ToString()),
                    UriKind.Absolute);

                this.IsListing = true;
                //this.Message = "Loading...";

                try
                {
                    //this.currentPage = 0;
                    this.Items.Clear();
                    ItemsListBox.Items.Clear();
                    mvMessage.Text = "Loading...";
                    mvProgressBar.Opacity = 1;
                    this.Items.LoadAsync(requestUri);
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                    this.IsListing = false;
                    //this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
        }


        private void OnItemsLoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.DispatchResult("The operation has been cancelled.");
            }
            else if (e.Error != null)
            {
                //this.hasError = true;
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(e.Error).Message;
                this.DispatchResult(string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage));
            }
            else
            {
                //erased quite a bit here
                ItemsListBox.Items.Clear();
                for (int i = 0; i < this.Items.Count; i++)
                {
                    ItemsListBox.Items.Add(Items[i]);
                }
                mvMessage.Text = "";
                mvProgressBar.Opacity = 0;
                this.DispatchResult();
            }
        }


        private void ItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = sender as Selector;
            if ((selector == null) || (selector.SelectedIndex == -1))
            {
                return;
            }
            if (selector.SelectedIndex >= this.Items.Count)
                return;
            PhoneHelpers.SetApplicationState("CurrentReservation", this.Items[selector.SelectedIndex]);
            this.NavigationService.Navigate(new Uri("/Pages/ReservationDetailsPage.xaml", UriKind.Relative));
            selector.SelectedIndex = -1;
        }


        private void DispatchResult(string message = "")
        {
            this.dispatcher.BeginInvoke(
                () =>
                {
                    this.IsListing = false;
                    this.Message = message;
                    //this.NotifyPropertyChanged("LoadMoreResultsVisible");
                });
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/DishesPage.xaml", UriKind.Relative));
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/RestaurantsPage.xaml", UriKind.Relative));
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/CategoriesPage.xaml", UriKind.Relative));
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/RestaurantsPage2.xaml", UriKind.Relative));
        }

        private void button5_Click(object sender, RoutedEventArgs e)
        {
            if (searchBox.Text != null)
            {
                //this.NavigationService.Navigate(new Uri("/Pages/RestaurantsPage2.xaml", UriKind.Relative));
                this.NavigationService.Navigate(new Uri("/Pages/RestaurantsPage2.xaml?SearchName=" + searchBox.Text, UriKind.Relative));
            }
        }

        private void button6_Click(object sender, RoutedEventArgs e)
        {
                //this.NavigationService.Navigate(new Uri("/Pages/RestaurantsPage2.xaml", UriKind.Relative));
                this.NavigationService.Navigate(new Uri("/Pages/MapPage.xaml", UriKind.Relative));
        }

        public void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            GeoCoordinate coord = e.Position.Location;

            if (coord.IsUnknown != true)
            {
                locationBlock.Text = "Lat: " + coord.Latitude.ToString() + "\nLong: " + coord.Longitude.ToString();
                //Console.WriteLine("Lat: {0}, Long: {1}", coord.Latitude, coord.Longitude);
            }
        }

        public void GetLocationProperty()
        {
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();

            // Do not suppress prompt, and wait 1000 milliseconds to start.
            watcher.TryStart(false, TimeSpan.FromMilliseconds(1000));

            GeoCoordinate coord = watcher.Position.Location;

            if (coord.IsUnknown != true)
            {
                locationBlock.Text = "Lat: " + coord.Latitude.ToString() + " Long: " + coord.Longitude.ToString();
                //Console.WriteLine("Lat: {0}, Long: {1}", coord.Latitude, coord.Longitude);
            }
            else
            {
                locationBlock.Text = "Unknown location";
                //Console.WriteLine("Unknown latitude and longitude.");
            }
        }

        private void profile_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/ProfilePage.xaml", UriKind.Relative));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
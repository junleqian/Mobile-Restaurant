using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Data.Services.Client;
using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
using Microsoft.Samples.CRUDSqlAzure.Phone.Helpers;
using Microsoft.Samples.WindowsPhoneCloud.StorageClient;

namespace Microsoft.Samples.CRUDSqlAzure.Phone.Pages
{
    public partial class ReservationDetailsPage : PhoneApplicationPage
    {
        public ReservationDetailsPage()
        {
            InitializeComponent();
            this.Context = App.CloudClientFactory.ResolveNorthwindContext();
            this.Items = new DataServiceCollection<Restaurant>(this.Context);
            this.Items.LoadCompleted += this.OnItemsLoadCompleted;
            this.Loaded += (s, e) => this.LoadData();

        }

        public DataServiceCollection<Restaurant> Items { get; set; }
        protected NorthwindContext Context { get; private set; }
        private bool islisting = false;
        public bool IsListing
        {
            get
            {
                return this.islisting;
            }

            set
            {
                this.islisting = value;
                //this.NotifyPropertyChanged("IsListing");
            }
        }

        Reservation my_reservation = null;
        public bool isListing = false;
        public string Message = "";

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (my_reservation == null)
            {
                my_reservation = PhoneHelpers.GetApplicationState<Reservation>("CurrentReservation");
                nameBox.Text = my_reservation.RestaurantName;
                DateBox.Text += my_reservation.StartTime.ToShortDateString();
                TimeBox.Text += my_reservation.StartTime.ToShortTimeString();
                // my_reservation.NumGuests.ToString();
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        public virtual void LoadData()
        {
            if (!this.IsListing)
            {
                var requestUri = new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}/Restaurants({1})",
                        this.Context.BaseUri,
                        my_reservation.RestaurantID),
                    UriKind.Absolute);

                this.IsListing = true;
                //this.Message = "Loading...";

                try
                {
                    //this.currentPage = 0;
                    this.Items.Clear();
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
                //this.DispatchResult("The operation has been cancelled.");
            }
            else if (e.Error != null)
            {
                //this.hasError = true;
                //var errorMessage = StorageClientExceptionParser.ParseDataServiceException(e.Error).Message;
                //this.DispatchResult(string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage));
            }
            else
            {
                //erased quite a bit here
                //this.DispatchResult();
                if (this.Items.Count == 1)
                {
                    //extract info
                    isListing = false;
                    mvProgressBar.Opacity = 0;
                    AddressBox.Text += Items[0].Address;
                    CityBox.Text += Items[0].City;
                    PhoneBox.Text += Items[0].Phone;
                    if (Items[0].Image == null)
                    {
                        BitmapImage defaultImage = new BitmapImage(new System.Uri("..\\Images\\gour_noimage.png", UriKind.RelativeOrAbsolute));
                        restaurantImage.Source = defaultImage;
                        return;
                    }

                    Stream memStream = new MemoryStream(Items[0].Image);
                    memStream.Seek(0, SeekOrigin.Begin);
                    BitmapImage empImage = new BitmapImage();
                    empImage.SetSource(memStream);
                    restaurantImage.Source = empImage;
                    button1.IsEnabled = true;
                    return;
                }
                else
                {
                    //something else went wrong
                    throw new NotImplementedException("restaurant query didn't work apparently");
                }
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            //OR WAIT---perhaps we can just use reservationID rather than orderID
            this.NavigationService.Navigate(new Uri("/Pages/OrderDishesPage.xaml?RestaurantID=" + my_reservation.RestaurantID.ToString() + "&ReservationID=" + my_reservation.ReservationID.ToString(), UriKind.Relative));
            return;
            /*
            if (my_reservation.OrderID == null)
            {
                //create new order
                Order order = new Order();
                order.CustomerID = my_reservation.CustomerID;
                order.OrderDate = my_reservation.StartTime;
                order.ReservationID = my_reservation.ReservationID;
                order.RestaurantID = my_reservation.RestaurantID;
                Context.AddToOrders(order);
                Context.AttachToOrders(order);
            }
            else
            {
                //use existing order
            }*/
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
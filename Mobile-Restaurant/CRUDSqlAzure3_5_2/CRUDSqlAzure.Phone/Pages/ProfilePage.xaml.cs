using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
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
    public partial class ProfilePage : PhoneApplicationPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            this.Context = App.CloudClientFactory.ResolveNorthwindContext();
            this.Items = new DataServiceCollection<Customer>(this.Context);
            this.Items.LoadCompleted += this.OnItemsLoadCompleted;
            this.Loaded += (s, e) => this.LoadData();
        }

        public DataServiceCollection<Customer> Items { get; set; }
        protected NorthwindContext Context { get; private set; }
        private bool isListing = false;
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
        bool done = false;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (done && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            done = true;
        }

        public virtual void LoadData()
        {
            textBlock1.Text = "looking up your profile...";
            if (!this.IsListing)
            {
                string my_username = PhoneHelpers.GetIsolatedStorageSetting<string>("reg_username");
                string my_email = PhoneHelpers.GetIsolatedStorageSetting<string>("reg_email");
                int my_id = PhoneHelpers.GetIsolatedStorageSetting<int>("reg_id");
                if (my_id == -1) //definitely haven't made a profile yet
                {
                    PhoneHelpers.RemoveApplicationState("CurrentProfile");
                    this.NavigationService.Navigate(new Uri("/Pages/ProfileDetailsPage.xaml?editProfile=false", UriKind.Relative));
                }

                //Customers?$filter=Name eq 'Nicholas Ewalt' and Email eq 'ewalt1@hotmail.com'
                /*
                var requestUri = new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}/Customers?$filter=Name eq '{1}' and Email eq '{2}'",
                        this.Context.BaseUri,
                        my_username,
                        my_email),
                    UriKind.Absolute);
                */
                var requestUri = new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}/Customers({1})",
                        this.Context.BaseUri,
                        my_id.ToString()),
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
                    //we have an account, use this one to modify
                    textBlock1.Text += "\nProfile found, going to edit page";
                    PhoneHelpers.SetApplicationState("CurrentProfile", this.Items[0]);
                    this.NavigationService.Navigate(new Uri("/Pages/ProfileDetailsPage.xaml?editProfile=true", UriKind.Relative));
                }
                else if (this.Items.Count == 0)
                {
                    //we need to account to db
                    textBlock1.Text += "\nNo profile found, creating new one";
                    PhoneHelpers.RemoveApplicationState("CurrentProfile");
                    this.NavigationService.Navigate(new Uri("/Pages/ProfileDetailsPage.xaml?editProfile=false", UriKind.Relative));
                    
                }
                else
                {
                    //more than one account something was wrong with query or something else went wrong
                    textBlock1.Text += "More than one profile that matches your credentials exists in the database\nWTF did you do?!?!?!?!??";
                    throw new NotImplementedException("more than one account with this info found");
                }
            }
        }
    }
}
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

namespace Microsoft.Samples.CRUDSqlAzure.Phone.ViewModel
{
    using System.Windows;
    using System.Windows.Threading;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient;

    public class RestaurantDetailsPage2ViewModel : PartialListViewModel<Review>
    {

        private Restaurant restaurant;

        public RestaurantDetailsPage2ViewModel()
            : this(Deployment.Current.Dispatcher, App.CloudClientFactory.ResolveNorthwindContext())
        {
        }

        public RestaurantDetailsPage2ViewModel(Dispatcher dispatcher, NorthwindContext northwindContext)
            : base(dispatcher, northwindContext)
        {
        }

        public Restaurant Restaurant
        {
            get
            {
                return this.restaurant;
            }

            set
            {
                if (this.restaurant != value)
                {
                    this.restaurant = value;
                    this.NotifyPropertyChanged("Restaurant");
                }
            }
        }

        public void SetRestaurantModel(Restaurant restaurant = null)
        {
            if (restaurant == null)
            {
                this.Restaurant = new Restaurant();
                try
                {
                    this.Context.AddToRestaurants(this.Restaurant);
                }
                catch (System.Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                    this.Message = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
            else
            {
                this.Restaurant = restaurant;
                //shouldn't really be able to reach this point so it should be okay
                //this.Restaurant.PropertyChanged += this.OnUpdateRestaurant;
            }
        }

        protected override string EntitySetName
        {
            get
            {
                return "Reviews";
            }
        }
    }
}

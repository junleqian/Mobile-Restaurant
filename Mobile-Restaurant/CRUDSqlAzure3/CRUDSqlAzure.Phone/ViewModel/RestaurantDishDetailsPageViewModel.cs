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

    public class RestaurantDishDetailsPageViewModel : PartialListViewModel<Rating>
    {

        private Dish dish;

        public RestaurantDishDetailsPageViewModel()
            : this(Deployment.Current.Dispatcher, App.CloudClientFactory.ResolveNorthwindContext())
        {
        }

        public RestaurantDishDetailsPageViewModel(Dispatcher dispatcher, NorthwindContext northwindContext)
            : base(dispatcher, northwindContext)
        {
        }

        public Dish Dish
        {
            get
            {
                return this.dish;
            }

            set
            {
                if (this.dish != value)
                {
                    this.dish = value;
                    this.NotifyPropertyChanged("Dish");
                }
            }
        }

        public void SetDishModel(Dish dish = null)
        {
            if (dish == null)
            {
                this.Dish = new Dish();
                try
                {
                    this.Context.AddToDishes(this.Dish);
                }
                catch (System.Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                    this.Message = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
            else
            {
                this.Dish = dish;
                //shouldn't really be able to reach this point so it should be okay
                //this.Dish.PropertyChanged += this.OnUpdateDish;
            }
        }

        protected override string EntitySetName
        {
            get
            {
                return "Ratings";
            }
        }
    }
}

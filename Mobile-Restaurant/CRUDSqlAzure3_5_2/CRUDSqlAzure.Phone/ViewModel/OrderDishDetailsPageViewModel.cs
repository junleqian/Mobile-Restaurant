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
    using System;
    using System.Linq;
    using System.Globalization;
    using System.Windows;
    using System.Data.Services.Client;
    using System.Collections.Generic;
    using System.Windows.Threading;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient;

    public class OrderDishDetailsPageViewModel : PartialListViewModel<Rating>
    {

        private Dish dish;

        public OrderDishDetailsPageViewModel()
            : this(Deployment.Current.Dispatcher, App.CloudClientFactory.ResolveNorthwindContext())
        {
        }

        public OrderDishDetailsPageViewModel(Dispatcher dispatcher, NorthwindContext northwindContext)
            : base(dispatcher, northwindContext)
        {
            this.odItems = new DataServiceCollection<OrderDetails>(northwindContext);
            this.odItems.LoadCompleted += this.OnOdItemsLoadCompleted;
        }

        public DataServiceCollection<OrderDetails> odItems { get; set; }
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
        public bool isOdListing = false;

        public virtual void LoadOdData()
        {
            if (!this.isOdListing)
            {
                ///Order_Details?$filter=OrderID eq 18 and DishID eq 31
                var requestUri = new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}/Order_Details?$filter=OrderID eq {1} and DishID eq {2}",
                        this.Context.BaseUri,
                        this.ReservationID,
                        this.DishID
                        ),
                    UriKind.Absolute);

                this.isOdListing = true;
                //this.Message = "Loading...";

                try
                {
                    //this.currentPage = 0;
                    this.odItems.Clear();
                    this.odItems.LoadAsync(requestUri);
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                    this.isOdListing = false;
                    //this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
        }

        public int qPrevious = -1;

        private void OnOdItemsLoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.isOdListing = false;
            }
            else if (e.Error != null)
            {
                this.isOdListing = false;
            }
            else
            {
                if (this.odItems.Count == 1)
                {
                    //we have an order detail, modify this bitch
                    //this.NavigationService.Navigate(new Uri("/Pages/ProfileDetailsPage.xaml?editProfile=true", UriKind.Relative));
                    qPrevious = this.odItems[0].Quantity;
                }
                else if (this.odItems.Count == 0)
                {
                    //we need to add order detail
                    OrderDetails temp = new OrderDetails();
                    temp.DishID = this.DishID;
                    temp.OrderID = this.ReservationID;
                    temp.Discount = 0;
                    temp.DishName = this.Dish.DishName;
                    temp.UnitPrice = (decimal)this.Dish.UnitPrice;
                    temp.Quantity = 0;
                    this.odItems.Add(temp);
                    qPrevious = this.odItems[0].Quantity;
                    //this.NavigationService.Navigate(new Uri("/Pages/ProfileDetailsPage.xaml?editProfile=false", UriKind.Relative));
                }
                else
                {
                    //something else went wrong
                    throw new NotImplementedException("more than one account with this info found");
                }
                this.isOdListing = false;
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

        public void saveOrderDetails()
        {
            //this.Context.AddToOrderDetails(odItems[0]);
            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == odItems[0]))
                {
                    this.Context.AttachToOrderDetails(odItems[0]);
                }

                this.Context.BeginSaveChanges(this.OnBeginSaveChanges, null);
            }
            catch (Exception exception)
            {
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
            }
        }


        public void DeleteOrderDetails()
        {
            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == odItems[0]))
                {
                    this.Context.AttachToOrderDetails(odItems[0]);
                }

                this.Context.DeleteObject(odItems[0]);
                this.Context.BeginSaveChanges(this.OnBeginSaveChanges, null);
            }
            catch (Exception exception)
            {
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
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

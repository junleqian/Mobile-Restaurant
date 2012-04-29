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
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Threading;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient;

    public class RestaurantDetailsPageViewModel : BaseViewModel
    {
        private readonly Dispatcher dispatcher;
        
        private Restaurant restaurant;
        private string message;
        private bool isSaving = false;

        public RestaurantDetailsPageViewModel()
            : this(Deployment.Current.Dispatcher, App.CloudClientFactory.ResolveNorthwindContext())
        {
        }

        public RestaurantDetailsPageViewModel(Dispatcher dispatcher, NorthwindContext northwindContext)
        {
            this.dispatcher = dispatcher;
            this.Context = northwindContext;
        }

        public event EventHandler SaveChangesSuccess;

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

        public bool IsSaving
        {
            get
            {
                return this.isSaving;
            }

            set
            {
                if (this.isSaving != value)
                {
                    this.isSaving = value;
                    this.NotifyPropertyChanged("IsSaving");
                }
            }
        }

        protected NorthwindContext Context { get; private set; }

        public void SetRestaurantModel(Restaurant restaurant = null)
        {
            if (restaurant == null)
            {
                this.Restaurant = new Restaurant();
                try
                {
                    this.Context.AddToRestaurants(this.Restaurant);
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
            else
            {
                this.Restaurant = restaurant;
                this.Restaurant.PropertyChanged += this.OnUpdateRestaurant;
            }
        }

        public void DetachRestaurantModel()
        {
            this.Context.Detach(this.Restaurant);
        }

        public void DeleteRestaurant()
        {
            this.Message = "Deleting...";
            this.IsSaving = true;

            this.Restaurant.PropertyChanged -= this.OnUpdateRestaurant;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Restaurant))
                {
                    this.Context.AttachToRestaurants(this.Restaurant);
                }

                this.Context.DeleteObject(this.Restaurant);
                this.Context.BeginSaveChanges(this.OnBeginSaveChanges, null);
            }
            catch (Exception exception)
            {
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                this.IsSaving = false;
                this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
            }
        }

        public void SaveRestaurant()
        {
            this.Message = "Saving...";
            this.IsSaving = true;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Restaurant))
                {
                    this.Context.AttachToRestaurants(this.Restaurant);
                }

                this.Context.BeginSaveChanges(this.OnBeginSaveChanges, null);
            }
            catch (Exception exception)
            {
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                this.IsSaving = false;
                this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
            }
        }

        private void OnUpdateRestaurant(object sender, PropertyChangedEventArgs e)
        {
            this.Restaurant.PropertyChanged -= this.OnUpdateRestaurant;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Restaurant))
                {
                    this.Context.AttachToRestaurants(this.Restaurant);
                }

                this.Context.UpdateObject(this.Restaurant);
            }
            catch (Exception exception)
            {
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
            }
        }

        private void OnBeginSaveChanges(IAsyncResult asyncResult)
        {
            this.dispatcher.BeginInvoke(
                () =>
                {
                    try
                    {
                        this.Context.EndSaveChanges(asyncResult);

                        this.Message = "Changes saved successfully.";
                        this.IsSaving = false;

                        this.RaiseSaveChangesSuccess();
                    }
                    catch (Exception exception)
                    {
                        this.Message = string.Format(
                            CultureInfo.InvariantCulture,
                            "Error: {0}", 
                            StorageClientExceptionParser.ParseDataServiceException(exception).Message);

                        this.IsSaving = false;

                        this.DetachRestaurantModel();
                        if (this.Restaurant.RestaurantID > 0)
                        {
                            this.Restaurant.PropertyChanged += this.OnUpdateRestaurant;
                        }
                        else
                        {
                            this.Context.AddToRestaurants(this.Restaurant);
                        }
                    }
                });
        }

        private void RaiseSaveChangesSuccess()
        {
            var saveChangesSuccess = this.SaveChangesSuccess;
            if (saveChangesSuccess != null)
            {
                saveChangesSuccess(this, EventArgs.Empty);
            }
        }
    }
}

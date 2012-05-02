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
    using System.Data.Services.Client;
    using System.Globalization;
    using System.Windows.Threading;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient;

    public abstract class PartialListViewModel<T> : BaseViewModel
    {
        private readonly Dispatcher dispatcher;

        private bool isListing = false;
        private bool hasError = false;
        private int currentPage = 0;
        private string message = string.Empty;

        public PartialListViewModel(Dispatcher dispatcher, NorthwindContext northwindContext)
        {
            this.dispatcher = dispatcher;
            this.Context = northwindContext;
            this.Items = new DataServiceCollection<T>(northwindContext);
            this.Items.LoadCompleted += this.OnItemsLoadCompleted;
            //this.PartialItems = new DataServiceCollection<T>();
        }

        public DataServiceCollection<T> Items { get; set; }
        //public DataServiceCollection<T> PartialItems { get; set; }

        public int PageNumber { get; set; }
        public int RestID { get; set; }
        public int ReservationID { get; set; }
        public int DishID { get; set; }

        public bool IsListing
        {
            get
            {
                return this.isListing;
            }
            
            set
            {
                this.isListing = value;
                this.NotifyPropertyChanged("IsListing");
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

        public bool HasContinuation
        {
            get
            {
                return this.Items.Continuation != null;
            }
        }

        public bool LoadMoreResultsVisible
        {
            get
            {
                return this.HasContinuation && !this.IsListing && !this.hasError;
            }
        }

        protected NorthwindContext Context { get; private set; }

        protected abstract string EntitySetName { get; }

        public virtual void LoadData()
        {
            if (!this.IsListing)
            {
                // Adding an incremental seed to avoid a cached response.
                /*var requestUri = new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}/{1}?incrementalSeed={2}",
                        this.Context.BaseUri,
                        this.EntitySetName,
                        DateTime.UtcNow.Ticks),
                    UriKind.Absolute);
                */
                //new partial odata query
                Uri requestUri;
                if (this.DishID == -1) //querying a restaurant's dishes
                {
                    //to include category info do this, or find cats then do separate q for each
                    //{0}/Restaurants({1})/{2}?$expand=Categories&$orderby=CategoryID

                    requestUri = new Uri(
                                      string.Format(
                                          CultureInfo.InvariantCulture,
                                          //"{0}/Restaurants({1})/{2}?incrementalSeed={3}",
                                          "{0}/Restaurants({1})/{2}?$orderby=CategoryID",
                                          this.Context.BaseUri,
                                          this.RestID.ToString(),
                                          this.EntitySetName),
                                          //DateTime.UtcNow.Ticks),
                                      UriKind.Absolute);
                }
                else if (this.DishID == -2) //querying a restaurant's dishes
                {
                    //to include category info do this, or find cats then do separate q for each
                    //{0}/Restaurants({1})/{2}?$expand=Categories&$orderby=CategoryID

                    requestUri = new Uri(
                                      string.Format(
                                          CultureInfo.InvariantCulture,
                        //"{0}/Restaurants({1})/{2}?incrementalSeed={3}",
                                          "{0}/Restaurants({1})/{2}",
                                          this.Context.BaseUri,
                                          this.RestID.ToString(),
                                          this.EntitySetName),
                        //DateTime.UtcNow.Ticks),
                                      UriKind.Absolute);
                }
                else if (this.DishID == -3) //querying an order
                {
                    //to include category info do this, or find cats then do separate q for each
                    //{0}/Restaurants({1})/{2}?$expand=Categories&$orderby=CategoryID

                    requestUri = new Uri(
                                      string.Format(
                                          CultureInfo.InvariantCulture,
                        //"{0}/Restaurants({1})/{2}?incrementalSeed={3}",
                                          "{0}/Order_Details/?$filter=OrderID eq {1}",
                                          this.Context.BaseUri,
                                          this.ReservationID.ToString(),
                                          this.EntitySetName),
                        //DateTime.UtcNow.Ticks),
                                      UriKind.Absolute);
                }
                else //querying a dish
                {
                    requestUri = new Uri(
                                      string.Format(
                                          CultureInfo.InvariantCulture,
                                          "{0}/Restaurants({1})/Dishes({2})/{3}?incrementalSeed={4}",
                                          this.Context.BaseUri,
                                          this.RestID.ToString(),
                                          this.DishID.ToString(),
                                          this.EntitySetName,
                                          DateTime.UtcNow.Ticks),
                                      UriKind.Absolute);
                }
                this.IsListing = true;
                this.Message = "Loading...";

                try
                {
                    this.currentPage = 0;
                    this.Items.Clear();
                    this.Items.LoadAsync(requestUri);
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                    this.IsListing = false;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
        }

        public void LoadNextPage()
        {
            if (this.HasContinuation && !this.IsListing)
            {
                this.IsListing = true;
                try
                {
                    this.Items.LoadNextPartialSetAsync();
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                    this.IsListing = false;
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
                this.hasError = true;
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(e.Error).Message;
                this.DispatchResult(string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage));
            }
            else
            {
                this.hasError = false;
                this.currentPage++;
                if (this.PageNumber < this.currentPage)
                {
                    this.PageNumber = this.currentPage;
                }

                if (this.HasContinuation && this.PageNumber > this.currentPage)
                {
                    try
                    {
                        this.Items.LoadNextPartialSetAsync();
                    }
                    catch (Exception exception)
                    {
                        var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                        this.DispatchResult(string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage));
                    }
                }
                else
                {
                    //TODO: CHANGE THIS!!!
                    /*
                    int prevCat = -1;
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        if (( new Dish(this.Items[i])).CategoryID != RestID)
                        {
                            //PartialItems.Add(this.Items[i]);
                            this.Items.RemoveAt(i);
                            i--;
                        }
                    }
                    */

                    //this.Items = PartialItems;
                    this.DispatchResult();
                }
            }
        }

        private void DispatchResult(string message = "")
        {
            this.dispatcher.BeginInvoke(
                () =>
                {
                    this.IsListing = false;
                    this.Message = message;
                    this.NotifyPropertyChanged("LoadMoreResultsVisible");
                });
        }

        protected void OnBeginSaveChanges(IAsyncResult asyncResult)
        {
            this.dispatcher.BeginInvoke(
                () =>
                {
                    try
                    {
                        this.Context.EndSaveChanges(asyncResult);

                        this.Message = "Changes saved successfully.";

                        this.RaiseSaveChangesSuccess();
                    }
                    catch (Exception exception)
                    {
                        this.Message = string.Format(
                            CultureInfo.InvariantCulture,
                            "Error: {0}",
                            StorageClientExceptionParser.ParseDataServiceException(exception).Message);
                    }
                });
        }

        public event EventHandler SaveChangesSuccess;

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
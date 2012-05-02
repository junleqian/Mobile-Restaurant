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
    using System.Linq;
    using System.Windows.Threading;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient;

    public abstract class ReservationsViewModel : BaseViewModel
    {
        private readonly Dispatcher dispatcher;

        private bool isrListing = false;
        private bool istListing = false;
        private bool isListing = false;
        private bool isSaving = false;
        private bool hasError = false;
        private int rcurrentPage = 0;
        private int tcurrentPage = 0;
        private string message = string.Empty;

        public ReservationsViewModel(Dispatcher dispatcher, NorthwindContext northwindContext)
        {
            this.dispatcher = dispatcher;
            this.Context = northwindContext;
            this.rItems = new DataServiceCollection<Reservation>(northwindContext);
            this.tItems = new DataServiceCollection<Table>(northwindContext);
            this.rItems.LoadCompleted += this.OnrItemsLoadCompleted;
            this.tItems.LoadCompleted += this.OntItemsLoadCompleted;
        }

        public DataServiceCollection<Reservation> rItems { get; set; }
        public DataServiceCollection<Table> tItems { get; set; }

        public int tPageNumber { get; set; }
        public int rPageNumber { get; set; }
        public int RestID { get; set; }
        public string RestName { get; set; }
        public Reservation Reservation;

        public event EventHandler SaveChangesSuccess;

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

        public bool IsrListing
        {
            get
            {
                return this.isrListing;
            }
            
            set
            {
                this.isrListing = value;
                this.NotifyPropertyChanged("IsrListing");
            }
        }

        public bool IstListing
        {
            get
            {
                return this.istListing;
            }

            set
            {
                this.istListing = value;
                this.NotifyPropertyChanged("IstListing");
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

        public bool rHasContinuation
        {
            get
            {
                return this.rItems.Continuation != null;
            }
        }

        public bool rLoadMoreResultsVisible
        {
            get
            {
                return this.rHasContinuation && !this.IsrListing && !this.hasError;
            }
        }

        public bool tHasContinuation
        {
            get
            {
                return this.tItems.Continuation != null;
            }
        }

        public bool tLoadMoreResultsVisible
        {
            get
            {
                return this.tHasContinuation && !this.IstListing && !this.hasError;
            }
        }

        protected NorthwindContext Context { get; private set; }

        public void SetReservationModel()
        {
            try
            {
                this.Context.AddToReservations(Reservation);
            }
            catch (Exception exception)
            {
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
            }
        }

        public void SaveReservation()
        {
            this.Message = "Booking...";
            this.IsSaving = true;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Reservation))
                {
                    this.Context.AttachToReservations(this.Reservation);
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

                        //TODO: fix this?
                        //this.DetachRestaurantModel();
                        //if (this.Restaurant.RestaurantID > 0)
                        //{
                        //    this.Restaurant.PropertyChanged += this.OnUpdateRestaurant;
                        //}
                        //else
                        //{
                        //    this.Context.AddToRestaurants(this.Restaurant);
                        //}
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

        public virtual void LoadData()
        {
            if (!this.IsrListing)
            {
                //new partial odata query
                Uri requestrUri;

                    //to include category info do this, or find cats then do separate q for each
                    //{0}/Restaurants({1})/{2}?$expand=Categories&$orderby=CategoryID
                //CultureInfo ci = new CultureInfo("

                //Reservations?$filter=EndTime ge datetime'2012-05-03T14:35:00' and StartTime le datetime'2012-05-03T14:55:00'
                    requestrUri = new Uri(
                                      string.Format(
                                          CultureInfo.InvariantCulture,
                                          //"{0}/Restaurants({1})/{2}?incrementalSeed={3}",
                                          "{0}/Restaurants({1})/Reservations?$filter=EndTime ge datetime'{2}' and StartTime le datetime'{3}'&$orderby=TableID",
                                          this.Context.BaseUri,
                                          this.RestID.ToString(),
                                          this.Reservation.StartTime.ToString("o", CultureInfo.InvariantCulture), //TODOOOOOOOOOOOOOOOOOOO: fix this format!!!!!!!!!!!!!!!!!!!!!
                                          this.Reservation.EndTime.ToString("o", CultureInfo.InvariantCulture)
                                          ),
                                          //DateTime.UtcNow.Ticks),
                                      UriKind.Absolute);

                this.IsrListing = true;
                this.IsListing = true;
                this.Message = "Loading...";

                try
                {
                    this.rcurrentPage = 0;
                    this.rItems.Clear();
                    this.rItems.LoadAsync(requestrUri);
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                    this.IsrListing = false;
                    this.IsListing = false;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
            if (!this.IstListing)
            {
                //new partial odata query
                Uri requesttUri;

                requesttUri = new Uri(
                                  string.Format(
                                      CultureInfo.InvariantCulture,
                    //"{0}/Restaurants({1})/{2}?incrementalSeed={3}",
                                      "{0}/Restaurants({1})/Tables?$filter=NumSeats ge {2}&$orderby=TableID",
                                      this.Context.BaseUri,
                                      this.RestID.ToString(),
                                      this.Reservation.NumGuests.ToString()
                                      ),
                    //DateTime.UtcNow.Ticks),
                                  UriKind.Absolute);

                this.IstListing = true;
                this.IsListing = true;
                this.Message = "Loading...";

                try
                {
                    this.tcurrentPage = 0;
                    this.tItems.Clear();
                    this.tItems.LoadAsync(requesttUri);
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                    this.IstListing = false;
                    this.IsListing = false;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
        }

        public void rLoadNextPage()
        {
            if (this.rHasContinuation && !this.IsrListing)
            {
                this.IsrListing = true;
                this.IsListing = true;
                try
                {
                    this.rItems.LoadNextPartialSetAsync();
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                    this.IsrListing = false;
                    this.IsListing = false;
                }
            }
        }
        public void tLoadNextPage()
        {
            if (this.tHasContinuation && !this.IstListing)
            {
                this.IstListing = true;
                try
                {
                    this.tItems.LoadNextPartialSetAsync();
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                    this.IstListing = false;
                }
            }
        }

        private void OntItemsLoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.tDispatchResult("The operation has been cancelled.");
            }
            else if (e.Error != null)
            {
                this.hasError = true;
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(e.Error).Message;
                this.tDispatchResult(string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage));
            }
            else
            {
                this.hasError = false;
                this.tcurrentPage++;
                if (this.tPageNumber < this.tcurrentPage)
                {
                    this.tPageNumber = this.tcurrentPage;
                }

                if (this.tHasContinuation && this.tPageNumber > this.tcurrentPage)
                {
                    try
                    {
                        this.tItems.LoadNextPartialSetAsync();
                    }
                    catch (Exception exception)
                    {
                        var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                        this.tDispatchResult(string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage));
                    }
                }
                else
                {
                    this.tDispatchResult();
                }
            }
        }

        private void OnrItemsLoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                this.rDispatchResult("The operation has been cancelled.");
            }
            else if (e.Error != null)
            {
                this.hasError = true;
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(e.Error).Message;
                this.rDispatchResult(string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage));
            }
            else
            {
                this.hasError = false;
                this.rcurrentPage++;
                if (this.rPageNumber < this.rcurrentPage)
                {
                    this.rPageNumber = this.rcurrentPage;
                }

                if (this.rHasContinuation && this.rPageNumber > this.rcurrentPage)
                {
                    try
                    {
                        this.rItems.LoadNextPartialSetAsync();
                    }
                    catch (Exception exception)
                    {
                        var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                        this.rDispatchResult(string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage));
                    }
                }
                else
                {
                    this.rDispatchResult();
                }
            }
        }
        private void tDispatchResult(string message = "")
        {
            this.dispatcher.BeginInvoke(
                () =>
                {
                    this.IstListing = false;
                    this.Message = message;
                    this.NotifyPropertyChanged("LoadMoreResultsVisible");
                });
        }
        private void rDispatchResult(string message = "")
        {
            this.dispatcher.BeginInvoke(
                () =>
                {
                    this.IsrListing = false;
                    this.Message = message;
                    this.NotifyPropertyChanged("LoadMoreResultsVisible");
                });
        }
    }
}
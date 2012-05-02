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
    using Microsoft.Samples.CRUDSqlAzure.Phone.Helpers;
    using Microsoft.Samples.WindowsPhoneCloud.StorageClient;

    public class CustomerDetailsPageViewModel : BaseViewModel
    {
        private readonly Dispatcher dispatcher;
        
        private Customer customer;
        private string message;
        private bool isSaving = false;

        public CustomerDetailsPageViewModel()
            : this(Deployment.Current.Dispatcher, App.CloudClientFactory.ResolveNorthwindContext())
        {
        }

        public CustomerDetailsPageViewModel(Dispatcher dispatcher, NorthwindContext northwindContext)
        {
            this.dispatcher = dispatcher;
            this.Context = northwindContext;
        }

        public event EventHandler SaveChangesSuccess;

        public Customer Customer
        {
            get
            {
                return this.customer;
            }

            set
            {
                if (this.customer != value)
                {
                    this.customer = value;
                    this.NotifyPropertyChanged("Customer");
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

        public void SetCustomerModel(Customer customer = null)
        {
            if (customer == null)
            {
                this.Customer = new Customer();
                this.Customer.CustomerID = PhoneHelpers.GetIsolatedStorageSetting<int>("reg_id");
                this.Customer.Name = PhoneHelpers.GetIsolatedStorageSetting<string>("reg_username");
                this.Customer.Email = PhoneHelpers.GetIsolatedStorageSetting<string>("reg_email");
                try
                {
                    this.Context.AddToCustomers(this.Customer);
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
            else
            {
                this.Customer = customer;
                this.Customer.PropertyChanged += this.OnUpdateCustomer;
            }
        }

        public void DetachCustomerModel()
        {
            this.Context.Detach(this.Customer);
        }

        public void DeleteCustomer()
        {
            this.Message = "Deleting...";
            this.IsSaving = true;

            this.Customer.PropertyChanged -= this.OnUpdateCustomer;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Customer))
                {
                    this.Context.AttachToCustomers(this.Customer);
                }

                this.Context.DeleteObject(this.Customer);
                this.Context.BeginSaveChanges(this.OnBeginSaveChanges, null);
            }
            catch (Exception exception)
            {
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                this.IsSaving = false;
                this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
            }
        }

        public void SaveCustomer()
        {
            this.Message = "Saving...";
            this.IsSaving = true;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Customer))
                {
                    this.Context.AttachToCustomers(this.Customer);
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

        private void OnUpdateCustomer(object sender, PropertyChangedEventArgs e)
        {
            this.Customer.PropertyChanged -= this.OnUpdateCustomer;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Customer))
                {
                    this.Context.AttachToCustomers(this.Customer);
                }

                this.Context.UpdateObject(this.Customer);
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

                        this.DetachCustomerModel();
                        if (this.Customer.CustomerID > 0)
                        {
                            this.Customer.PropertyChanged += this.OnUpdateCustomer;
                        }
                        else
                        {
                            this.Context.AddToCustomers(this.Customer);
                        }
                    }
                });
        }

        private void RaiseSaveChangesSuccess()
        {
            var saveChangesSuccess = this.SaveChangesSuccess;
            if (saveChangesSuccess != null)
            {
                PhoneHelpers.SetIsolatedStorageSetting("reg_id", this.Customer.CustomerID);
                PhoneHelpers.SetIsolatedStorageSetting("reg_username", this.Customer.Name);
                PhoneHelpers.SetIsolatedStorageSetting("reg_email", this.Customer.Email);
                saveChangesSuccess(this, EventArgs.Empty);
            }
        }
    }
}

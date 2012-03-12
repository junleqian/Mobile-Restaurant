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

    public class SupplierDetailsPageViewModel : BaseViewModel
    {
        private readonly Dispatcher dispatcher;
        
        private Supplier supplier;
        private string message;
        private bool isSaving = false;

        public SupplierDetailsPageViewModel()
            : this(Deployment.Current.Dispatcher, App.CloudClientFactory.ResolveNorthwindContext())
        {
        }

        public SupplierDetailsPageViewModel(Dispatcher dispatcher, NorthwindContext northwindContext)
        {
            this.dispatcher = dispatcher;
            this.Context = northwindContext;
        }

        public event EventHandler SaveChangesSuccess;

        public Supplier Supplier
        {
            get
            {
                return this.supplier;
            }

            set
            {
                if (this.supplier != value)
                {
                    this.supplier = value;
                    this.NotifyPropertyChanged("Supplier");
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

        public void SetSupplierModel(Supplier supplier = null)
        {
            if (supplier == null)
            {
                this.Supplier = new Supplier();
                try
                {
                    this.Context.AddToSuppliers(this.Supplier);
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
            else
            {
                this.Supplier = supplier;
                this.Supplier.PropertyChanged += this.OnUpdateSupplier;
            }
        }

        public void DetachSupplierModel()
        {
            this.Context.Detach(this.Supplier);
        }

        public void DeleteSupplier()
        {
            this.Message = "Deleting...";
            this.IsSaving = true;

            this.Supplier.PropertyChanged -= this.OnUpdateSupplier;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Supplier))
                {
                    this.Context.AttachToSuppliers(this.Supplier);
                }

                this.Context.DeleteObject(this.Supplier);
                this.Context.BeginSaveChanges(this.OnBeginSaveChanges, null);
            }
            catch (Exception exception)
            {
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                this.IsSaving = false;
                this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
            }
        }

        public void SaveSupplier()
        {
            this.Message = "Saving...";
            this.IsSaving = true;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Supplier))
                {
                    this.Context.AttachToSuppliers(this.Supplier);
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

        private void OnUpdateSupplier(object sender, PropertyChangedEventArgs e)
        {
            this.Supplier.PropertyChanged -= this.OnUpdateSupplier;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Supplier))
                {
                    this.Context.AttachToSuppliers(this.Supplier);
                }

                this.Context.UpdateObject(this.Supplier);
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

                        this.DetachSupplierModel();
                        if (this.Supplier.SupplierID > 0)
                        {
                            this.Supplier.PropertyChanged += this.OnUpdateSupplier;
                        }
                        else
                        {
                            this.Context.AddToSuppliers(this.Supplier);
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

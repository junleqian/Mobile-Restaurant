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

    public class ProductDetailsPageViewModel : BaseViewModel
    {
        private readonly Dispatcher dispatcher;
        
        private Product product;
        private string message;
        private bool isSaving = false;

        public ProductDetailsPageViewModel()
            : this(Deployment.Current.Dispatcher, App.CloudClientFactory.ResolveNorthwindContext())
        {
        }

        public ProductDetailsPageViewModel(Dispatcher dispatcher, NorthwindContext northwindContext)
        {
            this.dispatcher = dispatcher;
            this.Context = northwindContext;
        }

        public event EventHandler SaveChangesSuccess;

        public Product Product
        {
            get
            {
                return this.product;
            }

            set
            {
                if (this.product != value)
                {
                    this.product = value;
                    this.NotifyPropertyChanged("Product");
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

        public void SetProductModel(Product product = null)
        {
            if (product == null)
            {
                this.Product = new Product();
                try
                {
                    this.Context.AddToProducts(this.Product);
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
            else
            {
                this.Product = product;
                this.Product.PropertyChanged += this.OnUpdateProduct;
            }
        }

        public void DetachProductModel()
        {
            this.Context.Detach(this.Product);
        }

        public void DeleteProduct()
        {
            this.Message = "Deleting...";
            this.IsSaving = true;

            this.Product.PropertyChanged -= this.OnUpdateProduct;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Product))
                {
                    this.Context.AttachToProducts(this.Product);
                }

                this.Context.DeleteObject(this.Product);
                this.Context.BeginSaveChanges(this.OnBeginSaveChanges, null);
            }
            catch (Exception exception)
            {
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                this.IsSaving = false;
                this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
            }
        }

        public void SaveProduct()
        {
            this.Message = "Saving...";
            this.IsSaving = true;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Product))
                {
                    this.Context.AttachToProducts(this.Product);
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

        private void OnUpdateProduct(object sender, PropertyChangedEventArgs e)
        {
            this.Product.PropertyChanged -= this.OnUpdateProduct;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Product))
                {
                    this.Context.AttachToProducts(this.Product);
                }

                this.Context.UpdateObject(this.Product);
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

                        this.DetachProductModel();
                        if (this.Product.ProductID > 0)
                        {
                            this.Product.PropertyChanged += this.OnUpdateProduct;
                        }
                        else
                        {
                            this.Context.AddToProducts(this.Product);
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

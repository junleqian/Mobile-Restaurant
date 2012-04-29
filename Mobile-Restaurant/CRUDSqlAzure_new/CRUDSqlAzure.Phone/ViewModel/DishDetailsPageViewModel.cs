// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, dishs, domain names,
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

    public class DishDetailsPageViewModel : BaseViewModel
    {
        private readonly Dispatcher dispatcher;
        
        private Dish dish;
        private string message;
        private bool isSaving = false;

        public DishDetailsPageViewModel()
            : this(Deployment.Current.Dispatcher, App.CloudClientFactory.ResolveNorthwindContext())
        {
        }

        public DishDetailsPageViewModel(Dispatcher dispatcher, NorthwindContext northwindContext)
        {
            this.dispatcher = dispatcher;
            this.Context = northwindContext;
        }

        public event EventHandler SaveChangesSuccess;

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

        public void SetDishModel(Dish dish = null)
        {
            if (dish == null)
            {
                this.Dish = new Dish();
                try
                {
                    this.Context.AddToDishes(this.Dish);
                }
                catch (Exception exception)
                {
                    var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;
                    this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
                }
            }
            else
            {
                this.Dish = dish;
                this.Dish.PropertyChanged += this.OnUpdateDish;
            }
        }

        public void DetachDishModel()
        {
            this.Context.Detach(this.Dish);
        }

        public void DeleteDish()
        {
            this.Message = "Deleting...";
            this.IsSaving = true;

            this.Dish.PropertyChanged -= this.OnUpdateDish;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Dish))
                {
                    this.Context.AttachToDishes(this.Dish);
                }

                this.Context.DeleteObject(this.Dish);
                this.Context.BeginSaveChanges(this.OnBeginSaveChanges, null);
            }
            catch (Exception exception)
            {
                var errorMessage = StorageClientExceptionParser.ParseDataServiceException(exception).Message;

                this.IsSaving = false;
                this.Message = string.Format(CultureInfo.InvariantCulture, "Error: {0}", errorMessage);
            }
        }

        public void SaveDish()
        {
            this.Message = "Saving...";
            this.IsSaving = true;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Dish))
                {
                    this.Context.AttachToDishes(this.Dish);
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

        private void OnUpdateDish(object sender, PropertyChangedEventArgs e)
        {
            this.Dish.PropertyChanged -= this.OnUpdateDish;

            try
            {
                if (!this.Context.Entities.Any(ed => ed.Entity == this.Dish))
                {
                    this.Context.AttachToDishes(this.Dish);
                }

                this.Context.UpdateObject(this.Dish);
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

                        this.DetachDishModel();
                        if (this.Dish.DishID > 0)
                        {
                            this.Dish.PropertyChanged += this.OnUpdateDish;
                        }
                        else
                        {
                            this.Context.AddToDishes(this.Dish);
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

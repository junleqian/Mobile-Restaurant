// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, dishes, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace Microsoft.Samples.CRUDSqlAzure.Phone.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Services.Common;

    [EntitySetAttribute("Orders")]
    [DataServiceKeyAttribute("OrderID")]
    public class Order : INotifyPropertyChanged
    {
        private int? reservationId;
        private int? restaurantId;
        private int orderId;
        private int? customerId;
        private DateTime orderDate;

        public event PropertyChangedEventHandler PropertyChanged;

        public int? ReservationID
        {
            get
            {
                return this.reservationId;
            }

            set
            {
                this.reservationId = value;
                this.OnPropertyChanged("ReservationID");
            }
        }

        public int? RestaurantID
        {
            get
            {
                return this.restaurantId;
            }

            set
            {
                this.restaurantId = value;
                this.OnPropertyChanged("RestaurantID");
            }
        }

        public DateTime OrderDate
        {
            get
            {
                return this.orderDate;
            }

            set
            {
                this.orderDate = value;
                this.OnPropertyChanged("OrderDate");
            }
        }

        public int? CustomerID
        {
            get
            {
                return this.customerId;
            }

            set
            {
                this.customerId = value;
                this.OnPropertyChanged("CustomerID");
            }
        }

        public int OrderID
        {
            get
            {
                return this.orderId;
            }

            set
            {
                this.orderId = value;
                this.OnPropertyChanged("OrderID");
            }
        }

        protected virtual void OnPropertyChanged(string changedProperty)
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(changedProperty));
            }
        }
    }
}

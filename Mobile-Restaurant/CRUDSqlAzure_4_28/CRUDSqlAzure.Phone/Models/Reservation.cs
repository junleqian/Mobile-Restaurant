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

    [EntitySetAttribute("Reservations")]
    [DataServiceKeyAttribute("ReservationID")]
    public class Reservation : INotifyPropertyChanged
    {
        private int reservationId;
        private int restaurantId;
        private int tableId;
        private int? numGuests;
        private int? customerId;
        private DateTime startTime;
        private DateTime endTime;
        private int? orderId;

        public event PropertyChangedEventHandler PropertyChanged;

        public int ReservationID
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

        public int RestaurantID
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

        public int TableID
        {
            get
            {
                return this.tableId;
            }

            set
            {
                this.tableId = value;
                this.OnPropertyChanged("TableID");
            }
        }

        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }

            set
            {
                this.startTime = value;
                this.OnPropertyChanged("StartTime");
            }
        }

        public DateTime EndTime
        {
            get
            {
                return this.endTime;
            }

            set
            {
                this.endTime = value;
                this.OnPropertyChanged("EndTime");
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

        public int? NumGuests
        {
            get
            {
                return this.numGuests;
            }

            set
            {
                this.numGuests = value;
                this.OnPropertyChanged("NumGuests");
            }
        }

        public int? OrderID
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

        public static Reservation CreateReservation(int reservationID, int customerID, int tableID)
        {
            return new Reservation
            {
                ReservationID = reservationID,
                TableID = tableID,
                CustomerID = customerID,
            };
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

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

    [EntitySetAttribute("Table")]
    [DataServiceKeyAttribute("TableID")]
    public class Table : INotifyPropertyChanged
    {
        private int restaurantId;
        private int tableId;
        private int numSeats;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public int NumSeats
        {
            get
            {
                return this.numSeats;
            }

            set
            {
                this.numSeats = value;
                this.OnPropertyChanged("NumSeats");
            }
        }

        public static Table CreateTablew(int numSeats, int restaurantID, int tableID)
        {
            return new Table
            {
                RestaurantID = restaurantID,
                TableID = tableID,
                NumSeats = numSeats,
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

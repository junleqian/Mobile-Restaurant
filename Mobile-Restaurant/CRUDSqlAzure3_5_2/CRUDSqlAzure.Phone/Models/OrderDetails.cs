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

    [EntitySetAttribute("Order_Details")]
    [DataServiceKeyAttribute("OrderID")]
    public class OrderDetails : INotifyPropertyChanged
    {
        private int orderId;
        private int dishId;
        private decimal unitPrice;
        private short quantity;
        private Single discount;
        private string dishName;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public int DishID
        {
            get
            {
                return this.dishId;
            }

            set
            {
                this.dishId = value;
                this.OnPropertyChanged("DishID");
            }
        }

        public decimal UnitPrice
        {
            get
            {
                return this.unitPrice;
            }

            set
            {
                this.unitPrice = value;
                this.OnPropertyChanged("UnitPrice");
            }
        }

        public short Quantity
        {
            get
            {
                return this.quantity;
            }

            set
            {
                this.quantity = value;
                this.OnPropertyChanged("Quantity");
            }
        }

        public Single Discount
        {
            get
            {
                return this.discount;
            }

            set
            {
                this.discount = value;
                this.OnPropertyChanged("Discount");
            }
        }

        public string DishName
        {
            get
            {
                return this.dishName;
            }

            set
            {
                this.dishName = value;
                this.OnPropertyChanged("DishName");
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

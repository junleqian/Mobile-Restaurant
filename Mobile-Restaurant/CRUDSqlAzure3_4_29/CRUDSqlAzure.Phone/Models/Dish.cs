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

    [EntitySetAttribute("Dishes")]
    [DataServiceKeyAttribute("DishID")]
    public class Dish : INotifyPropertyChanged
    {
        private int id;
        private string dishName;
        private string categoryName;
        private int? restaurantID;
        private int? categoryID;
        private string description;
        private decimal? unitPrice;
        private short? unitsInStock;
        private short? unitsOnOrder;
        private short? reorderLevel;
        private bool discontinued;
        private byte[] image;

        public event PropertyChangedEventHandler PropertyChanged;

        public int DishID
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
                this.OnPropertyChanged("DishID");
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

        public string CategoryName
        {
            get
            {
                return this.categoryName;
            }

            set
            {
                this.categoryName = value;
                this.OnPropertyChanged("CategoryName");
            }
        }

        public int? RestaurantID
        {
            get
            {
                return this.restaurantID;
            }

            set
            {
                this.restaurantID = value;
                this.OnPropertyChanged("RestaurantID");
            }
        }

        public int? CategoryID
        {
            get
            {
                return this.categoryID;
            }

            set
            {
                this.categoryID = value;
                this.OnPropertyChanged("CategoryID");
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;
                this.OnPropertyChanged("Description");
            }
        }

        public byte[] Image
        {
            get
            {
                return this.image;
            }

            set
            {
                this.image = value;
                this.OnPropertyChanged("Image");
            }
        }

        public decimal? UnitPrice
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

        public short? UnitsInStock
        {
            get
            {
                return this.unitsInStock;
            }

            set
            {
                this.unitsInStock = value;
                this.OnPropertyChanged("UnitsInStock");
            }
        }

        public short? UnitsOnOrder
        {
            get
            {
                return this.unitsOnOrder;
            }

            set
            {
                this.unitsOnOrder = value;
                this.OnPropertyChanged("UnitsOnOrder");
            }
        }

        public short? ReorderLevel
        {
            get
            {
                return this.reorderLevel;
            }

            set
            {
                this.reorderLevel = value;
                this.OnPropertyChanged("ReorderLevel");
            }
        }

        public bool Discontinued
        {
            get
            {
                return this.discontinued;
            }

            set
            {
                this.discontinued = value;
                this.OnPropertyChanged("Discontinued");
            }
        }

        public static Dish CreateDish(int dishID, string dishName, bool discontinued)
        {
            return new Dish
            {
                DishID = dishID,
                DishName = dishName,
                Discontinued = discontinued,
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

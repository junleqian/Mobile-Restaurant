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

namespace Microsoft.Samples.CRUDSqlAzure.Phone.Models
{
    using System;
    using System.ComponentModel;
    using System.Data.Services.Common;

    [EntitySetAttribute("Products")]
    [DataServiceKeyAttribute("ProductID")]
    public class Product : INotifyPropertyChanged
    {
        private int id;
        private string productName;
        private int? supplierID;
        private int? categoryID;
        private string quantityPerUnit;
        private decimal? unitPrice;
        private short? unitsInStock;
        private short? unitsOnOrder;
        private short? reorderLevel;
        private bool discontinued;

        public event PropertyChangedEventHandler PropertyChanged;

        public int ProductID
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
                this.OnPropertyChanged("ProductID");
            }
        }

        public string ProductName
        {
            get
            {
                return this.productName;
            }

            set
            {
                this.productName = value;
                this.OnPropertyChanged("ProductName");
            }
        }

        public int? SupplierID
        {
            get
            {
                return this.supplierID;
            }

            set
            {
                this.supplierID = value;
                this.OnPropertyChanged("SupplierID");
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

        public string QuantityPerUnit
        {
            get
            {
                return this.quantityPerUnit;
            }

            set
            {
                this.quantityPerUnit = value;
                this.OnPropertyChanged("QuantityPerUnit");
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

        public static Product CreateProduct(int productID, string productName, bool discontinued)
        {
            return new Product
            {
                ProductID = productID,
                ProductName = productName,
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

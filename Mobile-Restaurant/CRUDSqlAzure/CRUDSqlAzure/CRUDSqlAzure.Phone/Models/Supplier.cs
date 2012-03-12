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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data.Services.Common;

    [EntitySetAttribute("Suppliers")]
    [DataServiceKeyAttribute("SupplierID")]
    public class Supplier : INotifyPropertyChanged
    {
        private int supplierID;
        private string companyName;
        private string contactName;
        private string contactTitle;
        private string address;
        private string city;
        private string region;
        private string postalCode;
        private string country;
        private string phone;
        private string fax;
        private string homePage;
        private Collection<Product> products = new Collection<Product>();

        public event PropertyChangedEventHandler PropertyChanged;

        public int SupplierID
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

        public string CompanyName
        {
            get
            {
                return this.companyName;
            }

            set
            {
                this.companyName = value;
                this.OnPropertyChanged("CompanyName");
            }
        }

        public string ContactName
        {
            get
            {
                return this.contactName;
            }

            set
            {
                this.contactName = value;
                this.OnPropertyChanged("ContactName");
            }
        }

        public string ContactTitle
        {
            get
            {
                return this.contactTitle;
            }

            set
            {
                this.contactTitle = value;
                this.OnPropertyChanged("ContactTitle");
            }
        }

        public string Address
        {
            get
            {
                return this.address;
            }

            set
            {
                this.address = value;
                this.OnPropertyChanged("Address");
            }
        }

        public string City
        {
            get
            {
                return this.city;
            }

            set
            {
                this.city = value;
                this.OnPropertyChanged("City");
            }
        }

        public string Region
        {
            get
            {
                return this.region;
            }

            set
            {
                this.region = value;
                this.OnPropertyChanged("Region");
            }
        }

        public string PostalCode
        {
            get
            {
                return this.postalCode;
            }

            set
            {
                this.postalCode = value;
                this.OnPropertyChanged("PostalCode");
            }
        }

        public string Country
        {
            get
            {
                return this.country;
            }

            set
            {
                this.country = value;
                this.OnPropertyChanged("Country");
            }
        }

        public string Phone
        {
            get
            {
                return this.phone;
            }

            set
            {
                this.phone = value;
                this.OnPropertyChanged("Phone");
            }
        }

        public string Fax
        {
            get
            {
                return this.fax;
            }

            set
            {
                this.fax = value;
                this.OnPropertyChanged("Fax");
            }
        }

        public string HomePage
        {
            get
            {
                return this.homePage;
            }

            set
            {
                this.homePage = value;
                this.OnPropertyChanged("HomePage");
            }
        }

        public Collection<Product> Products
        {
            get
            {
                return this.products;
            }

            set
            {
                if (value != null)
                {
                    this.products = value;
                }
            }
        }

        public static Supplier CreateSupplier(int supplierID, string companyName)
        {
            return new Supplier
            {
                SupplierID = supplierID,
                CompanyName = companyName
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

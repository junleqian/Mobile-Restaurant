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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data.Services.Common;

    [EntitySetAttribute("Customers")]
    [DataServiceKeyAttribute("CustomerID")]
    public class Customer : INotifyPropertyChanged
    {
        private int customerID;
        private string name;
        private string address;
        private string city;
        private string email;
        private string postalCode;
        private string phone;
        private string creditCard;
        private bool isverified;

        public event PropertyChangedEventHandler PropertyChanged;

        public int CustomerID
        {
            get
            {
                return this.customerID;
            }

            set
            {
                this.customerID = value;
                this.OnPropertyChanged("CustomerID");
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
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

        public string Email
        {
            get
            {
                return this.email;
            }

            set
            {
                this.email = value;
                this.OnPropertyChanged("Email");
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

        public string CreditCard
        {
            get
            {
                return this.creditCard;
            }

            set
            {
                this.creditCard = value;
                this.OnPropertyChanged("CreditCard");
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


        public bool isVerified
        {
            get
            {
                return this.isverified;
            }

            set
            {
                this.isverified = value;
                this.OnPropertyChanged("isVerified");
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

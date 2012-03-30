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

    [EntitySetAttribute("Restaurants")]
    [DataServiceKeyAttribute("RestaurantID")]
    public class Restaurant : INotifyPropertyChanged
    {
        private int restaurantID;
        private string restaurantName;
        private string address;
        private string city;
        private string region;
        private string postalCode;
        private string phone;
        private string homePage;
        private string description;
        private Collection<Dish> dishes = new Collection<Dish>();
        private int? scheduleID;

        public event PropertyChangedEventHandler PropertyChanged;

        public int RestaurantID
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

        public string RestaurantName
        {
            get
            {
                return this.restaurantName;
            }

            set
            {
                this.restaurantName = value;
                this.OnPropertyChanged("RestaurantName");
            }
        }

        public int? ScheduleID
        {
            get
            {
                return this.scheduleID;
            }

            set
            {
                this.scheduleID = value;
                this.OnPropertyChanged("ScheduleID");
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

        public Collection<Dish> Dishes
        {
            get
            {
                return this.dishes;
            }

            set
            {
                if (value != null)
                {
                    this.dishes = value;
                }
            }
        }

        public static Restaurant CreateRestaurant(int restaurantID, string restaurantName)
        {
            return new Restaurant
            {
                RestaurantID = restaurantID,
                RestaurantName = restaurantName
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

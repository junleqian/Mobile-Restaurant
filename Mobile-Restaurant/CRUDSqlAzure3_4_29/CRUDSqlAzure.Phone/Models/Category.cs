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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Data.Services.Common;

    [EntitySetAttribute("Categories")]
    [DataServiceKeyAttribute("CategoryID")]
    public class Category : INotifyPropertyChanged
    {
        private Collection<Dish> dishes = new Collection<Dish>();
        private int categoryID;
        private string categoryName;
        private string description;
        private byte[] picture;
        private int? availableStart;
        private int? availableEnd;

        public event PropertyChangedEventHandler PropertyChanged;

        public int CategoryID
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

        public int? AvailableStart
        {
            get
            {
                return this.availableStart;
            }

            set
            {
                this.availableStart = value;
                this.OnPropertyChanged("AvailableStart");
            }
        }

        public int? AvailableEnd
        {
            get
            {
                return this.availableEnd;
            }

            set
            {
                this.availableEnd = value;
                this.OnPropertyChanged("AvailableEnd");
            }
        }

        public byte[] Picture
        {
            get
            {
                if (this.picture != null)
                {
                    return (byte[])this.picture.Clone();
                }
                else
                {
                    return null;
                }
            }

            set
            {
                this.picture = value;
                this.OnPropertyChanged("Picture");
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

        public static Category CreateCategory(int categoryID, string categoryName)
        {
            return new Category
            {
                CategoryID = categoryID,
                CategoryName = categoryName
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

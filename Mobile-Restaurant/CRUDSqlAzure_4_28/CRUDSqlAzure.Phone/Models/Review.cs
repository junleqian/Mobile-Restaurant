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

    [EntitySetAttribute("Reviews")]
    [DataServiceKeyAttribute("ReviewID")]
    public class Review : INotifyPropertyChanged
    {
        private int reviewId;
        private int? restaurantID;
        private int? customerID;
        private int? numStars;
        private float? star1;
        private float? star2;
        private float? star3;
        private float? star4;
        private float? star5;
        private string comment;
        private string customerName;

        public event PropertyChangedEventHandler PropertyChanged;

        public int ReviewID
        {
            get
            {
                return this.reviewId;
            }

            set
            {
                this.reviewId = value;
                this.OnPropertyChanged("ReviewID");
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

        public int? CustomerID
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

        public int? NumStars
        {
            get
            {
                return this.numStars;
            }

            set
            {
                this.numStars = value;
                this.OnPropertyChanged("NumStars");
            }
        }

        public float? Star1
        {
            get
            {
                return this.star1;
            }

            set
            {
                this.star1 = value;
                this.OnPropertyChanged("Star1");
            }
        }

        public float? Star2
        {
            get
            {
                return this.star2;
            }

            set
            {
                this.star2 = value;
                this.OnPropertyChanged("Star2");
            }
        }
        public float? Star3
        {
            get
            {
                return this.star3;
            }

            set
            {
                this.star3 = value;
                this.OnPropertyChanged("Star3");
            }
        }
        public float? Star4
        {
            get
            {
                return this.star4;
            }

            set
            {
                this.star4 = value;
                this.OnPropertyChanged("Star4");
            }
        }
        public float? Star5
        {
            get
            {
                return this.star5;
            }

            set
            {
                this.star5 = value;
                this.OnPropertyChanged("Star5");
            }
        }

        public string Comment
        {
            get
            {
                return this.comment;
            }

            set
            {
                this.comment = value;
                this.OnPropertyChanged("Comment");
            }
        }

        public string CustomerName
        {
            get
            {
                return this.customerName;
            }

            set
            {
                this.customerName = value;
                this.OnPropertyChanged("CustomerName");
            }
        }


        public static Review CreateReview(int reviewID, int customerID, int restaurantID)
        {
            return new Review
            {
                ReviewID = reviewID,
                RestaurantID = restaurantID,
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

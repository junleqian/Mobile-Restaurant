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
    using System.Data.Services.Client;

    public class NorthwindContext : DataServiceContext
    {
        public NorthwindContext(Uri serviceRoot)
            : base(serviceRoot)
        {
            this.MergeOption = MergeOption.OverwriteChanges;
            this.SaveChangesDefaultOptions = SaveChangesOptions.ContinueOnError;
        }

        public void AddToCategories(Category category)
        {
            this.AddObject("Categories", category);
        }

        public void AddToDishes(Dish dish)
        {
            this.AddObject("Dishes", dish);
        }

        public void AddToRestaurants(Restaurant restaurant)
        {
            this.AddObject("Restaurants", restaurant);
        }

        public void AddToReservations(Reservation reservation)
        {
            this.AddObject("Reservations", reservation);
        }

        public void AttachToCategories(Category category)
        {
            this.AttachTo("Categories", category);
        }

        public void AttachToDishes(Dish dish)
        {
            this.AttachTo("Dishes", dish);
        }

        public void AttachToRestaurants(Restaurant restaurant)
        {
            this.AttachTo("Restaurants", restaurant);
        }

        public void AttachToReservations(Reservation reservation)
        {
            this.AttachTo("Reservations", reservation);
        }
    }
}

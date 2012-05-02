// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, Dishes, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, Dish, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace Microsoft.Samples.CRUDSqlAzure.Phone.Pages
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Helpers;
    using Microsoft.Samples.CRUDSqlAzure.Phone.ViewModel;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
    using System.Threading;

    public partial class ConfirmReservationPage : PhoneApplicationPage
    {
        public ConfirmReservationPage()
        {
            this.InitializeComponent();
            this.ViewModel = new ConfirmReservationPageViewModel();
            this.Loaded += (s, e) => this.ViewModel.LoadData();
            this.ViewModel.rItems.LoadCompleted += new EventHandler<System.Data.Services.Client.LoadCompletedEventArgs>(rItems_LoadCompleted);
            this.ViewModel.tItems.LoadCompleted += new EventHandler<System.Data.Services.Client.LoadCompletedEventArgs>(tItems_LoadCompleted);
        }
        
        bool rComplete = false;
        bool tComplete = false;

        void rItems_LoadCompleted(object sender, System.Data.Services.Client.LoadCompletedEventArgs e)
        {
            for (int i = 0; i < this.ViewModel.rItems.Count; i++)
            {
                //textBlock1.Text += this.ViewModel.rItems[i].TableID.ToString() + "\n";
            }
            rComplete = true;
            if (tComplete)
            {
                textBlock1.Text += "Reservations found\n";   
                bothComplete();
            }
            else
                textBlock1.Text = "Reservations found\n";   
        }

        void tItems_LoadCompleted(object sender, System.Data.Services.Client.LoadCompletedEventArgs e)
        {
            for (int i = 0; i < this.ViewModel.tItems.Count; i++)
            {
                //textBlock1.Text += this.ViewModel.tItems[i].TableID.ToString() + "\n";
            }
            tComplete = true;
            if (rComplete)
            {
                textBlock1.Text += "Tables found\n";
                bothComplete();
            }
            else
                textBlock1.Text = "Tables found\n";
        }

        private void bothComplete()
        {
            textBlock1.Text += "Finding an open table\n";
            int Ti = 0, Ri = 0, bestIndex = -1, bestNum = 10000;
            Reservation sentinal = new Reservation();
            sentinal.TableID = 10000;
            this.ViewModel.rItems.Insert(ViewModel.rItems.Count, sentinal);
            while (Ti < this.ViewModel.tItems.Count && Ri < this.ViewModel.rItems.Count)
            {
                if (this.ViewModel.tItems[Ti].TableID > this.ViewModel.rItems[Ri].TableID)
                {
                    Ri++;
                }
                else if (this.ViewModel.tItems[Ti].TableID < this.ViewModel.rItems[Ri].TableID)
                {
                    //textBlock1.Text += "Open table found - " + this.ViewModel.tItems[Ti].TableID + "\n";
                    if (this.ViewModel.tItems[Ti].NumSeats - this.ViewModel.Reservation.NumGuests < bestNum)
                    {
                        bestIndex = Ti;
                        bestNum = (int)(this.ViewModel.tItems[Ti].NumSeats - this.ViewModel.Reservation.NumGuests);
                        if (bestNum == 0)
                            break;
                    }
                    Ti++;
                }
                else
                {
                    Ti++;
                    Ri++;
                }
            }
            this.ViewModel.rItems.RemoveAt(this.ViewModel.rItems.Count - 1); //remove sentinal
            if (bestIndex == -1)
            {
                textBlock1.Text += "No open tables found :(\nPlease go back and try again\n";
                PhoneHelpers.SetIsolatedStorageSetting("goBack", false);
                //appropriate action here
            }
            else
            {
                textBlock1.Text += "Best Table Available is " + this.ViewModel.tItems[bestIndex].TableID.ToString() + " numSeats: " + this.ViewModel.tItems[bestIndex].NumSeats.ToString() + "\n";
                //make reservation here
                this.ViewModel.Reservation.RestaurantName = this.ViewModel.RestName;
                this.ViewModel.Reservation.TableID = this.ViewModel.tItems[bestIndex].TableID;
                this.ViewModel.Reservation.ReservationID = 79;
                this.ViewModel.Reservation.CustomerID = PhoneHelpers.GetIsolatedStorageSetting<int>("reg_id");
                this.ViewModel.SetReservationModel();
                textBlock1.Text += "Booking Reservation\n";
                this.ViewModel.SaveChangesSuccess += new EventHandler(ViewModel_SaveChangesSuccess);
                this.ViewModel.SaveReservation();
            }
            this.ViewModel.IsListing = false;
        }

        void ViewModel_SaveChangesSuccess(object sender, EventArgs e)
        {
            textBlock1.Text += "Reservation Successful!\n";
            textBlock1.Text += "\nGoing back in 3..";
            PhoneHelpers.SetIsolatedStorageSetting("goBack", true);
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            //NavigationService.GoBack();
            //escape here
        }

        System.Windows.Threading.DispatcherTimer timer;
        int ticks = 3;

        void timer_Tick(object sender, EventArgs e)
        {
            ticks--;
            textBlock1.Text += ticks.ToString() + "..";
            if (ticks > 0)
                return;
            timer.Stop();
            NavigationService.GoBack();
        }

        public ConfirmReservationPageViewModel ViewModel
        {
            get { return this.DataContext as ConfirmReservationPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            PhoneHelpers.SetApplicationState("UserBackPress", true);
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.ViewModel.rPageNumber = (int)PhoneHelpers.GetApplicationState<int>("CurrentrPageNumber");
            this.ViewModel.tPageNumber = (int)PhoneHelpers.GetApplicationState<int>("CurrenttPageNumber");
            string RestIDasString = this.NavigationContext.QueryString["RestaurantID"];
            this.ViewModel.RestID = int.Parse(RestIDasString);
            this.ViewModel.RestName = this.NavigationContext.QueryString["RestaurantName"];
            if (this.ViewModel.Reservation == null)
            {
                var reserve = PhoneHelpers.GetApplicationState<Reservation>("CurrentReservation");
                if (reserve == null)
                {
                    this.NavigationService.GoBack();
                }

                this.ViewModel.Reservation = reserve;
                PhoneHelpers.RemoveApplicationState("CurrentReservation");
            }
            //PageTitle.Text = this.NavigationContext.QueryString["RestaurantName"];
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (this.ViewModel != null)
            {
                PhoneHelpers.SetApplicationState("CurrentrPageNumber", this.ViewModel.rPageNumber);
                PhoneHelpers.SetApplicationState("CurrenttPageNumber", this.ViewModel.tPageNumber);
            }
        }

        private void ItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void LoadMoreItems(object sender, RoutedEventArgs e)
        {
            this.ViewModel.tLoadNextPage();
            this.ViewModel.rLoadNextPage();
        }

        private void OnAddDish(object sender, EventArgs e)
        {
            //PhoneHelpers.RemoveApplicationState("CurrentDishRow");
            //this.NavigationService.Navigate(new Uri("/Pages/DishDetailsPage.xaml?editDish=false", UriKind.Relative));
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            this.ViewModel.LoadData();
        }

        private void OnLogout(object sender, EventArgs e)
        {
            this.CleanUp();
        }
        
        private void CleanUp()
        {
            // Clean the current authentication token, flags and view models.
            App.CloudClientFactory.CleanAuthenticationToken();
            PhoneHelpers.SetApplicationState("UserBackPress", false);
            PhoneHelpers.RemoveIsolatedStorageSetting("UserIsRegistered");

            this.ViewModel = null;

            // Navigate to the log in page.
            this.NavigationService.GoBack();
        }
    }
}
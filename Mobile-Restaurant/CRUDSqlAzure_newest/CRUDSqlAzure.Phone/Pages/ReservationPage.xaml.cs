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
    using ReservationService3;
    using System.Data.Services.Client;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Models;

    public partial class ReservationPage : PhoneApplicationPage
    {
        public ReservationPage()
        {
            this.InitializeComponent();
            //serviceClient = new ReservationService3Client();
            //serviceClient.testService3Completed += new EventHandler<testService3CompletedEventArgs>(serviceClient_testService3Completed);
        }

        int RestID;
        string RestName;
        //private ReservationService3Client serviceClient;

        void serviceClient_testService3Completed(object sender, testService3CompletedEventArgs e)
        {
            if (e.Error == null || e.Result == null)
                return;
            submit_button.Content = e.Result;
        }

        private void submit_button_Click(object sender, RoutedEventArgs e)
        {
            //serviceClient.testService3Async("world!");
            //TODO: add something here
            Models.Reservation temp = new Reservation();
            temp.StartTime = ((DateTime)datePicker.Value).Date;
            temp.StartTime = temp.StartTime.AddHours(((DateTime)timePicker.Value).TimeOfDay.Hours);
            temp.StartTime = temp.StartTime.AddMinutes(((DateTime)timePicker.Value).TimeOfDay.Minutes);
            temp.EndTime = temp.StartTime;
            temp.EndTime = temp.EndTime.AddMinutes(45); //average reservation length could be defined by restaurant
            temp.RestaurantID = RestID;
            temp.NumGuests = Int32.Parse(guestsBox.Text);
            PhoneHelpers.SetApplicationState("CurrentReservation", temp);
            this.NavigationService.Navigate(new Uri("/Pages/ConfirmReservationPage.xaml?RestaurantID=" + RestID.ToString() + "&RestaurantName=" + RestName, UriKind.Relative));
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            PhoneHelpers.SetApplicationState("UserBackPress", true);
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string RestIDasString = this.NavigationContext.QueryString["RestaurantID"];
            RestID = int.Parse(RestIDasString);
            RestName = this.NavigationContext.QueryString["RestaurantName"];

            //PageTitle.Text = this.NavigationContext.QueryString["RestaurantName"];
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
        }

        private void OnRefresh(object sender, EventArgs e)
        {
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
            //serviceClient.CloseAsync();

            // Navigate to the log in page.
            this.NavigationService.GoBack();
        }

    }
}
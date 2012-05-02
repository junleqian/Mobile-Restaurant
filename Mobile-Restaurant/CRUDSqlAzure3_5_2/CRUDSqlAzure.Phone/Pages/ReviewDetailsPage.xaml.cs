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

namespace Microsoft.Samples.CRUDSqlAzure.Phone.Pages
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Helpers;
    using Microsoft.Samples.CRUDSqlAzure.Phone.Models;
    using Microsoft.Samples.CRUDSqlAzure.Phone.ViewModel;

    public partial class ReviewDetailsPage : PhoneApplicationPage
    {
        public ReviewDetailsPage()
        {
            this.InitializeComponent();

            this.ViewModel = new ReviewDetailsPageViewModel();
            this.ViewModel.SaveChangesSuccess += (s, e) => this.NavigationService.GoBack();
        }

        public ReviewDetailsPageViewModel ViewModel
        {
            get { return this.DataContext as ReviewDetailsPageViewModel; }
            set { this.DataContext = value; }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var editReviewAsString = this.NavigationContext.QueryString["editReview"];
            bool editReview;

            if (string.IsNullOrWhiteSpace(editReviewAsString) || !bool.TryParse(editReviewAsString, out editReview))
            {
                editReview = false;
            }

            if (this.ViewModel.Review == null)
            {
                var review = PhoneHelpers.GetApplicationState<Review>("CurrentReviewRow");
                if (editReview && (review == null))
                {
                    this.NavigationService.GoBack();
                }

                this.ViewModel.SetReviewModel(review);
                this.ViewModel.Review.CustomerID = PhoneHelpers.GetIsolatedStorageSetting<int>("reg_id");
                this.ViewModel.Review.CustomerName = PhoneHelpers.GetIsolatedStorageSetting<string>("reg_username");
                this.ViewModel.Review.RestaurantID = Int32.Parse(this.NavigationContext.QueryString["RestaurantID"]);
                this.ViewModel.Review.ReviewID = -1;
                PhoneHelpers.RemoveApplicationState("CurrentReviewRow");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.ViewModel.DetachReviewModel();
        }

        private void OnDeleteReview(object sender, EventArgs e)
        {
            this.ViewModel.DeleteReview();
        }

        private void OnSaveReview(object sender, EventArgs e)
        {
            var focusTextbox = FocusManager.GetFocusedElement() as TextBox;
            if (focusTextbox != null)
            {
                var binding = focusTextbox.GetBindingExpression(TextBox.TextProperty);
                binding.UpdateSource();
            }

            this.ViewModel.SaveReview();
        }

        private void OnNavigatePage(object sender, NavigationEventArgs e)
        {
            this.NavigationService.Navigate(e.Uri);
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var img = sender as Image;
            string star = img.Name;
            char cNum = star[4];
            int numStars = Int32.Parse(cNum.ToString());
            this.ViewModel.Review.NumStars = numStars;
            this.ViewModel.Review.Star1 = 1.0f;
            if(numStars > 1)
                this.ViewModel.Review.Star2 = 1.0f;
            else
                this.ViewModel.Review.Star2 = 0.5f;
            if (numStars > 2)
                this.ViewModel.Review.Star3 = 1.0f;
            else
                this.ViewModel.Review.Star3 = 0.5f;
            if (numStars > 3)
                this.ViewModel.Review.Star4 = 1.0f;
            else
                this.ViewModel.Review.Star4 = 0.5f;
            if (numStars > 4)
                this.ViewModel.Review.Star5 = 1.0f;
            else
                this.ViewModel.Review.Star5 = 0.5f;
        }
    }
}
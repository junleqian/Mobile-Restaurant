namespace Gour.Phone.PivotContent
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Gour.Phone.ViewModel;

    public partial class NotificationsPage : UserControl
    {
        public NotificationsPage()
        {
            this.InitializeComponent();

            this.Loaded += this.OnNotificationsPageLoaded;
        }

        public event EventHandler<EventArgs> BeginPushConnection
        {
            add { this.ViewModel.BeginPushConnection += value; }
            remove { this.ViewModel.BeginPushConnection -= value; }
        }

        public event EventHandler<EventArgs> EndPushConnection
        {
            add { this.ViewModel.EndPushConnection += value; }
            remove { this.ViewModel.EndPushConnection -= value; }
        }

        public NotificationsPageViewModel ViewModel
        {
            get { return this.DataContext as NotificationsPageViewModel; }
            set { this.DataContext = value; }
        }

        private void OnNotificationsPageLoaded(object sender, RoutedEventArgs args)
        {
            if (this.ViewModel != null)
            {
                this.ViewModel.CleanPushNotificationServiceException();
                this.ViewModel.PushNotificationServiceException += (s, e) => MessageBox.Show(e.Value, "Push Notification Error", MessageBoxButton.OK);

                if (this.ViewModel.IsPushEnabled)
                {
                    this.ViewModel.ConnectToPushNotificationService();
                }
            }
        }
    }
}
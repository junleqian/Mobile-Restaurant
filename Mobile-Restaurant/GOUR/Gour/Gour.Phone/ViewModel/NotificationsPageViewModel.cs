namespace Gour.Phone.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Threading;
    using Microsoft.Phone.Notification;
    using Microsoft.Phone.Shell;
    using Gour.Phone.Push;

    public class NotificationsPageViewModel : PivotItemViewModel
    {
        private const string IconsRootUri = "/Toolkit.Content/";

        private readonly Dispatcher dispatcher;
        private readonly ISamplePushUserRegistrationClient pushNotificationClient;

        private string message;
        private bool canEnableOrDisablePush = true;

        public NotificationsPageViewModel()
            : this(App.CloudClientFactory.ResolvePushNotificationClient(), Deployment.Current.Dispatcher)
        {
        }

        public NotificationsPageViewModel(ISamplePushUserRegistrationClient pushNotificationClient, Dispatcher dispatcher)
        {
            this.pushNotificationClient = pushNotificationClient;
            this.dispatcher = dispatcher;
            this.Notifications = new ObservableCollection<string>();

            PushContext.Current.RawNotification += this.OnRawNotification;
            PushContext.Current.PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName.Equals("IsConnected", StringComparison.OrdinalIgnoreCase))
                    {
                        this.NotifyPropertyChanged("ConnectionStatus");
                    }
                };
        }

        public event EventHandler<EventArgs<string>> PushNotificationServiceException;

        public event EventHandler<EventArgs> BeginPushConnection;

        public event EventHandler<EventArgs> EndPushConnection;

        public bool IsPushEnabled
        {
            get
            {
                return PushContext.Current.IsPushEnabled;
            }

            set
            {
                if (PushContext.Current.IsPushEnabled != value)
                {
                    PushContext.Current.IsPushEnabled = value;

                    if (PushContext.Current.IsPushEnabled)
                    {
                        this.ConnectToPushNotificationService();
                    }
                    else
                    {
                        this.DisconnectFromPushNotificationService();
                    }

                    this.NotifyPropertyChanged("IsPushEnabled");
                }
            }
        }

        public bool CanEnableOrDisablePush
        {
            get
            {
                return this.canEnableOrDisablePush;
            }

            set
            {
                if (this.canEnableOrDisablePush != value)
                {
                    this.canEnableOrDisablePush = value;
                    this.NotifyPropertyChanged("CanEnableOrDisablePush");
                }
            }
        }

        public string ConnectionStatus
        {
            get
            {
                return PushContext.Current.IsConnected ? "Connected" : "Disconnected";
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.NotifyPropertyChanged("Message");
                }
            }
        }

        public ObservableCollection<string> Notifications { get; private set; }

        public void GetNotificationUpdates()
        {
            this.Message = "Getting last notification updates...";

            this.pushNotificationClient.GetUpdates(
                r =>
                {
                    this.dispatcher.BeginInvoke(
                        () =>
                        {
                            if (r.Exception == null)
                            {
                                foreach (var message in r.Response)
                                {
                                    this.Notifications.Insert(0, message);
                                }

                                this.Message = string.Format(CultureInfo.InvariantCulture, "Notification updates received at {0}.", DateTime.Now.ToLongTimeString());
                            }
                            else
                            {
                                this.Message = r.Exception.Message;
                            }

                            this.CanEnableOrDisablePush = true;
                            this.RaiseEndPushConnection();
                        });
                });
        }

        public void CleanPushNotificationServiceException()
        {
            this.PushNotificationServiceException = null;
        }

        public void ConnectToPushNotificationService()
        {
            this.Message = "Registering with Push Notification Service...";
            this.CanEnableOrDisablePush = false;
            this.RaiseBeginPushConnection();

            this.pushNotificationClient.Connect(
                r =>
                {
                    this.dispatcher.BeginInvoke(
                        () =>
                        {
                            if (r.Exception == null)
                            {
                                this.GetNotificationUpdates();
                            }
                            else
                            {
                                this.Message = "Error connecting from Push Notification Service.";
                                this.CanEnableOrDisablePush = true;

                                this.RaisePushNotificationServiceException(r.Exception.Message);
                                this.RaiseEndPushConnection();
                            }
                        });
                });
        }

        public void DisconnectFromPushNotificationService()
        {
            this.RaiseBeginPushConnection();

            this.pushNotificationClient.Disconnect(
                r =>
                {
                    this.dispatcher.BeginInvoke(
                        () =>
                        {
                            if (r.Exception == null)
                            {
                                this.Message = "Disconnected from Push Notification Service.";
                            }
                            else
                            {
                                this.Message = "Error disconnecting from Push Notification Service.";
                                this.RaisePushNotificationServiceException(r.Exception.Message);
                            }

                            this.CanEnableOrDisablePush = true;
                            this.RaiseEndPushConnection();
                        });
                });
        }

        protected override void PopulateApplicationBarButtons(IApplicationBar applicationBar)
        {
            var clearMessagesButton = new ApplicationBarIconButton(new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}", IconsRootUri, "appbar.delete.rest.png"), UriKind.Relative)) { Text = "clear" };
            clearMessagesButton.Click += (s, e) => this.Notifications.Clear();

            applicationBar.Buttons.Add(clearMessagesButton);
        }

        private void OnRawNotification(object sender, HttpNotificationEventArgs e)
        {
            try
            {
                e.Notification.Body.Position = 0;

                var stream = new StreamReader(e.Notification.Body);
                var rawMessage = stream.ReadToEnd();
                this.Notifications.Insert(0, rawMessage);

                this.Message = string.Format(CultureInfo.InvariantCulture, "Push notification received at {0}.", DateTime.Now.ToLongTimeString());
            }
            catch (Exception exception)
            {
                this.Message = string.Format(CultureInfo.InvariantCulture, "There was an error receiving last push notification: {0}", exception.Message);
            }
        }

        private void RaisePushNotificationServiceException(string errorMessage)
        {
            var pushNotificationServiceException = this.PushNotificationServiceException;
            if (pushNotificationServiceException != null)
            {
                pushNotificationServiceException(this, new EventArgs<string>(errorMessage));
            }
        }

        private void RaiseBeginPushConnection()
        {
            var beginPushConnection = this.BeginPushConnection;
            if (beginPushConnection != null)
            {
                beginPushConnection(this, EventArgs.Empty);
            }
        }

        private void RaiseEndPushConnection()
        {
            var endPushConnection = this.EndPushConnection;
            if (endPushConnection != null)
            {
                endPushConnection(this, EventArgs.Empty);
            }
        }
    }
}

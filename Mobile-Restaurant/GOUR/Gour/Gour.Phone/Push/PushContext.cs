namespace Gour.Phone.Push
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Threading;
    using Microsoft.Phone.Notification;
    using Gour.Phone.Helpers;

    public sealed class PushContext : INotifyPropertyChanged
    {
        #region Fields

        private static PushContext current;
        private bool isConnected;

        #endregion

        #region Ctor

        public PushContext(string channelName, string serviceName, IList<Uri> allowedDomains, Dispatcher dispatcher)
        {
            if (current != null)
            {
                return;
            }

            this.ChannelName = channelName;
            this.ServiceName = serviceName;
            this.AllowedDomains = allowedDomains;
            this.Dispatcher = dispatcher;

            current = this;
        }

        #endregion

        #region Events
        public event EventHandler<PushContextErrorEventArgs> Error;

        public event EventHandler<PushContextEventArgs> ChannelPrepared;

        public event EventHandler<HttpNotificationEventArgs> RawNotification;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion

        #region Properties

        public static PushContext Current
        {
            get { return current; }
        }

        public string ChannelName { get; private set; }

        public string ServiceName { get; private set; }

        public IList<Uri> AllowedDomains { get; private set; }

        public HttpNotificationChannel NotificationChannel { get; private set; }

        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }

            set
            {
                if (this.isConnected != value)
                {
                    this.isConnected = value;
                    this.NotifyPropertyChanged("IsConnected");
                }
            }
        }

        public bool IsPushEnabled
        {
            get { return PhoneHelpers.GetIsolatedStorageSetting<bool>("PushContext.IsPushEnabled"); }
            set { PhoneHelpers.SetIsolatedStorageSetting("PushContext.IsPushEnabled", value); }
        }

        private Dispatcher Dispatcher { get; set; }

        #endregion

        #region Public Methods
        public void Connect(Action<HttpNotificationChannel> callback)
        {
            if (this.IsConnected)
            {
                callback(this.NotificationChannel);
            }
            else
            {
                try
                {
                    // First, try to pick up an existing channel.
                    this.NotificationChannel = HttpNotificationChannel.Find(this.ChannelName);

                    if (this.NotificationChannel == null)
                    {
                        this.CreateChannel(callback);
                    }
                    else
                    {
                        this.PrepareChannel(callback);
                    }

                    this.UpdateNotificationBindings();
                    this.IsConnected = true;
                }
                catch (Exception ex)
                {
                    this.OnError(ex);
                }
            }
        }

        public void Disconnect()
        {
            if (!this.IsConnected)
            {
                return;
            }

            try
            {
                if (this.NotificationChannel != null)
                {
                    this.UnbindFromTileNotifications();
                    this.UnbindFromToastNotifications();
                    this.UnsubscribeToNotificationEvents();
                    this.NotificationChannel.Close();
                }
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
            finally
            {
                this.NotificationChannel = null;
                this.IsConnected = false;
            }
        }
        #endregion

        #region Privates
        /// <summary>
        /// Create channel, subscribe to channel events and open the channel.
        /// </summary>
        private void CreateChannel(Action<HttpNotificationChannel> prepared)
        {
            // Create a new channel.
            this.NotificationChannel = new HttpNotificationChannel(this.ChannelName, this.ServiceName);

            // Register to UriUpdated event. This occurs when channel successfully opens.
            this.NotificationChannel.ChannelUriUpdated += (s, e) => this.Dispatcher.BeginInvoke(() => this.PrepareChannel(prepared));

            // Trying to Open the channel.
            this.NotificationChannel.Open();
        }

        private void SubscribeToNotificationEvents()
        {
            if (this.NotificationChannel != null)
            {
                this.NotificationChannel.HttpNotificationReceived += this.OnRawNotificationReceived;
            }
        }

        private void UnsubscribeToNotificationEvents()
        {
            if (this.NotificationChannel != null)
            {
                this.NotificationChannel.HttpNotificationReceived -= this.OnRawNotificationReceived;
            }
        }

        private void OnRawNotificationReceived(object sender, HttpNotificationEventArgs args)
        {
            this.Dispatcher.BeginInvoke(() =>
            {
                if (this.RawNotification != null)
                {
                    this.RawNotification(this, args);
                }
            });
        }

        private void PrepareChannel(Action<HttpNotificationChannel> prepared)
        {
            try
            {
                prepared(this.NotificationChannel);
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        private void OnError(Exception exception)
        {
            if (this.Error != null)
            {
                this.Error(this, new PushContextErrorEventArgs(exception));
            }
        }

        private void OnChannelPrepared(PushContextEventArgs args)
        {
            if (this.ChannelPrepared != null)
            {
                this.ChannelPrepared(this, args);
            }
        }

        private void BindToTileNotifications()
        {
            try
            {
                if (this.NotificationChannel != null && !this.NotificationChannel.IsShellTileBound)
                {
                    var listOfAllowedDomains = new Collection<Uri>(this.AllowedDomains);
                    this.NotificationChannel.BindToShellTile(listOfAllowedDomains);
                }
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        private void BindToToastNotifications()
        {
            try
            {
                if (this.NotificationChannel != null && !this.NotificationChannel.IsShellToastBound)
                {
                    this.NotificationChannel.BindToShellToast();
                }
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        private void UnbindFromTileNotifications()
        {
            try
            {
                if (this.NotificationChannel != null && this.NotificationChannel.IsShellTileBound)
                {
                    this.NotificationChannel.UnbindToShellTile();
                }
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        private void UnbindFromToastNotifications()
        {
            try
            {
                if (this.NotificationChannel != null && this.NotificationChannel.IsShellToastBound)
                {
                    this.NotificationChannel.UnbindToShellToast();
                }
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        private void UpdateNotificationBindings()
        {
            if (this.IsPushEnabled)
            {
                this.BindToTileNotifications();
                this.BindToToastNotifications();
                this.SubscribeToNotificationEvents();
            }
            else
            {
                this.UnbindFromTileNotifications();
                this.UnbindFromToastNotifications();
                this.UnsubscribeToNotificationEvents();
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

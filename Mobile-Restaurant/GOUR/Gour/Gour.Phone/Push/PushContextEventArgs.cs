namespace Gour.Phone.Push
{
    using System;
    using Microsoft.Phone.Notification;

    public class PushContextEventArgs : EventArgs
    {
        internal PushContextEventArgs(HttpNotificationChannel channel)
        {
            this.NotificationChannel = channel;
        }

        public HttpNotificationChannel NotificationChannel { get; private set; }
    }
}

namespace Gour.Web.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class PushUserEndpoint
    {
        [Key, Column(Order = 0)]
        public string ApplicationId { get; set; }

        [Key, Column(Order = 1)]
        public string DeviceId { get; set; }

        public string ChannelUri { get; set; }

        public string UserId { get; set; }

        public int TileCount { get; set; }

        public ICollection<QueuedPushNotification> QueuedPushNotifications { get; set; }
    }
}
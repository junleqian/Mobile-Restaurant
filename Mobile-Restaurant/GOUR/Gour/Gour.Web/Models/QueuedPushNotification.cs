namespace Gour.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class QueuedPushNotification
    {
        [Key]
        public int QueuedPushNotificationId { get; set; }

        public string ChannelUri { get; set; }

        [Required]
        public PushUserEndpoint PushUserEndpoint { get; set; }

        public string Message { get; set; }
    }
}
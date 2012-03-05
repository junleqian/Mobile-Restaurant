namespace Gour.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Web.Mvc;
    using WindowsPhone.Recipes.Push.Messages;
    using Gour.Web.Infrastructure;
    using Gour.Web.Models;
    using Gour.Web.UserAccountWrappers;

    [CustomAuthorize(Roles = PrivilegeConstants.AdminPrivilege)]
    public class PushNotificationsController : Controller
    {
        private readonly IPushUserEndpointsRepository pushUserEndpointsRepository;

        private readonly IMembershipService membershipService;

        public PushNotificationsController()
            : this(new SqlDataContext(), new AccountMembershipService())
        {
        }

        [CLSCompliant(false)]
        public PushNotificationsController(IPushUserEndpointsRepository pushUserEndpointsRepository, IMembershipService membershipService)
        {
            this.membershipService = membershipService;
            this.pushUserEndpointsRepository = pushUserEndpointsRepository;
        }

        public ActionResult Microsoft()
        {
            var users = this.pushUserEndpointsRepository
                .GetAllPushUsers()
                .Select(userId => new UserModel { UserId = userId, UserName = this.membershipService.GetUserByProviderUserKey(new Guid(userId)).UserName });

            return this.View(users);
        }


        [HttpPost]
        public ActionResult SendMicrosoftToast(string userId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return this.Json("The notification message cannot be null, empty nor white space.", JsonRequestBehavior.AllowGet);
            }

            var resultList = new List<MessageSendResultLight>();
            var uris = this.pushUserEndpointsRepository.GetPushUsersByName(userId).Select(u => u.ChannelUri);
            var toast = new ToastPushNotificationMessage
            {
                SendPriority = MessageSendPriority.High,
                Title = message
            };

            foreach (var uri in uris)
            {
                var messageResult = toast.SendAndHandleErrors(new Uri(uri));
                resultList.Add(messageResult);
                if (messageResult.Status.Equals(MessageSendResultLight.Success))
                {
                    this.QueueMessage(message, new Uri(uri));
                }
            }

            return this.Json(resultList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendMicrosoftTile(string userId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return this.Json("The notification message cannot be null, empty nor white space.", JsonRequestBehavior.AllowGet);
            }

            var resultList = new List<MessageSendResultLight>();
            var pushUserEndpointList = this.pushUserEndpointsRepository.GetPushUsersByName(userId);
            foreach (var pushUserEndpoint in pushUserEndpointList)
            {
                var tile = new TilePushNotificationMessage
                {
                    SendPriority = MessageSendPriority.High,
                    Count = ++pushUserEndpoint.TileCount
                };

                var messageResult = tile.SendAndHandleErrors(new Uri(pushUserEndpoint.ChannelUri));
                resultList.Add(messageResult);
                if (messageResult.Status.Equals(MessageSendResultLight.Success))
                {
                    this.QueueMessage(message, new Uri(pushUserEndpoint.ChannelUri));

                    this.pushUserEndpointsRepository.UpdatePushUserEndpoint(pushUserEndpoint);
                }
            }

            return this.Json(resultList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SendMicrosoftRaw(string userId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return this.Json("The notification message cannot be null, empty nor white space.", JsonRequestBehavior.AllowGet);
            }

            var resultList = new List<MessageSendResultLight>();
            var uris = this.pushUserEndpointsRepository.GetPushUsersByName(userId).Select(u => u.ChannelUri);
            var raw = new RawPushNotificationMessage
            {
                SendPriority = MessageSendPriority.High,
                RawData = Encoding.UTF8.GetBytes(message)
            };

            foreach (var uri in uris)
            {
                resultList.Add(raw.SendAndHandleErrors(new Uri(uri)));
            }

            return this.Json(resultList, JsonRequestBehavior.AllowGet);
        }

        private void QueueMessage(string message, Uri uri)
        {
            var userEndpoint = this.pushUserEndpointsRepository.GetPushUserEndpointByChannel(uri);
            if (userEndpoint == null)
            {
                throw new ArgumentException("The Channel URI is invalid", "uri");
            }

            this.pushUserEndpointsRepository.AddQueuedPushNotification(
                new QueuedPushNotification
                {
                    PushUserEndpoint = userEndpoint,
                    ChannelUri = userEndpoint.ChannelUri,
                    Message = message,
                });
        }
    }
}

namespace Gour.Web.Services
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web;
    using System.Web.Security;
    using WindowsPhone.Recipes.Push.Messages;
    using Gour.Web.Infrastructure;
    using Gour.Web.Models;
    using Gour.Web.UserAccountWrappers;

    [ServiceBehavior(IncludeExceptionDetailInFaults = false)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SamplePushUserRegistrationService : ISamplePushUserRegistrationService
    {
        private readonly HttpContextBase context;
        private readonly IPushUserEndpointsRepository pushUserEndpointsRepository;
        private readonly WebOperationContext webOperationContext;

        private readonly IFormsAuthentication formsAuth;
        private readonly IMembershipService membershipService;

        public SamplePushUserRegistrationService()
            : this(new HttpContextWrapper(HttpContext.Current), WebOperationContext.Current, new FormsAuthenticationService(), new AccountMembershipService(), new SqlDataContext())
        {
        }

        [CLSCompliant(false)]
        public SamplePushUserRegistrationService(HttpContextBase context, WebOperationContext webOperationContext, IFormsAuthentication formsAuth, IMembershipService membershipService, IPushUserEndpointsRepository pushUserEndpointsRepository)
        {
            if ((context == null) && (HttpContext.Current == null))
            {
                throw new ArgumentNullException("context", "Context cannot be null if not running on a Web context.");
            }

            if (pushUserEndpointsRepository == null)
            {
                throw new ArgumentNullException("pushUserEndpointsRepository", "PushUserEndpoints repository cannot be null.");
            }

            if (formsAuth == null)
            {
                throw new ArgumentNullException("formsAuth", "Forms Authentication service cannot be null.");
            }

            if (membershipService == null)
            {
                throw new ArgumentNullException("membershipService", "Membership service cannot be null.");
            }

            this.webOperationContext = webOperationContext;
            this.context = context;
            this.pushUserEndpointsRepository = pushUserEndpointsRepository;
            this.formsAuth = formsAuth;
            this.membershipService = membershipService;
        }

        private string UserId
        {
            get
            {
                string ticketValue = null;

                var cookie = this.context.Request.Cookies[this.formsAuth.FormsCookieName];
                if (cookie != null)
                {
                    // From cookie.
                    ticketValue = cookie.Value;
                }
                else if (this.context.Request.Headers["AuthToken"] != null)
                {
                    // From HTTP header.
                    ticketValue = this.context.Request.Headers["AuthToken"];
                }

                if (!string.IsNullOrWhiteSpace(ticketValue))
                {
                    FormsAuthenticationTicket ticket;

                    try
                    {
                        ticket = this.formsAuth.Decrypt(ticketValue);
                    }
                    catch
                    {
                        throw new WebFaultException<string>("The authorization ticket cannot be decrypted.", HttpStatusCode.Unauthorized);
                    }

                    if (ticket != null)
                    {
                        return this.membershipService.GetUser(new FormsIdentity(ticket).Name).ProviderUserKey.ToString();
                    }
                    else
                    {
                        throw new WebFaultException<string>("The authorization token is no longer valid.", HttpStatusCode.Unauthorized);
                    }
                }
                else
                {
                    throw new WebFaultException<string>("Resource not found.", HttpStatusCode.NotFound);
                }
            }
        }

        public string Register(PushUserServiceRequest pushUserRegister)
        {
            // Authenticate.
            var userId = this.UserId;

            try
            {
                var pushUserEndpoint = this.pushUserEndpointsRepository.GetPushUserByApplicationAndDevice(pushUserRegister.ApplicationId, pushUserRegister.DeviceId);
                if (pushUserEndpoint == null)
                {
                    var newPushUserEndPoint = new PushUserEndpoint() { ApplicationId = pushUserRegister.ApplicationId, DeviceId = pushUserRegister.DeviceId, ChannelUri = pushUserRegister.ChannelUri.ToString(), UserId = userId };
                    this.pushUserEndpointsRepository.AddPushUserEndpoint(newPushUserEndPoint);
                }
                else
                {
                    // If the user did not change the channel URI, then, there is nothing to update, otherwise, set the new connection status
                    if (!pushUserEndpoint.ChannelUri.Equals(pushUserRegister.ChannelUri.ToString()) ||
                        !pushUserEndpoint.UserId.Equals(userId))
                    {
                        // Update che channelUri for the UserEndpoint and reset status fields.
                        pushUserEndpoint.ChannelUri = pushUserRegister.ChannelUri.ToString();
                        pushUserEndpoint.UserId = userId;
                    }

                    this.pushUserEndpointsRepository.UpdatePushUserEndpoint(pushUserEndpoint);
                }
            }
            catch (Exception exception)
            {
                throw new WebFaultException<string>(
                    string.Format(CultureInfo.InvariantCulture, "There was an error registering the Push Notification Endpoint: {0}", exception.Message),
                    HttpStatusCode.InternalServerError);
            }

            return "Success";
        }

        public string Unregister(PushUserServiceRequest pushUserUnregister)
        {
            // Authenticate.
            var userId = this.UserId;

            try
            {
                this.pushUserEndpointsRepository.RemovePushUserEndpoint(new PushUserEndpoint() { ApplicationId = pushUserUnregister.ApplicationId, DeviceId = pushUserUnregister.DeviceId, UserId = userId });
            }
            catch (Exception exception)
            {
                throw new WebFaultException<string>(
                    string.Format(CultureInfo.InvariantCulture, "There was an error unregistering the Push Notification Endpoint: {0}", exception.Message),
                    HttpStatusCode.InternalServerError);
            }

            return "Success";
        }

        public string[] GetUpdates(string applicationId, string deviceId)
        {
            // Authenticate.
            var userId = this.UserId;

            this.webOperationContext.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");

            try
            {
                var pushUserEndpoint = this.pushUserEndpointsRepository.GetPushUserByApplicationAndDevice(applicationId, deviceId);
                var messages = this.pushUserEndpointsRepository.GetQueuedPushNotificationsByChannel(new Uri(pushUserEndpoint.ChannelUri)).ToArray();

                foreach (var message in messages)
                {
                    this.pushUserEndpointsRepository.DeleteQueuedMessage(message);
                }

                this.ResetTileNotificationCount(applicationId, deviceId);

                return messages.Select(m => m.Message).ToArray();
            }
            catch (Exception exception)
            {
                throw new WebFaultException<string>(string.Format(CultureInfo.InvariantCulture, "There was an error getting the push notification updates: {0}", exception.Message), HttpStatusCode.InternalServerError);
            }
        }

        private void ResetTileNotificationCount(string applicationId, string deviceId)
        {
            var pushUserEndpoint = this.pushUserEndpointsRepository.GetPushUserByApplicationAndDevice(applicationId, deviceId);

            pushUserEndpoint.TileCount = 0;

            var tile = new TilePushNotificationMessage
            {
                SendPriority = MessageSendPriority.High,
                Count = pushUserEndpoint.TileCount
            };

            // Send a new tile notification message to reset the count in the phone application.
            tile.SendAndHandleErrors(new Uri(pushUserEndpoint.ChannelUri));

            // Save the updated count.
            this.pushUserEndpointsRepository.UpdatePushUserEndpoint(pushUserEndpoint);
        }
    }
}
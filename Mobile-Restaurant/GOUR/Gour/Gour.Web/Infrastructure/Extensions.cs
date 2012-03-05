namespace Gour.Web.Infrastructure
{
    using System;
    using System.ServiceModel.Activation;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using WindowsPhone.Recipes.Push.Messages;

    public static class Extensions
    {

        [CLSCompliant(false)]
        public static MessageSendResultLight SendAndHandleErrors(this PushNotificationMessage message, Uri uri)
        {
            var result = default(MessageSendResultLight);
            try
            {
                var sendResult = message.Send(uri);
                result = sendResult.NotificationStatus == NotificationStatus.Received
                    ? new MessageSendResultLight { Status = MessageSendResultLight.Success }
                    : new MessageSendResultLight { Status = MessageSendResultLight.Error, Description = "The notification was not received by the device." };
            }
            catch (Exception exception)
            {
                result = new MessageSendResultLight { Status = MessageSendResultLight.Error, Description = exception.Message };
            }

            return result;
        }

        public static MvcHtmlString MenuItem(this HtmlHelper helper, string linkText, string actionName, string controllerName)
        {
            var li = new TagBuilder("li");
            var routeData = helper.ViewContext.RouteData;
            var currentAction = routeData.GetRequiredString("action");
            var currentController = routeData.GetRequiredString("controller");
            if (string.Equals(currentAction, actionName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(currentController, controllerName, StringComparison.OrdinalIgnoreCase))
            {
                li.AddCssClass("selected");
            }

            li.InnerHtml = helper.ActionLink(linkText, actionName, controllerName).ToHtmlString();
            return MvcHtmlString.Create(li.ToString());
        }

        public static void AddWcfServiceRoute(this RouteCollection routes, Type dataServiceType, string prefix)
        {
            routes.Add(new ServiceRoute(prefix, new AutomaticFormatServiceHostFactory(), dataServiceType));
        }

        public static void AddWcfServiceRoute<TService>(this RouteCollection routes, string prefix)
        {
            AddWcfServiceRoute(routes, typeof(TService), prefix);
        }
    }
}
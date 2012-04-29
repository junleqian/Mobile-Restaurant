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

namespace Microsoft.Samples.CRUDSqlAzure.Web.Infrastructure
{
    using System;
    using System.ServiceModel.Activation;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;

    public static class Extensions
    {
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
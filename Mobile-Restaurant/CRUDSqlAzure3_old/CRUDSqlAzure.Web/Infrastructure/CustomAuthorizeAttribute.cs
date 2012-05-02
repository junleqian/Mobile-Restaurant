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
    using System.Globalization;
    using System.Web.Mvc;
    using System.Web.Security;
    using Microsoft.Samples.CRUDSqlAzure.Web.Models;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private IUserPrivilegesRepository userPrivilegesRepository;

        public IUserPrivilegesRepository UserPrivilegesRepository
        {
            get
            {
                if (this.userPrivilegesRepository == null)
                {
                    this.userPrivilegesRepository = new InfrastructureEntities();
                }

                return this.userPrivilegesRepository;
            }

            private set
            {
                this.userPrivilegesRepository = value;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var cookie = filterContext.HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            var loginUrl = string.Format(CultureInfo.InvariantCulture, "~/Account/LogOn?returnUrl={0}", filterContext.HttpContext.Request.RawUrl);

            if (cookie == null)
            {
                filterContext.Result = new RedirectResult(loginUrl);
            }
            else
            {
                FormsAuthenticationTicket ticket = null;

                try
                {
                    ticket = FormsAuthentication.Decrypt(cookie.Value);
                }
                catch (ArgumentException)
                {
                    filterContext.Result = new RedirectResult(loginUrl);
                }

                if (ticket != null)
                {
                    var userId = Membership.GetUser(new FormsIdentity(ticket).Name).ProviderUserKey.ToString();
                    if (!this.UserPrivilegesRepository.HasUserPrivilege(userId, this.Roles))
                    {
                        filterContext.Result = new RedirectResult("~/Account/Unauthorized");
                    }
                }
            }
        }
    }
}
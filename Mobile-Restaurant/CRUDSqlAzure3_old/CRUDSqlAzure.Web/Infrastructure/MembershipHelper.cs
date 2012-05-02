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
    using System.Web;
    using System.Web.Security;
    using Microsoft.Samples.CRUDSqlAzure.Web.Infrastructure;
    using Microsoft.Samples.CRUDSqlAzure.Web.Models;

    public static class MembershipHelper
    {
        private static IUserPrivilegesRepository userPrivilegesRepository;

        public static string UserName
        {
            get
            {
                string ticketValue = null;

                var cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    ticketValue = cookie.Value;
                }

                if (!string.IsNullOrEmpty(ticketValue))
                {
                    FormsAuthenticationTicket ticket;

                    try
                    {
                        ticket = FormsAuthentication.Decrypt(ticketValue);
                    }
                    catch
                    {
                        return string.Empty;
                    }

                    if (ticket != null)
                    {
                        var identity = new FormsIdentity(ticket);

                        return identity.Name;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private static IUserPrivilegesRepository UserPrivilegesRepository
        {
            get
            {
                if (userPrivilegesRepository == null)
                {
                    userPrivilegesRepository = new InfrastructureEntities();
                }

                return userPrivilegesRepository;
            }
        }

        public static bool IsUserAdmin()
        {
            return IsUserLoggedIn() && UserPrivilegesRepository.HasUserPrivilege(Membership.GetUser(UserName).ProviderUserKey.ToString(), PrivilegeConstants.AdminPrivilege);
        }

        public static bool IsUserLoggedIn()
        {
            return !string.IsNullOrWhiteSpace(UserName);
        }
    }
}
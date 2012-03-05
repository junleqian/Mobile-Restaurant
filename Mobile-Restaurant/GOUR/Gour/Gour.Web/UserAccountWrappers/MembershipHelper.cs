namespace Gour.Web.UserAccountWrappers
{
    using System.Web;
    using System.Web.Security;
    using Gour.Web.Infrastructure;

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

                if (!string.IsNullOrWhiteSpace(ticketValue))
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

                    userPrivilegesRepository = new SqlDataContext();

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
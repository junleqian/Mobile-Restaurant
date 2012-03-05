namespace Gour.Web.UserAccountWrappers
{
    using System;
    using System.Web.Security;

    public class FormsAuthenticationService : IFormsAuthentication
    {
        public string FormsCookieName
        {
            get { return FormsAuthentication.FormsCookieName; }
        }

        public void SignIn(string userName, bool createPersistentCookie)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentNullException("userName", "User name cannot be null nor empty.");
            }

            FormsAuthentication.SetAuthCookie(userName, createPersistentCookie);
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        public string Encrypt(FormsAuthenticationTicket ticket)
        {
            return FormsAuthentication.Encrypt(ticket);
        }

        public FormsAuthenticationTicket Decrypt(string token)
        {
            return FormsAuthentication.Decrypt(token);
        }
    }
}
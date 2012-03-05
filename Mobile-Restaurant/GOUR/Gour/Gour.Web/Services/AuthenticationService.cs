namespace Gour.Web.Services
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web.Security;
    using Gour.Web.Infrastructure;
    using Gour.Web.Models;
    using Gour.Web.UserAccountWrappers;

    [ServiceBehavior(IncludeExceptionDetailInFaults = false)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IFormsAuthentication formsAuth;
        private readonly IMembershipService membershipService;
        private readonly IUserPrivilegesRepository userPrivilegesRepository;

        public AuthenticationService()
            : this(new FormsAuthenticationService(), new AccountMembershipService(), new SqlDataContext())
        {
        }

        [CLSCompliant(false)]
        public AuthenticationService(IFormsAuthentication formsAuth, IMembershipService membershipService, IUserPrivilegesRepository userPrivilegesRepository)
        {
            if (formsAuth == null)
            {
                throw new ArgumentNullException("formsAuth", "The Forms Authentication service cannot be null.");
            }

            if (membershipService == null)
            {
                throw new ArgumentNullException("membershipService", "The Membership service cannot be null.");
            }

            if (userPrivilegesRepository == null)
            {
                throw new ArgumentNullException("userPrivilegesRepository", "The User Privileges Repository cannot be null.");
            }

            this.formsAuth = formsAuth;
            this.membershipService = membershipService;
            this.userPrivilegesRepository = userPrivilegesRepository;
        }

        public string GenerateAuthToken(Login login)
        {
            if ((login == null) || string.IsNullOrWhiteSpace(login.UserName) || string.IsNullOrWhiteSpace(login.Password))
            {
                throw new WebFaultException<string>("Invalid credentials.", HttpStatusCode.BadRequest);
            }

            if (this.membershipService.ValidateUser(login.UserName, login.Password))
            {
                var user = this.membershipService.GetUser(login.UserName);
                var ticket = new FormsAuthenticationTicket(user.UserName, false, int.MaxValue);
                var token = this.formsAuth.Encrypt(ticket);

                return token;
            }

            return string.Empty;
        }

        public string ValidateAuthToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new WebFaultException<string>("Token cannot be null or empty.", HttpStatusCode.BadRequest);
            }

            var ticket = this.formsAuth.Decrypt(token);
            if (ticket != null)
            {
                return ticket.Name;
            }

            return null;
        }

        public string CreateUser(RegistrationUser user)
        {
            if ((user == null) || string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.EMail) || string.IsNullOrWhiteSpace(user.Password))
            {
                throw new WebFaultException<string>("Invalid user information.", HttpStatusCode.BadRequest);
            }

            var createStatus = this.membershipService.CreateUser(user.Name, user.Password, user.EMail);
            if (createStatus == MembershipCreateStatus.Success)
            {
                this.SetUserDefaultPermissions(this.membershipService.GetUser(user.Name).ProviderUserKey.ToString());
            }

            return createStatus.ToString();
        }

        private void SetUserDefaultPermissions(string userId)
        {
            this.userPrivilegesRepository.AddPrivilegeToUser(userId, PrivilegeConstants.SqlUsagePrivilege);
        }
    }
}
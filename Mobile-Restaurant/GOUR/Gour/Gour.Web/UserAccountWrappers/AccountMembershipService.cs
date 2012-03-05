namespace Gour.Web.UserAccountWrappers
{
    using System.Web.Security;

    public class AccountMembershipService : IMembershipService
    {
        private MembershipProvider provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            this.provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return this.provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            return this.provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            this.provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var currentUser = this.provider.GetUser(userName, true /* userIsOnline */);
            return currentUser.ChangePassword(oldPassword, newPassword);
        }

        public MembershipUser GetUserProfile(string userName)
        {
            return this.provider.GetUser(userName, false);
        }

        public MembershipUser GetUser(string userName)
        {
            return this.provider.GetUser(userName, false);
        }

        public MembershipUser GetUserByProviderUserKey(object providerUserKey)
        {
            return this.provider.GetUser(providerUserKey, false);
        }

        public MembershipUser CreateUser(string userName, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            return this.provider.CreateUser(userName, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
        }

        public MembershipUserCollection GetAllUsers()
        {
            return Membership.GetAllUsers();
        }
    }
}
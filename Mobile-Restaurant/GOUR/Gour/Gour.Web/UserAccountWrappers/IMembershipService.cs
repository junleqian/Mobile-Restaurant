namespace Gour.Web.UserAccountWrappers
{
    using System.Web.Security;

    public interface IMembershipService
    {
        int MinPasswordLength { get; }

        bool ValidateUser(string userName, string password);

        MembershipCreateStatus CreateUser(string userName, string password, string email);

        bool ChangePassword(string userName, string oldPassword, string newPassword);

        MembershipUser GetUserProfile(string userName);

        MembershipUser GetUser(string userName);

        MembershipUser GetUserByProviderUserKey(object providerKey);

        MembershipUser CreateUser(string userName, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status);

        MembershipUserCollection GetAllUsers();
    }
}
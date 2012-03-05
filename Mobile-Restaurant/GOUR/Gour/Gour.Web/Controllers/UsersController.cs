namespace Gour.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;
    using Gour.Web.Infrastructure;
    using Gour.Web.Models;
    using Gour.Web.UserAccountWrappers;

    [CustomAuthorize(Roles = PrivilegeConstants.AdminPrivilege)]
    public class UsersController : Controller
    {
        private readonly IUserPrivilegesRepository userPrivilegesRepository;

        private readonly IMembershipService membershipService;

        public UsersController()
            : this(new SqlDataContext(), new AccountMembershipService())
        {
        }

        [CLSCompliant(false)]
        public UsersController(IUserPrivilegesRepository userPrivilegesRepository, IMembershipService membershipService)
        {
            this.userPrivilegesRepository = userPrivilegesRepository;
            this.membershipService = membershipService;
        }

        public ActionResult Index()
        {
            var users = this.membershipService.GetAllUsers()
                .Cast<MembershipUser>()
                .Select(user => new UserPermissionsModel
                    {
                        UserName = user.UserName,
                        UserId = user.ProviderUserKey.ToString(),
                        SqlUsage = this.userPrivilegesRepository.HasUserPrivilege(user.ProviderUserKey.ToString(), PrivilegeConstants.SqlUsagePrivilege)
                    });

            return this.View(users);
        }

        [HttpPost]
        public void SetUserPermissions(string userId, bool useTables, bool useBlobs, bool useQueues, bool useSql)
        {
            this.SetStorageItemUsagePrivilege(useSql, userId, PrivilegeConstants.SqlUsagePrivilege);
        }

        private void SetStorageItemUsagePrivilege(bool allowAccess, string user, string privilege)
        {
            if (allowAccess)
            {
                this.userPrivilegesRepository.AddPrivilegeToUser(user, privilege);
            }
            else
            {
                this.userPrivilegesRepository.RemovePrivilegeFromUser(user, privilege);
            }
        }
    }
}

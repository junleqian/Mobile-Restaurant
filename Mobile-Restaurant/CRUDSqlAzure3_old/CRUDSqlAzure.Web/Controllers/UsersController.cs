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

namespace Microsoft.Samples.CRUDSqlAzure.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Microsoft.Samples.CRUDSqlAzure.Web.Infrastructure;
    using Microsoft.Samples.CRUDSqlAzure.Web.Models;

    [CustomAuthorize(Roles = PrivilegeConstants.AdminPrivilege)]
    public class UsersController : Controller
    {
        private readonly IUserPrivilegesRepository userPrivilegesRepository;

        private readonly IUserRepository userRepository;

        public UsersController()
            : this(null, null)
        {
        }

        public UsersController(IUserPrivilegesRepository userPrivilegesRepository, IUserRepository userRepository)
        {
            var context = default(InfrastructureEntities);
            if ((userPrivilegesRepository == null) || (userRepository == null))
            {
                context = new InfrastructureEntities();
            }

            this.userPrivilegesRepository = userPrivilegesRepository ?? context;
            this.userRepository = userRepository ?? context;
        }

        public ActionResult Index()
        {
            var users = this.userRepository.GetAllUsers()
                .Select(user => new UserPermissionsModel
                {
                    UserName = user.Name,
                    UserId = user.UserId,
                    SqlRead = this.userPrivilegesRepository.HasUserPrivilege(user.UserId, PrivilegeConstants.SqlReadPrivilege),
                    SqlCreate = this.userPrivilegesRepository.HasUserPrivilege(user.UserId, PrivilegeConstants.SqlCreatePrivilege),
                    SqlDelete = this.userPrivilegesRepository.HasUserPrivilege(user.UserId, PrivilegeConstants.SqlDeletePrivilege),
                    SqlUpdate = this.userPrivilegesRepository.HasUserPrivilege(user.UserId, PrivilegeConstants.SqlUpdatePrivilege)
                });

            return this.View(users);
        }

        [HttpPost]
        public void SetUserPermissions(UserPermissionsModel requestedUserPermissions)
        {
            this.SetStorageItemUsagePrivilege(requestedUserPermissions.SqlRead, requestedUserPermissions.UserId, PrivilegeConstants.SqlReadPrivilege);
            this.SetStorageItemUsagePrivilege(requestedUserPermissions.SqlCreate, requestedUserPermissions.UserId, PrivilegeConstants.SqlCreatePrivilege);
            this.SetStorageItemUsagePrivilege(requestedUserPermissions.SqlUpdate, requestedUserPermissions.UserId, PrivilegeConstants.SqlUpdatePrivilege);
            this.SetStorageItemUsagePrivilege(requestedUserPermissions.SqlDelete, requestedUserPermissions.UserId, PrivilegeConstants.SqlDeletePrivilege);
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

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

namespace Microsoft.Samples.CRUDSqlAzure.Web.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Samples.CRUDSqlAzure.Web.Infrastructure;

    public partial class InfrastructureEntities : IUserPrivilegesRepository, IUserRepository
    {
        public const string PushUserTableName = "PushUserEndpoints";

        private const string PublicUserId = "00000000-0000-0000-0000-000000000000";

        public void CreateUser(string userId, string userName, string email)
        {
            if (this.Users
                .Where(u => u.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) || u.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .FirstOrDefault() != null)
            {
                throw new System.ServiceModel.Web.WebFaultException<string>("A user with the same id or email already exists.", System.Net.HttpStatusCode.BadRequest);
            }

            var user = User.CreateUser(userId, userName);
            user.Email = email;

            this.AddToUsers(user);
            this.SaveChanges();
        }

        public IEnumerable<User> GetAllUsers()
        {
            return this.Users.ToList();
        }

        public User GetUser(string userId)
        {
            return this.Users
                .Where(u => u.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .FirstOrDefault();
        }

        public User GetUserByEmail(string email)
        {
            return this.Users
                .Where(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .FirstOrDefault();
        }

        public bool UserExists(string userId)
        {
            return this.GetUser(userId) != null;
        }

        public IEnumerable<UserPrivilege> GetUsersWithPrivilege(string privilege)
        {
            return this.UserPrivileges
                .Where(p => p.Privilege.Equals(privilege, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public void AddPrivilegeToUser(string userId, string privilege)
        {
            if (!this.HasUserPrivilege(userId, privilege))
            {
                this.AddToUserPrivileges(UserPrivilege.CreateUserPrivilege(Guid.NewGuid(), userId, privilege));
                this.SaveChanges();
            }
        }

        public void AddPublicPrivilege(string privilege)
        {
            this.AddPrivilegeToUser(PublicUserId, privilege);
        }

        public void RemovePrivilegeFromUser(string userId, string privilege)
        {
            var userPrivilege = this.GetUserPrivilege(userId, privilege);
            if (userPrivilege != null)
            {
                this.UserPrivileges.DeleteObject(userPrivilege);
                this.SaveChanges();
            }
        }

        public void DeletePublicPrivilege(string privilege)
        {
            this.RemovePrivilegeFromUser(PublicUserId, privilege);
        }

        public void DeletePrivilege(string privilege)
        {
            var userPrivileges = this.GetUsersWithPrivilege(privilege);
            foreach (var userPrivilege in userPrivileges)
            {
                this.UserPrivileges.DeleteObject(userPrivilege);
            }

            this.SaveChanges();
        }

        public bool HasUserPrivilege(string userId, string privilege)
        {
            return this.GetUserPrivilege(userId, privilege) != null;
        }

        public bool PublicPrivilegeExists(string privilege)
        {
            return this.HasUserPrivilege(PublicUserId, privilege);
        }

        private UserPrivilege GetUserPrivilege(string userId, string privilege)
        {
            return this.UserPrivileges
                .Where(p => p.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase) && p.Privilege.Equals(privilege, StringComparison.OrdinalIgnoreCase))
                .ToList()
                .FirstOrDefault();
        }
    }
}
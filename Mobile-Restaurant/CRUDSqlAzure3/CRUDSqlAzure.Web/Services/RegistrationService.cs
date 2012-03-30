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

namespace Microsoft.Samples.CRUDSqlAzure.Web.Services
{
    using System;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.Samples.CRUDSqlAzure.Web.Infrastructure;
    using Microsoft.Samples.CRUDSqlAzure.Web.Models;

    [ServiceBehavior(IncludeExceptionDetailInFaults = false)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RegistrationService : IRegistrationService
    {
        private readonly IClaimsIdentity identity;
        private readonly IUserRepository userRepository;
        private readonly IUserPrivilegesRepository userPrivilegesRepository;

        public RegistrationService()
            : this(HttpContext.Current.User.Identity as IClaimsIdentity, null, null)
        {
        }

        public RegistrationService(IClaimsIdentity identity, IUserRepository userRepository, IUserPrivilegesRepository userPrivilegesRepository)
        {
            var context = default(InfrastructureEntities);
            if ((userPrivilegesRepository == null) || (userRepository == null))
            {
                context = new InfrastructureEntities();
            }

            this.identity = identity;
            this.userRepository = userRepository ?? context;
            this.userPrivilegesRepository = userPrivilegesRepository ?? context;
        }

        protected string UserId
        {
            get
            {
                return this.identity.Claims.Single(c => c.ClaimType == ClaimTypes.NameIdentifier).Value;
            }
        }

        public string CreateUser(RegistrationUser user)
        {
            if ((user == null) || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.EMail))
            {
                throw new WebFaultException<string>("Invalid user information.", HttpStatusCode.BadRequest);
            }

            this.userRepository.CreateUser(this.UserId, user.Name, user.EMail);
            this.SetUserDefaultUserPrivileges(this.UserId);

            return "Success";
        }

        public string CheckUserRegistration()
        {
            return (this.userRepository.GetUser(this.UserId) != null).ToString();
        }

        private void SetUserDefaultUserPrivileges(string userId)
        {
            this.userPrivilegesRepository.AddPrivilegeToUser(userId, PrivilegeConstants.SqlReadPrivilege);
        }
    }
}
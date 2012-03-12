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
    using System.Data.Objects;
    using System.Data.Services;
    using System.Data.Services.Common;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.Web;
    using Microsoft.IdentityModel.Claims;
    using Microsoft.Samples.CRUDSqlAzure.Web.Infrastructure;
    using Microsoft.Samples.CRUDSqlAzure.Web.Models;

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class NorthwindODataService : DataService<NorthwindEntities>
    {
        private readonly IUserPrivilegesRepository userPrivilegesRepository;
        private readonly IClaimsIdentity identity;

        public NorthwindODataService()
            : this(HttpContext.Current.User.Identity as IClaimsIdentity, new InfrastructureEntities())
        {
        }

        public NorthwindODataService(IClaimsIdentity identity, IUserPrivilegesRepository userPrivilegesRepository)
        {
            this.identity = identity;
            this.userPrivilegesRepository = userPrivilegesRepository;
        }

        protected string UserId
        {
            get
            {
                var nameIdentifierClaim = this.identity.Claims.SingleOrDefault(c => c.ClaimType == ClaimTypes.NameIdentifier);
                if (nameIdentifierClaim == null)
                {
                    throw new DataServiceException(401, "Unauthorized", "The request requires authentication.", "en-US", null);
                }

                return nameIdentifierClaim.Value;
            }
        }

        /// <summary>
        /// Initializes service-wide policies. This method is called only once.
        /// </summary>
        /// <param name="config"></param>
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("Products", EntitySetRights.All);

            //TODO: change c&s back to read AllRead
            config.SetEntitySetAccessRule("Categories", EntitySetRights.All);
            config.SetEntitySetAccessRule("Suppliers", EntitySetRights.All);

            config.SetEntitySetPageSize("Products", 20);
            config.SetEntitySetPageSize("Categories", 20);
            config.SetEntitySetPageSize("Suppliers", 20);

            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
            config.UseVerboseErrors = true;
        }

        /// <summary>
        /// Define a query interceptor for the Products entity set.
        /// </summary>
        /// <returns></returns>
        [QueryInterceptor("Products")]
        public Expression<Func<Product, bool>> OnQueryProducts()
        {
            this.ValidateAuthorization("Products", PrivilegeConstants.SqlReadPrivilege);

            // The user has Read permission.
            return p => true;
        }

        /// <summary>
        /// Define a query interceptor for the Categories entity set.
        /// </summary>
        /// <returns></returns>
        [QueryInterceptor("Categories")]
        public Expression<Func<Category, bool>> OnQueryCategories()
        {
            this.ValidateAuthorization("Categories", PrivilegeConstants.SqlReadPrivilege);

            // The user has Read permission.
            return p => true;
        }

        /// <summary>
        /// Define a query interceptor for the Suppliers entity set.
        /// </summary>
        /// <returns></returns>
        [QueryInterceptor("Suppliers")]
        public Expression<Func<Supplier, bool>> OnQuerySuppliers()
        {
            this.ValidateAuthorization("Suppliers", PrivilegeConstants.SqlReadPrivilege);

            // The user has Read permission.
            return p => true;
        }

        /// <summary>
        /// Define a change interceptor for the Products entity set.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="operations"></param>
        [ChangeInterceptor("Products")]
        public void OnChangeProducts(Product product, UpdateOperations operations)
        {
            if (operations == UpdateOperations.Change)
            {
                this.ValidateAuthorization("Products", PrivilegeConstants.SqlUpdatePrivilege);

                var entry = default(ObjectStateEntry);
                if (this.CurrentDataSource.ObjectStateManager.TryGetObjectStateEntry(product, out entry))
                {
                    // Reject changes to a discontinued Product.
                    // Because the update is already made to the entity by the time the 
                    // change interceptor in invoked, check the original value of the Discontinued
                    // property in the state entry and reject the change if 'true'.
                    if ((bool)entry.OriginalValues["Discontinued"])
                    {
                        throw new DataServiceException(400, "Bad Request", "A discontinued product cannot be modified.", "en-US", null);
                    }
                }
                else
                {
                    throw new DataServiceException(404, "Not Found", "The requested product could not be found in the data source.", "en-US", null);
                }
            }
            else if (operations == UpdateOperations.Add)
            {
                this.ValidateAuthorization("Products", PrivilegeConstants.SqlCreatePrivilege);
            }
            else if (operations == UpdateOperations.Delete)
            {
                this.ValidateAuthorization("Products", PrivilegeConstants.SqlDeletePrivilege);

                var entry = default(ObjectStateEntry);
                if (this.CurrentDataSource.ObjectStateManager.TryGetObjectStateEntry(product, out entry))
                {
                    // Only a discontinued Product can be deleted.
                    if (!(bool)entry.OriginalValues["Discontinued"])
                    {
                        throw new DataServiceException(400, "Bad Request", "Products that are not discontinued cannot be deleted.", "en-US", null);
                    }
                }
                else
                {
                    throw new DataServiceException(404, "Not Found", "The requested product could not be found in the data source.", "en-US", null);
                }
            }
        }

        private static string BuildMessage(string entitySetName, string privilege)
        {
            var message = string.Empty;
            switch (privilege)
            {
                case PrivilegeConstants.SqlCreatePrivilege:
                    message = string.Format(CultureInfo.InvariantCulture, "You are not authorized to create new rows in the {0} entity set.", entitySetName);
                    break;
                case PrivilegeConstants.SqlReadPrivilege:
                    message = string.Format(CultureInfo.InvariantCulture, "You are not authorized to query the {0} entity set.", entitySetName);
                    break;
                case PrivilegeConstants.SqlUpdatePrivilege:
                    message = string.Format(CultureInfo.InvariantCulture, "You are not authorized to update rows in the {0} entity set.", entitySetName);
                    break;
                case PrivilegeConstants.SqlDeletePrivilege:
                    message = string.Format(CultureInfo.InvariantCulture, "You are not authorized to delete rows in the {0} entity set.", entitySetName);
                    break;

                default:
                    message = string.Format(CultureInfo.InvariantCulture, "You are not authorized to access the {0} entity set.", entitySetName);
                    break;
            }

            return message;
        }

        private void ValidateAuthorization(string entitySetName, string privilege)
        {
            if (!this.userPrivilegesRepository.HasUserPrivilege(this.UserId, privilege))
            {
                // The user does not have Read permission.
                throw new DataServiceException(401, "Unauthorized", BuildMessage(entitySetName, privilege), "en-US", null);
            }
        }
    }
}
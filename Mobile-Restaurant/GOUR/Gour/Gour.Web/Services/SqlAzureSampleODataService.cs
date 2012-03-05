namespace Gour.Web.Services
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Services;
    using System.Data.Services.Common;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Web;
    using System.Web.Security;
    using Gour.Web.Infrastructure;
    using Gour.Web.Models;
    using Gour.Web.UserAccountWrappers;

    // Summary:
    //     Sample OData Service exposing SQL Azure data.
    //     This sample service provides read-acess to all the collections of entities of the SQL Azure sample context,
    //     and validates the user credentials
    public class SqlAzureSampleODataService : DataService<ObjectContext>
    {

        private readonly HttpContextBase context;
        private readonly IFormsAuthentication formsAuth;
        private readonly IMembershipService membershipService;
        private readonly IUserPrivilegesRepository userPrivilegesRepository;


        public SqlAzureSampleODataService()
            : this(new HttpContextWrapper(HttpContext.Current), new SqlDataContext(), new FormsAuthenticationService(), new AccountMembershipService())
        {
        }


        [CLSCompliant(false)]
        public SqlAzureSampleODataService(HttpContextBase context, IUserPrivilegesRepository userPrivilegesRepository, IFormsAuthentication formsAuth, IMembershipService membershipService)
        {
            if ((context == null) && (HttpContext.Current == null))
            {
                throw new ArgumentNullException("context", "The context cannot be null if not running on a Web context.");
            }

            this.context = context;
            this.userPrivilegesRepository = userPrivilegesRepository;
            this.formsAuth = formsAuth;
            this.membershipService = membershipService;
        }

        private string UserId
        {
            get
            {
                string ticketValue = null;

                var cookie = this.context.Request.Cookies[this.formsAuth.FormsCookieName];
                if (cookie != null)
                {
                    // From cookie.
                    ticketValue = cookie.Value;
                }
                else if (this.context.Request.Headers["AuthToken"] != null)
                {
                    // From HTTP header.
                    ticketValue = this.context.Request.Headers["AuthToken"];
                }

                if (!string.IsNullOrWhiteSpace(ticketValue))
                {
                    FormsAuthenticationTicket ticket;

                    try
                    {
                        ticket = this.formsAuth.Decrypt(ticketValue);
                    }
                    catch
                    {
                        throw new DataServiceException((int)HttpStatusCode.Unauthorized, "The authorization ticket cannot be decrypted.");
                    }

                    if (ticket != null)
                    {
                        // Authorize Sql Azure OData Service usage.
                        var userId = this.membershipService.GetUser(new FormsIdentity(ticket).Name).ProviderUserKey.ToString();
                        if (!this.userPrivilegesRepository.HasUserPrivilege(userId, PrivilegeConstants.SqlUsagePrivilege))
                        {
                            throw new DataServiceException((int)HttpStatusCode.Unauthorized, "You have no permission to use SQL Azure.");
                        }

                        return userId;
                    }
                    else
                    {
                        throw new DataServiceException((int)HttpStatusCode.Unauthorized, "The authorization token is no longer valid.");
                    }
                }
                else
                {
                    throw new DataServiceException((int)HttpStatusCode.NotFound, "Resource not found.");
                }
            }
        }


        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("SqlSampleData", EntitySetRights.AllRead);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }

        [QueryInterceptor("SqlSampleData")]
        public Expression<Func<SqlSampleData, bool>> QuerySqlSampleData()
        {
            return c => c.IsPublic || c.UserId.Equals(this.UserId, StringComparison.OrdinalIgnoreCase);
        }

        protected override ObjectContext CreateDataSource()
        {
            var sampleDataContext = new SqlDataContext() as IObjectContextAdapter;
            return sampleDataContext.ObjectContext;
        }
    }
}